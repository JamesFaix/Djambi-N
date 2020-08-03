FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["api", "api/"]

RUN dotnet tool restore --tool-manifest "api/.config/dotnet-tools.json"
RUN dotnet restore "api/api.host/api.host.fsproj"
RUN dotnet publish "api/api.host/api.host.fsproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app/publish .

# See https://github.com/dotnet/SqlClient/issues/220s
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "api.host.dll"]