var Chart = React.createClass({
    render: function() {
        return (
            <svg width={this.props.width} height={this.props.height}>{this.props.children}</svg>
        );
    }
});

var Line = React.createClass({
    getDefaultProps: function() {
        return {
            path: "",
            color: "blue",
            width: 2
        };
    },

    render: function() {
        return (
            <path d={this.props.path} stroke={this.props.color} storkeWidth={this.props.width} fill="none" />
        );
    }
});

var LineDataSerie = React.createClass({
    getDefaultProps: function() {
        return {
            data: [],
            interpolate: "linear"
        };
    },

    render: function() {
        var props = this.props,
            yScale = props.yScale,
            xScale = props.xScale;
        console.log(this.props.data);
        var path = d3.svg.line()
            .x(function(d) { return xScale(d.x); })
            .y(function (d) { return yScale(d.y); })
            .interpolate(this.props.interpolate);
        return (
            <Line path={path(this.props.data)} color={this.props.color} />
        );
    }
});

var LineChart = React.createClass({
    getDefaultProps: function() {
        return {
            width: 600,
            height: 300
        };
    },

    render: function() {
        var data = this.props.data,
            size = { width: this.props.width, height: this.props.height };

        var xScale = d3.time.scale()
            .range([0, size.width])
            .domain(d3.extent(data, function (d) { return d.x; }));

        var yScale = d3.scale.linear()
            .range([size.height, 0])
            .domain(d3.extent(data, function (d) { return d.y; }));

        return (
            <Chart width={this.props.width} height={this.props.height}>
                <LineDataSerie data={data} size={size} xScale={xScale} yScale={yScale} />
            </Chart>
        );
    }
});

var data = [{ x: 0, y: 1 }, { x: 1, y: 2 }, { x: 2, y: 3 }, { x: 3, y: 3 }, { x: 2, y: 4 }];

ReactDOM.render(
    <LineChart data={data} />,
    document.getElementById("test")
);
