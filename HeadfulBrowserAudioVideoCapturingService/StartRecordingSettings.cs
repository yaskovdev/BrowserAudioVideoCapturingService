namespace HeadfulBrowserAudioVideoCapturingService;

public class StartRecordingSettings
{
    public bool Audio => true;

    public bool Video => true;

    public int TimeSliceMs => 1000;

    public string MimeType => $"video/webm; codecs=\"{Constants.Encoder}\"";

    public VideoConstraints VideoConstraints => new();
}
