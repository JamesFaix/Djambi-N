FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["api/api.host/api.host.fsproj", "api/api.host/"]
COPY ["api/api.db/api.db.fsproj", "api/api.db/"]
COPY ["api/api.db.interfaces/api.db.interfaces.fsproj", "api/api.db.interfaces/"]
COPY ["api/api.common/api.common.fsproj", "api/api.common/"]
COPY ["api/api.model/api.model.fsproj", "api/api.model/"]
COPY ["utils/client-generator.annotations/client-generator.annotations.fsproj", "utils/client-generator.annotations/"]
COPY ["api/api.logic.interfaces/api.logic.interfaces.fsproj", "api/api.logic.interfaces/"]
COPY ["api/api.logic/api.logic.fsproj", "api/api.logic/"]
COPY ["api/api.web/api.web.fsproj", "api/api.web/"]
COPY ["api/api.web.interfaces/api.web.interfaces.fsproj", "api/api.web.interfaces/"]
RUN dotnet restore "api/api.host/api.host.fsproj"
COPY . .
WORKDIR "/src/api/api.host"
RUN dotnet build "api.host.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api.host.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DJAMBI_apiAddress="http://*:5100"
ENTRYPOINT ["dotnet", "Djambi.Api.dll"]