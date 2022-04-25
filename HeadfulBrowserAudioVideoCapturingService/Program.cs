using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public static class Program
{
    private const string ExtensionId = "jjndjgheafjngoipoacpjgeicjeomjli";

    public static async Task Main()
    {
        Console.WriteLine("Started...");

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
        var options = new LaunchOptions { Headless = false, Args = browserArgs, ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe" };
        await using var browser = await Puppeteer.LaunchAsync(options);

        var extensionTarget = await browser.WaitForTargetAsync(IsExtensionBackgroundPage);
        var extensionPage = await extensionTarget.PageAsync();

        var pages = await browser.PagesAsync();
        var page = pages[0];
        await page.GoToAsync("https://yaskovdev.github.io/video-and-audio-capturing-test/");
        await page.SetViewportAsync(new ViewPortOptions { Width = Constants.Width, Height = Constants.Height });
        await page.BringToFrontAsync();

        var capture = new CapturingService(extensionPage);

        using (var ffmpegWrapper = new FfmpegWrapper())
        {
            await using var inputStream = ffmpegWrapper.StartFfmpeg();

            await extensionPage.ExposeFunctionAsync<string, object?>("sendData", data =>
            {
                inputStream.Write(data.Select(c => (byte)c).ToArray());
                return null;
            });

            await capture.StartCapturing();
            Console.WriteLine("Press any key to stop capturing...");
            Console.ReadKey();
        }

        await capture.StopCapturing(); // TODO: shouldn't you first stop capturing and then dispose ffmpeg to avoid sending captured media to the closed stream?
    }

    private static bool IsExtensionBackgroundPage(Target target) =>
        target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{ExtensionId}");
}
