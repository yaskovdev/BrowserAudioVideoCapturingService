using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public static class Program
{
    private const string ChromeExecutablePath = @"/usr/bin/google-chrome"; // TODO: unhardcode
    private const string ExtensionId = "jjndjgheafjngoipoacpjgeicjeomjli";

    public static async Task Main()
    {
        Console.WriteLine("Started...");

        await using (var ffmpegWrapper = new FfmpegWrapper())
        {
            await using (var inputStream = ffmpegWrapper.StartFfmpeg())
            {
                await using (var browser = await Puppeteer.LaunchAsync(ChromeLaunchOptions()))
                {
                    var extensionTarget = await browser.WaitForTargetAsync(IsExtensionBackgroundPage);
                    var extensionPage = await extensionTarget.PageAsync();

                    var pages = await browser.PagesAsync();
                    var page = pages[0];
                    await page.GoToAsync("https://yaskovdev.github.io/video-and-audio-capturing-test/");
                    await page.SetViewportAsync(new ViewPortOptions { Width = Constants.Width, Height = Constants.Height });

                    var capturingService = new CapturingService(extensionPage);

                    await extensionPage.ExposeFunctionAsync<string, Task>("sendData", async data =>
                    {
                        Console.WriteLine($"Going to write {data.Length} bytes");
                        await inputStream.WriteAsync(ToByteArray(data));
                    });

                    await capturingService.StartCapturing();
                    try
                    {
                        Console.WriteLine("If console is available, press any key to stop capturing...");
                        Console.ReadKey();
                    }
                    catch (InvalidOperationException)
                    {
                        await Task.Delay(-1);
                    }
                    Console.WriteLine("Going to stop capturing...");

                    await capturingService.StopCapturing();
                    Console.WriteLine("Capturing stopped");
                }
                Console.WriteLine("Browser closed");
            }
            Console.WriteLine("Input stream closed");
        }
        Console.WriteLine("ffmpeg closed");
    }

    private static bool IsConsolePresent()
    {
        try
        {
            var unused = Console.WindowHeight;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static LaunchOptions ChromeLaunchOptions()
    {
        var extensionDirectoryInfo = new DirectoryInfo("Extension");
        var extensionPath = extensionDirectoryInfo.FullName;
        var browserArgs = new[]
        {
            "--no-sandbox",
            "--autoplay-policy=no-user-gesture-required",
            $"--load-extension={extensionPath}",
            $"--disable-extensions-except={extensionPath}",
            $"--whitelisted-extension-id={ExtensionId}"
        };
        return new LaunchOptions { Headless = false, Args = browserArgs, ExecutablePath = ChromeExecutablePath };
    }

    private static byte[] ToByteArray(string buffer) => buffer.Select(c => (byte)c).ToArray();

    private static bool IsExtensionBackgroundPage(Target target) =>
        target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{ExtensionId}");
}
