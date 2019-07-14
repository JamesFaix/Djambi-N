const fs = require('fs');
const path = require('path');
const webpack = require('webpack');

var configJson = fs.readFileSync("./../environment.json", "utf8");
var environmentConfig = JSON.parse(configJson);

module.exports = {
    entry: "./src/index.tsx",
    output: {
        filename: "bundle.js",
        path: __dirname + "/dist"
    },
    devtool: "source-map",
    resolve: {
        extensions: [".ts", ".tsx", ".js", ".json"]
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: "awesome-typescript-loader"
            },
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            },
            {
                test: /\.css$/,
                use: [
                    { loader: "style-loader" },
                    { loader: "css-loader" }
                ]
            }
        ]
    },
    externals: {
        "react": "React",
        "react-dom": "ReactDOM"
    },
    plugins: [
      new webpack.DefinePlugin({
        'process.env':{
          'API_URL': JSON.stringify(environmentConfig.apiAddress)
        }
      })
    ]
};