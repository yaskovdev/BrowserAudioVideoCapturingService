using HeadfulBrowserAudioVideoCapturingService;
using PuppeteerSharp;

Console.WriteLine("Started...");
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
var extensionDirectoryInfo = new DirectoryInfo("Extension");
var extensionPath = extensionDirectoryInfo.FullName;
const string extensionId = "jjndjgheafjngoipoacpjgeicjeomjli";
var browserArgs = new[]
{
    "--no-sandbox",
    "--autoplay-policy=no-user-gesture-required",
    $"--load-extension={extensionPath}",
    $"--disable-extensions-except={extensionPath}",
    $"--whitelisted-extension-id={extensionId}"
};
var options = new LaunchOptions { Headless = false, Args = browserArgs, ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"};
await using var browser = await Puppeteer.LaunchAsync(options);

// await using var fileStream = File.Create("data.mp4");

var ffmpegWrapper = new FfmpegWrapper();
var inputStream = ffmpegWrapper.StartFfmpeg();

bool IsExtensionBackgroundPage(Target target) => target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{extensionId}");
var extensionTarget = await browser.WaitForTargetAsync(IsExtensionBackgroundPage);
var extensionPage = await extensionTarget.PageAsync();
await extensionPage.ExposeFunctionAsync<string, object?>("sendData", data =>
{
    Console.WriteLine($"Received {data.Length} bytes of data");
    inputStream.Write(data.Select(c => (byte)c).ToArray());
    return null;
});

var pages = await browser.PagesAsync();
var page = pages[0];
await page.GoToAsync("https://yaskovdev.github.io/video-and-audio-capturing-test/");
await page.SetViewportAsync(new ViewPortOptions { Width = 1920, Height = 1080 });
await page.BringToFrontAsync();

var capture = new CapturingService();

await capture.StartCapturing(extensionPage);
const int capturingDurationMs = 3600000;
await Task.Delay(capturingDurationMs);
await capture.StopCapturing(extensionPage);
