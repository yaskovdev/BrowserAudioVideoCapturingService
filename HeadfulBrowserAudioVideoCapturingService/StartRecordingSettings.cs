namespace HeadfulBrowserAudioVideoCapturingService;

public class StartRecordingSettings
{
    private const string Avc1ConstrainedBaseline = "avc1.424028";

    public bool Audio => true;

    public bool Video => true;

    public int TimeSliceMs => 1000;

    public string MimeType => $"video/webm; codecs=\"{Avc1ConstrainedBaseline}\"";

    public VideoConstraints VideoConstraints => new VideoConstraints();
}
