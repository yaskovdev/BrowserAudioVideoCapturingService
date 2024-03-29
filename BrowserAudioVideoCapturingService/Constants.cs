﻿namespace BrowserAudioVideoCapturingService;

// TODO: can we increase Width, Height and / or FrameRate? Will if affect CPU consumption much? Or it is mostly VideoEncoder that is affecting it?
public static class Constants
{
    public const int TimeSliceMs = 1000;
    
    public const int Width = 1280;

    public const int Height = 720;

    public const int FrameRate = 30;

    /// <summary>
    /// See https://developer.mozilla.org/en-US/docs/Web/Media/Formats/codecs_parameter#avc_profiles.
    /// </summary>
    public const string VideoEncoder = "avc1.424028";

    public const string AudioEncoder = "opus";
}
