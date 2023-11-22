FROM linuxserver/ffmpeg:amd64-version-6.0-cli AS base
WORKDIR /app

COPY ["videoprocessor.xabe/setup.sh","setup.sh"]
# COPY ["videoprocessor.sh","videoprocessor.sh"]

RUN chmod +x ./setup.sh
# RUN chmod +x ./videoprocessor.sh

RUN ./setup.sh
RUN rm -f ./setup.sh

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy-amd64 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["videoprocessor.xabe/videoprocessor.xabe.csproj", "videoprocessor.xabe/"]
RUN dotnet restore "./videoprocessor.xabe/videoprocessor.xabe.csproj"  -r linux-x64
COPY . .
WORKDIR "/src/videoprocessor.xabe"

FROM build AS publish
RUN dotnet publish "./videoprocessor.xabe.csproj" -c Release -p:IsDocker=true -p:buildOS=linux --no-restore -r linux-x64 --self-contained false -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "videoprocessor.xabe.dll"]