namespace HeadfulBrowserAudioVideoCapturingService;

public class StartRecordingSettings
{
    public bool Audio => true;

    public bool Video => true;

    public int TimeSliceMs => 100;

    public string MimeType => "video/webm;codecs=h264";

    public VideoConstraints VideoConstraints => new VideoConstraints();
}
