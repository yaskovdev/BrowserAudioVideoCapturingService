using System.Diagnostics;

namespace HeadfulBrowserAudioVideoCapturingService;

public class FfmpegWrapper
{
    private const string InputArgs = "-stream_loop -1 -re -i -";
    private const string OutputArgs = "-vcodec copy -acodec aac -strict -2 -y -f mpegts \"srt://127.0.0.1:4000?mode=listener\"";

    public Stream StartFfmpeg()
    {
        var process = new Process
        {
            StartInfo =
            {
                FileName = "ffmpeg.exe",
                Arguments = $"{InputArgs} {OutputArgs}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true
            }
        };
        process.Start();
        return process.StandardInput.BaseStream;
    }
}
