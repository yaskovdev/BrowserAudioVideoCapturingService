using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public class Capture
{
    public async Task StartCapturing(Page extensionPage, Page page, CaptureOptions opts)
    {
        await page.BringToFrontAsync();
        var settings = new object[] { new StartRecordingSettings() };
        await extensionPage.EvaluateFunctionAsync("(settings) => { START_RECORDING(settings) }", settings);

        // TODO: set encoder
    }

    public async Task StopCapturing(Page extensionPage)
    {
        await extensionPage.EvaluateFunctionAsync("(index) => { STOP_RECORDING(index) }", Constants.PageIndex);
    }
}
