FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG CONFIGURATION=Release
WORKDIR /build

COPY ./src ./src
COPY Directory.Build.props .
COPY nuget.config .

WORKDIR "./src"

RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet test --no-build
RUN dotnet publish "./cryptobank.api" -o ../out/api --no-build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS api
WORKDIR /app
EXPOSE 5000

COPY --from=build /build/out/api .

ENTRYPOINT ["dotnet", "cryptobank.api.dll"]