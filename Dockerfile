FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

WORKDIR /app
COPY ./ ./
RUN dotnet restore

WORKDIR /app/NanoPingPong
RUN dotnet publish -c Release -o out

WORKDIR /app/NanoPingPong.Web
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY docker-entrypoint.sh .
WORKDIR /app/listener
COPY --from=build-env /app/NanoPingPong/out .
WORKDIR /app/web
COPY --from=build-env /app/NanoPingPong.Web/out .
# End in /app/web so that wwwroot works.

RUN apt-get update && apt-get install -y curl libgdiplus \
    && chmod +x /app/docker-entrypoint.sh

VOLUME /run/logs

# optional
ENV DonationAddress=

# required but defaulted
ENV ASPNETCORE_URLS=http://+:2022
ENV Context=nano
ENV SeedFile=/run/secrets/nano-ping.seed
ENV TickSeconds=1
ENV Cache=true
ENV DefaultRaw=10000000000000000000000000000
#   ^ Default to 0.01 XNO or 0.1 BAN for ease of use.

# required
ENV Node=

# Can separate the two by overriding the command with one of the below:
# dotnet /app/listener/NanoPingPong.dll
# dotnet /app/web/NanoPingPong.Web.dll
CMD [ "sh", "/app/docker-entrypoint.sh" ]
