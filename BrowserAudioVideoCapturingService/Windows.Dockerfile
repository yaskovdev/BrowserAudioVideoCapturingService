FROM mcr.microsoft.com/windows:ltsc2019 AS base
WORKDIR /app

RUN powershell Set-ExecutionPolicy -Scope LocalMachine -ExecutionPolicy Bypass

ENV chocolateyVersion=1.4.0
RUN powershell -Command Invoke-Expression ((New-Object Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
RUN choco install googlechrome --version 124.0.6367.60 -y --ignore-checksums
RUN choco install ffmpeg -y
RUN choco install dotnet-6.0-sdk -y

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
ENTRYPOINT ["dotnet", "BrowserAudioVideoCapturingService.dll", "C:/Program Files/Google/Chrome/Application/chrome.exe"]
