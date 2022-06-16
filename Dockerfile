FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

WORKDIR /app
COPY ./ ./

WORKDIR /app/NanoPingPong
RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY --from=build-env /app/NanoPingPong/out .

ENV Context=nano
ENV SeedFile=/run/secrets/nano-ping.seed
ENV TickSeconds=1
ENV Node
ENV WorkServer
ENV DonationAddress # optional

VOLUME /run/logs

ENTRYPOINT [ "dotnet" ]
CMD [ "NanoPingPong.dll" ]
