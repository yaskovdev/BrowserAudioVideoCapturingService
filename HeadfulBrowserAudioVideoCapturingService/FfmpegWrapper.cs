using System.Diagnostics;

namespace HeadfulBrowserAudioVideoCapturingService;

public class FfmpegWrapper : IAsyncDisposable
{
    private const string InputArgs = "-re -i -";
    // TODO: with Docker the IP should be 0.0.0.0, also check if "-strict -2 -y" is needed
    private const string OutputArgs = "-vcodec copy -acodec aac -ar 44100 -strict -2 -y -f mpegts \"srt://127.0.0.1:4000?mode=listener\"";

    private readonly Process _process;

    public FfmpegWrapper() =>
        _process = new Process
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
