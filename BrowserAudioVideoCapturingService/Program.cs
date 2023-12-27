namespace BrowserAudioVideoCapturingService;

using System.Reflection;
using PuppeteerSharp;

public static class Program
{
    private const string ExtensionId = "jjndjgheafjngoipoacpjgeicjeomjli";

    private const string YouTubeVideoId = "8gR6uWsDaCI";

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

                    await extensionPage.ExposeFunctionAsync<string, string, Task>("sendData", async (streamId, data) =>
                    {
                        Console.WriteLine($"Going to write {data.Length / (double)1024:0.00} KB of media from stream {streamId}");
                        await inputStream.WriteAsync(ToByteArray(data));
                    });

                    await capturingService.StartCapturing(Constants.Width, Constants.Height, Constants.FrameRate);

                    if (Environment.UserInteractive)
                    {
                        Console.WriteLine("Press any key to stop capturing...");
                        Console.ReadKey();
                    }
                    else
                    {
                        await Task.Delay(Timeout.Infinite);
                    }
                    Console.WriteLine("Going to stop capturing...");

                    await capturingService.StopCapturing();
                    const int waitMs = 2 * Constants.TimeSliceMs;
                    Console.WriteLine($"Capturing stopped, waiting for {waitMs} ms to demonstrate that the extension will not send more data");
                    await Task.Delay(waitMs);
                }
                Console.WriteLine("Browser closed");
            }
            Console.WriteLine("Input stream closed");
        }
        Console.WriteLine("ffmpeg closed");
    }

    private static LaunchOptions ChromeLaunchOptions(string chromeExecutablePath)
    {
        var extensionPath = GetResourcePath("Extension");
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

    private static string GetResourcePath(string name)
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var uriBuilder = new UriBuilder(location);
        var path = Uri.UnescapeDataString(uriBuilder.Path);
        return Path.Combine(Path.GetDirectoryName(path) ?? "", name);
    }
}
