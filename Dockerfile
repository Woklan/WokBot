#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build-env
FROM mcr.microsoft.com/dotnet/sdk:5.0.402-alpine3.13-amd64 as build-env

RUN mkdir -p /app/build /app/publish WokBot

COPY ["./", "WokBot/"]

RUN dotnet publish "WokBot/WokBot.sln" -c Release -o /app/publish/

#FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine
FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine

ENV docker = 1

RUN apk update && apk add \
    opus-dev \
    libsodium-dev \
    curl \
    python3 \
    ffmpeg ; \
    ln -sf python3 /usr/bin/python ;\
    rm -rf /var/lib/apt/lists/*

RUN mkdir -p /app/publish

COPY --from=build-env /app/publish/ /app/publish/

RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /app/publish/youtube-dl
#RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /app/publish
RUN chmod +rx /app/publish/youtube-dl
    

ENTRYPOINT ["dotnet", "/app/publish/WokBot.dll"]

