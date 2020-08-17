const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = env => {
    console.log(env);
    const isProd = env.NODE_ENV === "production";

    return {
        entry: "./src/index.tsx",
        output: {
            filename: "bundle.js",
            path: getOutputDirectory(isProd)
        },
        mode: process.env.NODE_ENV,
        devtool: getDevTool(isProd),
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
                },
                {
                    test: /\.less$/,
                    use: [
                        "style-loader",
                        "css-loader",
                        "less-loader"
                    ]
                }
            ]
        },
        optimization: {
            minimize: isProd
        },
        plugins: [
            new webpack.DefinePlugin({
              'process.env':{
                API_URL: JSON.stringify(env.DJAMBI_apiAddress),
                NODE_ENV: JSON.stringify(env.NODE_ENV)
              }
            }),
            new CopyWebpackPlugin([
                {
                    from: "index.html",
                    to: "index.html"
                },
                {
                    from: "manifest.json",
                    to: "manifest.json"
                },
                {
                    from: "resources",
                    to: "resources",
                    ignore: [ "**/*.psd" ]
                }
            ])
        ]
    };
};

function getOutputDirectory(isProd) {
    return isProd
        ? `${__dirname}/dist/prod`
        : `${__dirname}/dist/dev`;
}

function getDevTool(isProd) {
    return isProd ? undefined : "source-map";
}
