const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = () => {
    return {
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
        externals: {
            "react": "React",
            "react-dom": "ReactDOM"
        },
        plugins: [
            new webpack.DefinePlugin({
              'process.env':{
                API_URL: JSON.stringify(process.env.DJAMBI_apiAddress)
              }
            }),
            new CopyWebpackPlugin([
                {
                    from: "index.html",
                    to: "index.html"
                },
                {
                    from: "resources",
                    to: "resources",
                    ignore: [ "**/*.psd" ]
                },
                {
                    from: "node_modules/react/umd/react.development.js",
                    to: "node_modules/react/umd/react.development.js"
                },
                {
                    from: "node_modules/react-dom/umd/react-dom.development.js",
                    to: "node_modules/react-dom/umd/react-dom.development.js"
                }
            ])
        ]
    };
};