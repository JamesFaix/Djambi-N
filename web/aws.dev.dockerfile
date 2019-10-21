FROM node:10-alpine as base

FROM base as build
WORKDIR /app

COPY ["web/package*.json", "./"]
RUN npm install

COPY ["web/", "."]
RUN npm run build-aws-dev

FROM base as final
WORKDIR /app

COPY --from=build ["app/dist", "./dist/"]
COPY --from=build ["app/resources/", "./resources/"]
COPY --from=build ["app/index.html", "./index.html"]
COPY --from=build ["app/node_modules/react/umd/react.development.js", "./node_modules/react/umd/react.development.js"]
COPY --from=build ["app/node_modules/react-dom/umd/react-dom.development.js", "./node_modules/react-dom/umd/react-dom.development.js"]

RUN npm install http-server -g
EXPOSE 8081
CMD ["http-server", "-p 8081"]
