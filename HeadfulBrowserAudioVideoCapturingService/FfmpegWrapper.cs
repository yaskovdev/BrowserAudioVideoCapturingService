using System.Diagnostics;

namespace HeadfulBrowserAudioVideoCapturingService;

public class FfmpegWrapper : IAsyncDisposable
{
    private const string InputArgs = "-re -i -";
    private const string OutputArgs = "-c copy -f mpegts \"srt://127.0.0.1:4000?mode=listener\""; // TODO: previously audio was re-encoded, may be still needed

    private readonly Process _process;

    public FfmpegWrapper()
    {
        _process = new Process
        {
            StartInfo =
            {
                FileName = "ffmpeg.exe",
                Arguments = $"{InputArgs} {OutputArgs}",
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardInput = true
            }
        };
    }

    public Stream StartFfmpeg()
    {
        _process.Start();
        return _process.StandardInput.BaseStream;
    }

    public async ValueTask DisposeAsync()
    {
        await _process.WaitForExitAsync();
        Console.WriteLine("ffmpeg stopped");
    }
}
