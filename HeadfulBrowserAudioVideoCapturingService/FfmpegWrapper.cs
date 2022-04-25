using System.Diagnostics;

namespace HeadfulBrowserAudioVideoCapturingService;

public class FfmpegWrapper : IDisposable
{
    private const string InputArgs = "-i -";
    private const string OutputArgs = "-vcodec copy -acodec aac -strict -2 -y -f mpegts \"srt://127.0.0.1:4000?mode=listener\"";

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

    public void Dispose()
    {
        _process.WaitForExit();
        Console.WriteLine("ffmpeg stopped");
    }
}
