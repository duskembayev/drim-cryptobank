FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /build

COPY ./src ./src
COPY Directory.Build.props .
COPY nuget.config .

WORKDIR "./src"

RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet test -c Release --no-build
RUN dotnet publish "./cryptobank.api" -c Release -o ../out/api --no-build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS api
WORKDIR /app
EXPOSE 5000
COPY --from=build /build/out/api .
ENTRYPOINT ["dotnet", "cryptobank.api.dll"]