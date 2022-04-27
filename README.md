# Headful Browser Audio Video Capturing Service

## Running Locally

1. Run the service (see `Program.cs`).
2. Run `ffplay -i srt://127.0.0.1:4000` to see the output of the service.

## Running With Docker

```powershell
docker build -f HeadfulBrowserAudioVideoCapturingService/Dockerfile -t yaskovdev/headful-capturing-server .
docker run -p 4000:4000/udp -d yaskovdev/headful-capturing-server
ffplay -i srt://127.0.0.1:4000
```

```powershell
docker build -f HeadfulBrowserAudioVideoCapturingService/win.Dockerfile -t yaskovdev/headful-capturing-server .
```
