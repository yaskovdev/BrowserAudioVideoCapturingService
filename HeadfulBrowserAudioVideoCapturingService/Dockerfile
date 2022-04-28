FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

RUN apt-get update && apt-get install -y xvfb ffmpeg wget && rm -rf /var/lib/apt/lists/* 

# TODO: find a way to install Google Chrome without --fix-broken hack
RUN wget https://dl.google.com/linux/direct/google-chrome-unstable_current_amd64.deb \
&& dpkg -i google-chrome-unstable_current_amd64.deb \
 ; apt-get -y update \
&& apt-get -y upgrade \
 ; apt -y --fix-broken install

FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build
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
RUN chmod +x ./run.sh
CMD ["./run.sh"]
