var React = require("react");
var ReactDOM = require("react-dom");
var LineChart = require("./charts.jsx").LineChart;

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
        $.getJSON("/api/instances/" + this.props.instanceName)
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
            self = this,
            instanceLink = "/instances/" + this.props.instanceName;
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
                    <h3><a href={instanceLink}>{self.props.header}</a></h3>
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
        $.getJSON("/api/instances")
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
