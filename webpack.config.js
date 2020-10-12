const Path = require("path");
const Webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");

const Config = {
    htmlTemplate: "./src/index.html",
    fsEntry: "./src/SampleApp/SampleApp.fsproj",
    outputDir: "./dist",
    assetsDir: "./public",
    devServerPort: 8888,
    babelOptions: {}
}

const resolve = filePath =>
    Path.isAbsolute(filePath)
        ? filePath
        : Path.join(__dirname, filePath);

const isProduction =
    !process.argv.find(v => v.indexOf("serve") !== -1);

console.log(`resolve: ${resolve(".")}\n isProduction: ${isProduction}`);

const commonPlugins = [
    new HtmlWebpackPlugin({
        filename: "index.html",
        template: resolve(Config.htmlTemplate),
    })
]

module.exports = {
    entry: isProduction
        ? {
            app: [
                resolve(Config.fsEntry),
            ],
        }
        : {
            app: [
                resolve(Config.fsEntry)
            ]
        },
    output: {
        path: resolve(Config.outputDir),
        filename: isProduction ? "[name].[contenthash].js" : "[name].js",
    },
    mode: isProduction ? "production" : "development",
    devtool: isProduction ? false : "eval-source-map",
    plugins: isProduction
        ? commonPlugins.concat([
            new CopyWebpackPlugin({
                patterns: [{ from: resolve(Config.assetsDir) }],
            })
        ])
        : commonPlugins.concat([
            new Webpack.HotModuleReplacementPlugin(),
        ]),
    optimization: {
        chunkIds: "named"
    },
    resolve: {
        symlinks: false
    },
    devServer: {
        publicPath: "/",
        contentBase: resolve(Config.assetsDir),
        port: Config.devServerPort,
        hot: true,
        inline: true,
        contentBase: Config.outputDir,
        open: true
    },
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                use: {
                    loader: "fable-loader",
                    options: {
                        babel: Config.babelOptions,
                    }
                }
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: Config.babelOptions,
                },
            }
        ]
    }
}