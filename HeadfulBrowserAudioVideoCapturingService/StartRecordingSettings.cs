namespace HeadfulBrowserAudioVideoCapturingService;

public class StartRecordingSettings
{
    public int index = Constants.PageIndex;
    public bool audio = true;
    public bool video = true;
    public int timesliceMs = 100;
    public string mimeType = "video/webm"; // TODO: replace with video/mp4
    public VideoConstraints videoConstraints = new VideoConstraints();
}
