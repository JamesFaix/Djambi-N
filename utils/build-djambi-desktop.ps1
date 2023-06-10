mkdir ../desktop-dist

cd ../api/api.host
dotnet restore
dotnet build -c Release

cd ../../web
npm i
npm run build:prod

cd ../
Copy-Item -Force -Recurse ./api/api.host/bin/Release/netcoreapp3.1/* -Destination ./desktop-dist/server
Copy-Item -Force -Recurse ./web/dist/prod/* -Destination ./desktop-dist/client
Copy-Item -Force -Recurse ./utils/desktop-files/* -Destination ./desktop-dist

cd ./utils