
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
 
RUN git clone https://github.com/YungGbeb/DiscordWelcomeBot && cd DiscordWelcomeBot && dotnet restore && dotnet build

COPY config.ini /
COPY res /

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY --from=build-env /DiscordWelcomeBot/test/bin/Debug/netcoreapp3.1 netcoreapp3.1/

COPY --from=build-env /etc/config.ini  netcoreapp3.1/config.ini
WORKDIR /netcoreapp3.1

CMD ["./test"]