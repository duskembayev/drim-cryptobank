FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /test
ARG CONFIGURATION=Release

COPY ./src/cryptobank.api/*.csproj ./src/cryptobank.api/
COPY ./src/cryptobank.api.tests/*.csproj ./src/cryptobank.api.tests/
COPY cryptobank.sln .
COPY Directory.Build.props .
COPY nuget.config .

RUN dotnet restore

COPY ./src/ ./src/