# Headful Browser Audio Video Capturing Service

## Running Locally

1. Run the service (see `Program.cs`). Make sure to pass path to Chrome executable as a program argument (e.g., `C:/Program Files/Google/Chrome/Application/chrome.exe` or `/usr/bin/google-chrome`).
2. Run `ffplay -i srt://127.0.0.1:4000` to see the output of the service.

## Running With Docker In Linux Container

```powershell
docker build -f HeadfulBrowserAudioVideoCapturingService/Dockerfile -t yaskovdev/headful-capturing-server .
docker run -p 4000:4000/udp -d yaskovdev/headful-capturing-server
ffplay -i srt://127.0.0.1:4000
```

## Running With Docker In Windows Container

```powershell
docker build -f HeadfulBrowserAudioVideoCapturingService/Windows.Dockerfile -t yaskovdev/headful-capturing-server .
docker run -p 4000:4000/udp -d yaskovdev/headful-capturing-server
ffplay -i srt://127.0.0.1:4000
```
