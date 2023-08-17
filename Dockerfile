FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["./cryptobank.api", "./cryptobank.api"]
COPY ["cryptobank.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]

RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet test -c Release --no-build
RUN dotnet publish "./cryptobank.api" -c Release -o /api

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS api
WORKDIR /app
EXPOSE 5000
COPY --from=build /api .
ENTRYPOINT ["dotnet", "cryptobank.api.dll"]