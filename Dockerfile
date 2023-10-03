FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build
ARG CONFIGURATION=Release

COPY ./src/cryptobank.api/*.csproj ./src/cryptobank.api/
COPY ./src/cryptobank.api.tests/*.csproj ./src/cryptobank.api.tests/
COPY ./tools/bitcoin.xpub/*.csproj ./tools/bitcoin.xpub/
COPY cryptobank.sln .
COPY Directory.Build.props .
COPY nuget.config .

RUN dotnet restore

COPY ./src/ ./src/
COPY ./tools/ ./tools/

RUN dotnet publish ./src/cryptobank.api/cryptobank.api.csproj -o ./out/api/ --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS api
WORKDIR /app

COPY --from=build /build/out/api .

ENTRYPOINT ["dotnet", "cryptobank.api.dll"]
