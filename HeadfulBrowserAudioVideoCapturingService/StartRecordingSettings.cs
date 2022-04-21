namespace HeadfulBrowserAudioVideoCapturingService;

public class StartRecordingSettings
{
    public bool Audio => true;

    public bool Video => true;

    public int TimeSliceMs => 100;

    public string MimeType => "video/webm"; // TODO: replace with video/mp4 once you have Chrome instead of Chromium

    public VideoConstraints VideoConstraints => new VideoConstraints();
}
