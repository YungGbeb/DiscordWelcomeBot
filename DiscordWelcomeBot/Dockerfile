
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
 
RUN git clone https://github.com/YungGbeb/DiscordWelcomeBot && cd DiscordWelcomeBot && dotnet restore && dotnet build --configuration Release

COPY config.ini /
COPY res /res

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY --from=build-env /DiscordWelcomeBot/DiscordWelcomeBot/bin/Release/netcoreapp3.1 netcoreapp3.1/

COPY --from=build-env /config.ini  netcoreapp3.1/config.ini
COPY --from=build-env /DiscordWelcomeBot/DiscordWelcomeBot/res/ netcoreapp3.1/res/

WORKDIR /netcoreapp3.1

CMD ["./DiscordWelcomeBot"]