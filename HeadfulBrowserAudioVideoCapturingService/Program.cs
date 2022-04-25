using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public static class Program
{
    private const string ChromeExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // TODO: unhardcode
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
                    await page.BringToFrontAsync();

                    var capturingService = new CapturingService(extensionPage);
                    
                    await using var fileStream = File.Create("data.webm");

                    await extensionPage.ExposeFunctionAsync<string, Task>("sendData", async data =>
                    {
                        Console.WriteLine($"Going to write {data.Length} bytes");
                        await File.AppendAllTextAsync("data.txt", data);
                        await inputStream.WriteAsync(ToByteArray(data));
                        await fileStream.WriteAsync(ToByteArray(data));
                    });

                    await capturingService.StartCapturing();
                    Console.WriteLine("Press any key to stop capturing...");
                    Console.ReadKey();
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
