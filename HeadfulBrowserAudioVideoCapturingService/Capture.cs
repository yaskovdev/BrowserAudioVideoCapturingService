using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public class Capture
{
    public async Task StartCapturing(Page extensionPage)
    {
        var settings = new object[] { new StartRecordingSettings() };
        await extensionPage.EvaluateFunctionAsync("START_RECORDING", settings);
    }

    public async Task StopCapturing(Page extensionPage)
    {
        await extensionPage.EvaluateFunctionAsync("STOP_RECORDING", Constants.PageIndex);
    }
}
