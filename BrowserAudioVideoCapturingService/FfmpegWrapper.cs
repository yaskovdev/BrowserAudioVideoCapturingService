namespace BrowserAudioVideoCapturingService;

using System.Diagnostics;

public class FfmpegWrapper : IAsyncDisposable
{
    private const string InputArgs = "-re -i -";
    private const string OutputArgs = "-c copy -f mpegts \"srt://0.0.0.0:4000?mode=listener\"";

    private readonly Process _process = new()
    {
        StartInfo =
        {
            FileName = "ffmpeg",
            Arguments = $"{InputArgs} {OutputArgs}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true
        }
    };

    public Stream StartFfmpeg()
    {
        _process.Start();
        return _process.StandardInput.BaseStream;
    }

    public async ValueTask DisposeAsync()
    {
        await _process.WaitForExitAsync();
        GC.SuppressFinalize(this);
    }
}
