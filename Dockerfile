FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env

WORKDIR /app
COPY ./ ./
RUN dotnet restore

WORKDIR /app/NanoPingPong
RUN dotnet publish -c Release -o out

WORKDIR /app/NanoPingPong.Web
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app/listener
COPY --from=build-env /app/NanoPingPong/out .
WORKDIR /app/web
COPY --from=build-env /app/NanoPingPong.Web/out .

WORKDIR /app
COPY docker-entrypoint.sh .

RUN apt-get update && apt-get install -y curl libgdiplus \
    && chmod +x docker-entrypoint.sh

VOLUME /run/logs

# optional
ENV DonationAddress=

# required but defaulted
ENV ASPNETCORE_URLS=http://+:2022
ENV Context=nano
ENV SeedFile=/run/secrets/nano-ping.seed
ENV TickSeconds=1

# required
ENV Node=
ENV WorkServer=

ENTRYPOINT [ "sh" ]
CMD [ "docker-entrypoint.sh" ]
