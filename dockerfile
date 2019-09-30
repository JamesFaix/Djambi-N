FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore

WORKDIR /app/api/api.host
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=build-env /app/api/api.host/out/ ./api/api.host/bin/Release/out/
COPY --from=build-env /app/environment.json ./
ENTRYPOINT ["dotnet", "api/api.host/bin/Release/out/Djambi.Api.dll"]