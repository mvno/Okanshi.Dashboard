module.exports = {
    entry: {
        "./Content/overview": "./../../Content/overview.jsx",
    },
    output: {
        path: __dirname,
        filename: "[name].js"
    },
    module: {
        loaders: [ {
                test: /\.jsx$/,
                exclude: /node_modules/,
                loader: "babel-loader",
                query: {
                    presets: ["react"]
                }
            } ]
    }
};