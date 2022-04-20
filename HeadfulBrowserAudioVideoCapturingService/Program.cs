// See https://aka.ms/new-console-template for more information

using PuppeteerSharp;

Console.WriteLine("Started...");
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com");
await page.ScreenshotAsync("screenshot.png");
