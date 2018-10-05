const fs = require('fs');
const path = require('path');
const webpack = require('webpack');

var configJson = fs.readFileSync("./../environment.json", "utf8");
var environmentConfig = JSON.parse(configJson);

module.exports = {
  entry: {
      "index": "./src/js/pages/index.ts"
  },
  output: {
    filename: '[name].js',
    path: path.resolve(__dirname, 'dist')
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: 'ts-loader',
        exclude: /node_modules/
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
  resolve: {
    extensions: [ '.ts', '.js' ]
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env':{
        'API_URL': JSON.stringify(environmentConfig.apiAddress)
      }
    })
  ]
};