using PuppeteerSharp;

namespace HeadfulBrowserAudioVideoCapturingService;

public class CapturingService
{
    private readonly Page _extensionPage;

    public CapturingService(Page extensionPage)
    {
        _extensionPage = extensionPage;
    }

    public async Task StartCapturing() => await _extensionPage.EvaluateFunctionAsync("START_RECORDING", new StartRecordingSettings());

    public async Task StopCapturing() => await _extensionPage.EvaluateFunctionAsync("STOP_RECORDING");
}
