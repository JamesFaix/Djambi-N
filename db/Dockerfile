FROM mcr.microsoft.com/mssql/server:2017-latest AS base
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD="GR3@Tpassword"
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src
COPY ["utils/db-reset/", "utils/db-reset/"]
RUN dotnet publish "utils/db-reset/db-reset.fsproj" -c Release -o /app/publish --self-contained -r linux-x64

FROM base AS final
WORKDIR /app
COPY ["db/", "db/"]
COPY --from=build /app/publish .
ENV APEX_masterConnectionString="Data Source=localhost;Initial Catalog=master;User Id=SA;Password=GR3@Tpassword;"
ENV APEX_apexConnectionString="Data Source=localhost;Initial Catalog=Apex;User Id=SA;Password=GR3@Tpassword;"
ENV APEX_sqlRoot="db"