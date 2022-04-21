using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public class CapturingService
{
    public async Task StartCapturing(Page extensionPage) => await extensionPage.EvaluateFunctionAsync("START_RECORDING", new StartRecordingSettings());

    public async Task StopCapturing(Page extensionPage) => await extensionPage.EvaluateFunctionAsync("STOP_RECORDING");
}
