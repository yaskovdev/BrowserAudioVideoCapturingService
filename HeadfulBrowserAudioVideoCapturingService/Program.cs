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
var options = new LaunchOptions { Headless = false, Args = browserArgs };
await using var browser = await Puppeteer.LaunchAsync(options);

await using var fileStream = File.Create("data.webm");

bool Predicate(Target target) => target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{extensionId}");
var extensionTarget = await browser.WaitForTargetAsync(Predicate);
var extensionPage = await extensionTarget.PageAsync();
await extensionPage.ExposeFunctionAsync<Args, object?>("sendData", args =>
{
    Console.WriteLine($"Received {args.data.Length} bytes of data");
    fileStream.Write(args.data.Select(c => (byte)c).ToArray());
    return null;
});

var pages = await browser.PagesAsync();
var page = pages[0];
await page.GoToAsync("https://yaskovdev.github.io/video-and-audio-capturing-test/");
await page.SetViewportAsync(new ViewPortOptions { Width = 1920, Height = 1080 });
await page.BringToFrontAsync();

var capture = new Capture();

await capture.StartCapturing(extensionPage);
const int capturingDurationMs = 5000;
await Task.Delay(capturingDurationMs);
await capture.StopCapturing(extensionPage);
