namespace BrowserAudioVideoCapturingService;

using PuppeteerSharp;

public class CapturingService
{
    private readonly IPage _extensionPage;

    public CapturingService(IPage extensionPage)
    {
        _extensionPage = extensionPage;
    }

    public async Task StartCapturing(int width, int height, int frameRate) => await _extensionPage.EvaluateFunctionAsync("START_RECORDING", new StartRecordingSettings(width, height, frameRate));

    public async Task StopCapturing() => await _extensionPage.EvaluateFunctionAsync("STOP_RECORDING");
}
