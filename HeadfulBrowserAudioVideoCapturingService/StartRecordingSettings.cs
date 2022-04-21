namespace HeadfulBrowserAudioVideoCapturingService;

public class StartRecordingSettings
{
    public int Index => Constants.PageIndex;

    public bool Audio => true;

    public bool Video => true;

    public int TimeSliceMs => 100;

    public string MimeType => "video/webm"; // TODO: replace with video/mp4

    public VideoConstraints VideoConstraints => new VideoConstraints();
}
