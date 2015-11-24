var React = require("react");

var Chart = React.createClass({
    getDefaultProps: function() {
        return {
            margin: { top: 0, right: 20, left: 20, bottom: 30 }
        };
    },

    render: function () {
        var transform = "translate(" + this.props.margin.left + ", " + this.props.margin.top + ")";
        return (
            <svg width={this.props.width} height={this.props.height}>
                <g transform={transform}>
                    {this.props.children}
                </g>
            </svg>
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
            data: []
        };
    },

    render: function() {
        var props = this.props,
            yScale = props.yScale,
            xScale = props.xScale;
        var path = d3.svg.line()
            .x(function(d) { return xScale(d.x); })
            .y(function (d) { return yScale(d.y); });
        return (
            <Line path={path(this.props.data)} color={this.props.color} />
        );
    }
});

var LineChart = React.createClass({
    getDefaultProps: function() {
        return {
            width: 600,
            height: 300,
            header: undefined,
            margin: { top: 0, right: 20, left: 20, bottom: 30 },
            graphSize: { height: 50, width: 150 }
        };
    },

    render: function() {
        var data = this.props.data,
            size = { width: this.props.width, height: this.props.height };

        var xScale = d3.time.scale()
            .range([0, this.props.graphSize.width])
            .domain(d3.extent(data, function (d) { return d.x; }));

        var yScale = d3.scale.linear()
            .range([this.props.graphSize.height, 0])
            .domain(d3.extent(data, function (d) { return d.y; }));

        return (
            <div className="chartdiv">
                <p>{this.props.header}</p>
                <Chart width={this.props.width} height={this.props.height} margin={this.props.margin}>
                    <LineDataSerie data={data} size={size} xScale={xScale} yScale={yScale} />
                </Chart>
            </div>
        );
    }
});

module.exports = {
    LineChart: LineChart,
    Chart: Chart,
    LineDataSerie: LineDataSerie,
    Line: Line
};
