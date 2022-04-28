FROM mcr.microsoft.com/windows:20H2 AS base
WORKDIR /app

RUN powershell Set-ExecutionPolicy -Scope LocalMachine -ExecutionPolicy Bypass

RUN powershell -Command iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))
RUN choco install -y googlechrome
RUN choco install -y ffmpeg
RUN choco install -y dotnet-6.0-runtime

FROM mcr.microsoft.com/dotnet/sdk:6.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["HeadfulBrowserAudioVideoCapturingService/HeadfulBrowserAudioVideoCapturingService.csproj", "HeadfulBrowserAudioVideoCapturingService/"]
RUN dotnet restore "HeadfulBrowserAudioVideoCapturingService/HeadfulBrowserAudioVideoCapturingService.csproj"
COPY . .
WORKDIR "/src/HeadfulBrowserAudioVideoCapturingService"
RUN dotnet build "HeadfulBrowserAudioVideoCapturingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HeadfulBrowserAudioVideoCapturingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HeadfulBrowserAudioVideoCapturingService.dll", "C:/Program Files/Google/Chrome/Application/chrome.exe"]
