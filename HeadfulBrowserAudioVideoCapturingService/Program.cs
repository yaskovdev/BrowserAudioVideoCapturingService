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
await using var browser = await Puppeteer.LaunchAsync(options);

bool Predicate(Target target) => target.Type == TargetType.BackgroundPage && target.Url.StartsWith($"chrome-extension://{extensionId}");
var extensionTarget = await browser.WaitForTargetAsync(Predicate);
var extensionPage = await extensionTarget.PageAsync();
await extensionPage.ExposeFunctionAsync<Args, object?>("sendData", args =>
{
    Console.WriteLine($"Received {args.data.Length} bytes of data");
    var bytes = new byte[args.data.Length];
    for (var i = 0; i < args.data.Length; i++)
    {
        bytes[i] = (byte)args.data[i];
    }
    File.WriteAllBytes("data.webm", bytes);
    return null;
});

var pages = await browser.PagesAsync();
var page = pages[0];
await page.GoToAsync("https://yaskovdev.github.io/video-and-audio-capturing-test/");
await page.SetViewportAsync(new ViewPortOptions { Width = 1920, Height = 1080 });
await page.BringToFrontAsync();

var capture = new Capture();

await capture.StartCapturing(extensionPage);
await Task.Delay(5000);
await capture.StopCapturing(extensionPage);
await Task.Delay(1000);
