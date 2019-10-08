FROM mcr.microsoft.com/mssql/server:2017-latest AS base
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD="GR3@Tpassword"
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["utils/db-reset/db-reset.fsproj", "utils/db-reset/"]
RUN dotnet restore "utils/db-reset/db-reset.fsproj"
COPY . .
WORKDIR "/src/utils/db-reset"
RUN dotnet build "db-reset.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "db-reset.fsproj" -c Release -o /app/publish --self-contained -r linux-x64

FROM base AS final
WORKDIR /app
COPY ["db/", "db/"]
COPY --from=publish /app/publish .
ENV DJAMBI_masterConnectionString="Data Source=localhost;Initial Catalog=master;User Id=SA;Password=GR3@Tpassword;"
ENV DJAMBI_djambiConnectionString="Data Source=localhost;Initial Catalog=djambi;User Id=SA;Password=GR3@Tpassword;"
ENV DJAMBI_sqlRoot="db"