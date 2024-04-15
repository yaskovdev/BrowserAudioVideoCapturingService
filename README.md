# Browser Audio Video Capturing Service

## Running Locally

1. Run the service (see `Program.cs`). Make sure to pass path to a Chromium executable as a program argument
   (e.g., `"C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe"` or `/usr/bin/google-chrome`). Any
   Chromium-based browser should work, Chrome and Edge were deliberately tested.
2. Run `ffplay -i srt://127.0.0.1:4000` to see the output of the service.

## Running With Docker In Linux Container

```powershell
docker build -f BrowserAudioVideoCapturingService/Dockerfile -t yaskovdev/browser-capturing-server .
docker run -p 4000:4000/udp -d yaskovdev/browser-capturing-server
ffplay -i srt://127.0.0.1:4000
```

## Running With Docker In Windows Container

```powershell
docker build -f BrowserAudioVideoCapturingService/Windows.Dockerfile -t yaskovdev/browser-capturing-server .
docker run -p 4000:4000/udp -d yaskovdev/browser-capturing-server
ffplay -i srt://127.0.0.1:4000
```

## Troubleshooting

1. If you see an error similar to "failed to solve with frontend dockerfile.v0: failed to create LLB definition: no
   match
   for platform in manifest sha256:c9b2234a2284ac6bad70bf9a3e137c4b413624b18c6f3a6ef8ceb4aa547abe99: not found" when
   building the Docker Windows container, make sure you have switched your Docker from Linux containers to Windows
   containers.
2. If you have issues reaching `choco` host during `docker build`, it could be related to the Network
   adapter picked by Docker. Check `Get-NetIPInterface -AddressFamily IPv4 | Sort-Object -Property InterfaceMetric -Descending`, Docker
   should use an adapter with the lowest id. Set the lowest id for your connection
   with `Set-NetIPInterface -InterfaceAlias '<your connection alias, most likely Wi-Fi>' -InterfaceMetric 1`.
3. If you are running locally with Edge, and it does not stream any content, it could be because Edge wants you to sing in first (ignoring the fact that you're running it programmatically).
   Try going to `HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge` and setting `BrowserSignin` to `0`. If it does not exist,
   create it. Its type should be `DWORD (32-bit) Value`. See [this article](https://www.tenforums.com/tutorials/162032-how-enable-disable-force-sign-microsoft-edge-chromium.html) for more details.
