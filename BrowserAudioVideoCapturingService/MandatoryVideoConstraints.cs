namespace BrowserAudioVideoCapturingService;

public record MandatoryVideoConstraints(int MinWidth, int MaxWidth, int MinHeight, int MaxHeight, int MinFrameRate, int MaxFrameRate)
{
    public MandatoryVideoConstraints(int width, int height, int frameRate) : this(width, width, height, height, frameRate, frameRate)
    {
    }
}
