FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["test-netcore-app/test-netcore-app.csproj", "test-netcore-app/"]
RUN dotnet restore "test-netcore-app/test-netcore-app.csproj"
COPY . .
WORKDIR "/src/test-netcore-app"
RUN dotnet build "test-netcore-app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "test-netcore-app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "test-netcore-app.dll"]