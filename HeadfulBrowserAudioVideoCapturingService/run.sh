#!/bin/sh

xvfb-run --auto-servernum dotnet HeadfulBrowserAudioVideoCapturingService.dll "/usr/bin/google-chrome"
