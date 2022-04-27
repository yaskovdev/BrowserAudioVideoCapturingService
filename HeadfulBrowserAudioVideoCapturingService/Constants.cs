namespace HeadfulBrowserAudioVideoCapturingService;

public static class Constants
{
    public const int Width = 1920;

    public const int Height = 1080;

    public const int FrameRate = 15;

    /// <summary>
    /// See https://developer.mozilla.org/en-US/docs/Web/Media/Formats/codecs_parameter#avc_profiles.
    /// </summary>
    public const string VideoEncoder = "avc1.424028";

    // TODO: what audio codecs does InputComponent expect (previously it was absent; InputComponent wanted aac)?
    public const string AudioEncoder = "opus";
}
