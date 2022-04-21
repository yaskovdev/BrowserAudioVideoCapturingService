using HeadfulBrowserAudioVideoCapturingService;
using PuppeteerSharp;

Console.WriteLine("Started...");
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
// TODO: path should not be hardcoded, but most likely still full path
const string extensionPath = @"C:\dev\git_home\HeadfulBrowserAudioVideoCapturingService\extension";
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
var launchAsync = Puppeteer.LaunchAsync(options);
await using var browser = await launchAsync;

bool Predicate(Target target) => target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{extensionId}");
var extensionTarget = await browser.WaitForTargetAsync(Predicate);
var extensionPage = await extensionTarget.PageAsync();
await extensionPage.ExposeFunctionAsync<Args, object?>("sendData", args =>
{
    Console.WriteLine($"Data is {args.data}");
    File.WriteAllText("data.txt", args.data);
    return null;
});

var pages = await browser.PagesAsync();
var page = pages[0];
await page.GoToAsync("https://yaskovdev.github.io/video-and-audio-capturing-test/");
await page.SetViewportAsync(new ViewPortOptions { Width = 1920, Height = 1080 });

var capture = new Capture();

await capture.StartCapturing(extensionPage, page, new CaptureOptions());
await Task.Delay(5000);
await capture.StopCapturing(extensionPage);
await Task.Delay(300000);
