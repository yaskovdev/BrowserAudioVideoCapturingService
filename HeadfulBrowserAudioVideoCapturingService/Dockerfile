FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
ENTRYPOINT ["dotnet", "HeadfulBrowserAudioVideoCapturingService.dll"]
