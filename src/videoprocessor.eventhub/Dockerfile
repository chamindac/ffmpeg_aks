FROM linuxserver/ffmpeg:amd64-version-6.1.1-cli AS base
WORKDIR /app

RUN apt-get update && apt-get upgrade -y
RUN apt-get install -y dotnet-runtime-8.0

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy-amd64 AS build
WORKDIR /src
COPY ["videoprocessor.eventhub/videoprocessor.eventhub.csproj", "videoprocessor.eventhub/"]
COPY ["common-lib/common-lib.csproj", "common-lib/"]

RUN dotnet restore "videoprocessor.eventhub/videoprocessor.eventhub.csproj"  -r linux-x64
COPY . .
WORKDIR "/src/videoprocessor.eventhub"

FROM build AS publish
RUN dotnet publish "videoprocessor.eventhub.csproj" -c Release -p:IsDocker=true -p:buildOS=linux --no-restore -r linux-x64 --self-contained false -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "videoprocessor.eventhub.dll"]