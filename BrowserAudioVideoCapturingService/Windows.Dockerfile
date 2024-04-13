FROM mcr.microsoft.com/windows:20H2 AS base
WORKDIR /app

RUN powershell Set-ExecutionPolicy -Scope LocalMachine -ExecutionPolicy Bypass

RUN powershell -Command Invoke-Expression ((New-Object Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
RUN choco install -y microsoft-edge --version 123.0.2420.97
RUN choco install -y ffmpeg --version 7.0.0
RUN choco install -y dotnet-6.0-runtime --version 6.0.29

FROM mcr.microsoft.com/dotnet/sdk:6.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["BrowserAudioVideoCapturingService/BrowserAudioVideoCapturingService.csproj", "BrowserAudioVideoCapturingService/"]
RUN dotnet restore "BrowserAudioVideoCapturingService/BrowserAudioVideoCapturingService.csproj"
COPY . .
WORKDIR "/src/BrowserAudioVideoCapturingService"
RUN dotnet build "BrowserAudioVideoCapturingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrowserAudioVideoCapturingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrowserAudioVideoCapturingService.dll", "C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe"]
