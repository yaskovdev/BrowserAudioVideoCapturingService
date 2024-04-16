namespace BrowserAudioVideoCapturingService;

using System.Diagnostics;
using System.Reflection;
using PuppeteerSharp;

public static class Program
{
    private const string ExtensionId = "jjndjgheafjngoipoacpjgeicjeomjli";

    private const string YouTubeVideoId = "IMyqasy2Lco";

    private static int _receivedFirstChunk;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Starting stopwatch...");
        var chromeExecutablePath = args[0];
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await using (var ffmpegWrapper = new FfmpegWrapper())
        {
            await using (var inputStream = ffmpegWrapper.StartFfmpeg())
            {
                Console.WriteLine($"Launched ffmpeg, {stopwatch.ElapsedMilliseconds} ms passed");
                await using (var browser = await Puppeteer.LaunchAsync(ChromeLaunchOptions(chromeExecutablePath)))
                {
                    Console.WriteLine($"Launched the browser, {stopwatch.ElapsedMilliseconds} ms passed");

                    for (var i = 0; i < 8; i++)
                    {
                        _ = await browser.NewPageAsync();
                        Console.WriteLine($"Opened an empty warmup page {i}, {stopwatch.ElapsedMilliseconds} ms passed");
                    }

                    var page = await browser.NewPageAsync();
                    Console.WriteLine($"Opened the actual new page for YouTube, {stopwatch.ElapsedMilliseconds} ms passed");

                    await page.GoToAsync($"https://www.youtube.com/embed/{YouTubeVideoId}?autoplay=1&loop=1&playlist={YouTubeVideoId}");
                    Console.WriteLine($"Opened YouTube, {stopwatch.ElapsedMilliseconds} ms passed");

                    await page.SetViewportAsync(new ViewPortOptions { Width = Constants.Width, Height = Constants.Height });
                    Console.WriteLine($"Set viewport, {stopwatch.ElapsedMilliseconds} ms passed");

                    var extensionTarget = await browser.WaitForTargetAsync(IsExtensionBackgroundPage);
                    Console.WriteLine($"Found extension target, {stopwatch.ElapsedMilliseconds} ms passed");

                    var extensionPage = await extensionTarget.PageAsync();
                    Console.WriteLine($"Found extension page, {stopwatch.ElapsedMilliseconds} ms passed");

                    var capturingService = new CapturingService(extensionPage);
                    Console.WriteLine($"Created capturing service, {stopwatch.ElapsedMilliseconds} ms passed");

                    await extensionPage.ExposeFunctionAsync<string, string, Task>("sendData", async (streamId, data) =>
                    {
                        if (Interlocked.CompareExchange(ref _receivedFirstChunk, 1, 0) == 0)
                        {
                            Console.WriteLine($"Received first chunk, {stopwatch.ElapsedMilliseconds} ms passed");
                        }
                        Console.WriteLine($"Going to write {data.Length / (double)1024:0.00} KB of media from stream {streamId}");
                        await inputStream.WriteAsync(ToByteArray(data));
                    });
                    Console.WriteLine($"Exposed the capturing callback, {stopwatch.ElapsedMilliseconds} ms passed");

                    await capturingService.StartCapturing(Constants.Width, Constants.Height, Constants.FrameRate);
                    Console.WriteLine($"Started capturing, {stopwatch.ElapsedMilliseconds} ms passed");

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
        Console.WriteLine("Assembly is " + location);
        var directoryName = Path.GetDirectoryName(location) ?? throw new InvalidOperationException("Cannot get directory name for location " + location);
        Console.WriteLine("Assembly directory is " + directoryName);
        return Path.Combine(directoryName, name);
    }
}
