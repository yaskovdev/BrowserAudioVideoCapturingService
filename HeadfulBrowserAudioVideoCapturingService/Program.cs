using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public static class Program
{
    private const string ExtensionId = "jjndjgheafjngoipoacpjgeicjeomjli";

    private const string YouTubeVideoId = "ucZl6vQ_8Uo";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Starting...");
        var chromeExecutablePath = args[0];

        await using (var ffmpegWrapper = new FfmpegWrapper())
        {
            await using (var inputStream = ffmpegWrapper.StartFfmpeg())
            {
                await using (var browser = await Puppeteer.LaunchAsync(ChromeLaunchOptions(chromeExecutablePath)))
                {
                    var extensionTarget = await browser.WaitForTargetAsync(IsExtensionBackgroundPage);
                    var extensionPage = await extensionTarget.PageAsync();

                    var pages = await browser.PagesAsync();
                    var page = pages[0];
                    await page.GoToAsync($"https://www.youtube.com/embed/{YouTubeVideoId}?autoplay=1&loop=1&playlist={YouTubeVideoId}");
                    await page.SetViewportAsync(new ViewPortOptions { Width = Constants.Width, Height = Constants.Height });

                    var capturingService = new CapturingService(extensionPage);

                    await extensionPage.ExposeFunctionAsync<string, Task>("sendData", async data =>
                    {
                        Console.WriteLine($"Going to write {data.Length / (double)1024:0.00} KB of media");
                        await inputStream.WriteAsync(ToByteArray(data));
                    });

                    await capturingService.StartCapturing();
                    Console.WriteLine("If console is available, press any key to stop capturing...");
                    try
                    {
                        Console.ReadKey();
                    }
                    catch (InvalidOperationException)
                    {
                        await Task.Delay(Timeout.Infinite);
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

    private static LaunchOptions ChromeLaunchOptions(string chromeExecutablePath)
    {
        var extensionDirectoryInfo = new DirectoryInfo("Extension");
        var extensionPath = extensionDirectoryInfo.FullName;
        var browserArgs = new[]
        {
            "--no-sandbox",
            "--autoplay-policy=no-user-gesture-required",
            $"--load-extension={extensionPath}",
            $"--disable-extensions-except={extensionPath}",
            $"--allowlisted-extension-id={ExtensionId}",
            "--headless=new",
            "--hide-scrollbars"
        };
        return new LaunchOptions { Headless = false, Args = browserArgs, ExecutablePath = chromeExecutablePath };
    }

    private static byte[] ToByteArray(string buffer) => buffer.Select(c => (byte)c).ToArray();

    private static bool IsExtensionBackgroundPage(ITarget target) =>
        target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{ExtensionId}");
}
