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

var OkanshiMetric = React.createClass({
    getDefaultProps: function() {
        return {
            windowSize: undefined,
            measurements: [],
            width: 250,
            height: 100,
            name: undefined
        };
    },

    mapMeasurement: function () {
        this.props.measurements.reverse();
        var duration = moment.duration(this.props.windowSize / 2);
        var elements = [];
        this.props.measurements.forEach(function (e, i, array) {
            var time = moment(e.StartTime).add(duration);
            elements.push({ x: time.toDate(), y: e.Average });
            var nextIndex = i + 1;
            if (array.length <= nextIndex) { return; }
            var nextElement = array[nextIndex];
            var nextStartTime = moment(nextElement.StartTime);
            var firstEmptyPointTime = moment(e.EndTime).add(duration);
            if (firstEmptyPointTime.isBefore(nextStartTime)) {
                elements.push({ x: firstEmptyPointTime.toDate(), y: 0 });
                var lastEmptyPointTime = nextStartTime.subtract(duration);
                if (lastEmptyPointTime.isAfter(firstEmptyPointTime)) {
                    elements.push({ x: lastEmptyPointTime.toDate(), y: 0 });
                }
            }
        });
        var max = d3.max(elements, function (d) { return d.x; });
        var current = new Date();
        if (max < current) {
            var time = moment(max).add(duration).toDate();
            elements.push({ x: time, y: 0 }, { x: current, y: 0 });
        }
        return elements;
    },

    render: function () {
        var graphSize = { height: 75, width: 200 };
        return (
            <LineChart data={this.mapMeasurement(this.props.measurements)} width={this.props.width} height={this.props.height} header={this.props.name} graphSize={graphSize} />
        );
    }
});

var OkanshiInstance = React.createClass({
    getInitialState: function() {
        return {
            metrics: [],
            healthChecks: [],
            windowSize: undefined
        };
    },

    getDefaultProps: function() {
        return {
            header: undefined,
            width: 250,
            height: 100,
            instanceName: undefined,
            refreshRate: 10000
        };
    },

    getInstanceData: function () {
        var self = this;
        $.getJSON("/instances/" + this.props.instanceName)
            .done(function (data) {
                var metrics = data.Metrics,
                    healthChecks = data.HealthChecks,
                    windowSize = data.WindowSize;
                self.setState({
                    metrics: metrics,
                    healthChecks: healthChecks,
                    windowSize: windowSize
                });
            });
    },

    componentDidMount: function () {
        this.getInstanceData();
        this.timer = setInterval(this.getInstanceData, this.props.refreshRate);
    },

    componentWillUnmount : function() {
        clearInterval(this.timer);
    },

    render: function () {
        var healthCheckClass,
            self = this;
        if (_.some(self.state.healthChecks, function (h) { return h.Success === false; })) {
            healthCheckClass = "circle error";
        } else {
            healthCheckClass = "circle success";
        }

        var measurements = _.map(self.state.metrics, function (x) {
            return (
                <OkanshiMetric measurements={x.Measurements} width={self.props.width} height={self.props.height} windowSize={x.WindowSize.windowSize} name={x.Name} key={x.Name} />
            );
        });

        return (
            <div>
                <div className="header">
                    <h3>{self.props.header}</h3>
                    <div className={healthCheckClass}></div>
                </div>
                {measurements}
            </div>
        );
    }
});

var InstanceList = React.createClass({
    getInitialState: function() {
        return {
            instances: []
        }
    },

    componentDidMount: function() {
        this.getInstances();
    },

    getInstances: function () {
        var self = this;
        $.getJSON("/instances")
            .done(function(data) {
                self.setState({
                    instances: _.map(data, function(x) {
                            return {
                                name: x.Name,
                                url: x.Url,
                                refreshRate: x.RefreshRate
                            };
                        })
                });
            });
    },

    render: function () {
        var self = this;
        var instances = _.map(self.state.instances, function(x) {
            var name = x.name,
                refreshRate = x.refreshRate * 1000;
            return (
                <OkanshiInstance header={name} instanceName={name} key={name} refreshRate={refreshRate} />
            );
        });
        return (
            <div>
                {instances}
            </div>
        );
    }
});

ReactDOM.render(
    <InstanceList />,
    document.getElementById("chartContainer")
);
