#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["WokBot/WokBot.csproj", "WokBot/"]
#RUN dotnet restore "WokBot/WokBot.csproj" -r ubuntu.20.04-x64 /p:Platform=arm64
RUN dotnet restore "WokBot/WokBot.csproj" -r linux-arm64 -p:Platform=arm64
COPY . .
WORKDIR "/src/WokBot"
RUN dotnet build "WokBot.csproj" -c Release -o /app/build -r linux-arm64 -p:Platform=arm64
# RUN dotnet build "WokBot.csproj" -c Release -o /app/build -r ubuntu.20.04-x64 /p:Platform=arm64

FROM build AS publish
#RUN dotnet publish "WokBot.csproj" -c Release -o /app/publish -r ubuntu.20.04-x64 /p:Platform=arm64
RUN dotnet publish "WokBot.csproj" -c Release -o /app/publish -r linux-arm64 -p:Platform=arm64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ISDOCKER = 1
#RUN apt install opus-dev libsodium-dev curl ffmpeg
RUN apt-get update && apt install libopus-dev libsodium-dev curl ffmpeg python3 python3-pip -y && ln -sf python3 /usr/bin/python


RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /app/youtube-dl
RUN chmod a+rx /app/youtube-dl
RUN mkdir /app/resources
RUN pip3 install --no-cache --upgrade pip setuptools
ENTRYPOINT ["dotnet", "WokBot.dll"]

