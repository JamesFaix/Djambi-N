FROM node:10-alpine as base

FROM base as build
WORKDIR /app

# Install packages
COPY ["web/package*.json", "./"]
RUN npm install

# Build
COPY ["web/", "."]
# We need to set the environment variables during image building.
# This gets injected into the JS bundle by Webpack.
# This is different than a docker-compose file, where environment variables are set for containers.
ENV DJAMBI_apiAddress="http://localhost:5100"
RUN npm run build

# Copy artifacts
FROM base as final
WORKDIR /app

COPY --from=build ["app/dist", "./dist/"]
COPY --from=build ["app/resources/", "./resources/"]
COPY --from=build ["app/index.html", "./index.html"]
COPY --from=build ["app/node_modules/react/umd/react.development.js", "./node_modules/react/umd/react.development.js"]
COPY --from=build ["app/node_modules/react-dom/umd/react-dom.development.js", "./node_modules/react-dom/umd/react-dom.development.js"]

# Start web server
RUN npm install http-server -g
EXPOSE 8080
CMD "http-server"
