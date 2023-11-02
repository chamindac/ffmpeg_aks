FROM linuxserver/ffmpeg:amd64-version-6.0-cli AS base
WORKDIR /app

COPY ["setup.sh","setup.sh"]
COPY ["videoprocessor.sh","videoprocessor.sh"]

RUN chmod +x ./setup.sh
RUN chmod +x ./videoprocessor.sh

RUN ./setup.sh
RUN rm -f ./setup.sh

ENTRYPOINT ["sh", "videoprocessor.sh" ]