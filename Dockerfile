FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

WORKDIR /app
COPY ./ ./

WORKDIR /app/NanoPingPong
RUN dotnet restore
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY --from=build-env /app/WaxRentals/out .

ENTRYPOINT [ "dotnet" ]

CMD [ "NanoPingPong.dll" ]
