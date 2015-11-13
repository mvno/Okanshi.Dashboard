var React = require("react");
var ReactDOM = require("react-dom");
var LineChart = require("./charts.jsx").LineChart;
var ReactFauxDOM = require("react-faux-dom");

var Metrics = React.createClass({
    getInitialState: function() {
        return {
            error: undefined,
            data: undefined,
            selectedMetric: ""
        }
    },

    componentDidMount: function() {
        this.loadData();
    },

    loadData: function() {
        var url = "/api/instances/" + this.props.instanceName + "/metrics";
        d3.json(url, this.dataLoaded);
    },

    dataLoaded: function(err, data) {
        if (err) {
            this.setState({ error: err });
        } else {
            this.setState({ data: data });
        }
    },

    metricChanged: function(event) {
        this.setState({ selectedMetric: event.target.value });
    },

    mapMeasurement: function (name) {
        var metric = _.find(this.state.data, function(x) {
            return x.Name === name;
        });
        var windowSize = metric.WindowSize;
        var measurements = [];
        for (var i = metric.Measurements.length - 1; i >= 0; i--) {
            measurements.push(metric.Measurements[i]);
        }
        var duration = moment.duration(windowSize / 2);
        var elements = [];
        measurements.forEach(function (e, i, array) {
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

    render: function() {
        var error = this.state.error,
            data = this.state.data;

        if (error !== undefined) {
            return (
                <div className="alert alert-danger" role="alert">
                    {error}
                </div>
            );
        }

        if (data === undefined) {
            return (
                <p>Loading...</p>
            );
        }

        if (_.isEmpty(data)) {
            return (
                <div className="alert alert-warning">No metrics defined or used yet</div>
            );
        }

        var metricOptionTags = _.map(data, function(x) {
            return (<option value={x.Name} key={x.Name}>{x.Name}</option>);
        });

        var chart = null;
        if (this.state.selectedMetric !== "") {
            var measurements = this.mapMeasurement(this.state.selectedMetric);
            var margin = { top: 10, right: 20, left: 50, bottom: 30 };
            var height = 600 - margin.top - margin.bottom;
            var width = 1000 - margin.left - margin.right;

            var xScale = d3.time.scale()
                .range([0, width])
                .domain(d3.extent(measurements, function (d) { return d.x; }));

            var yScale = d3.scale.linear()
                .range([height, 0])
                .domain(d3.extent(measurements, function (d) { return d.y; }));

            var xAxis = d3.svg.axis()
                .scale(xScale)
                .orient("bottom");

            var yAxis = d3.svg.axis()
                .scale(yScale)
                .orient("left");

            var lineFunction = d3.svg.line()
                .x(function(d) { return xScale(d.x); })
                .y(function (d) { return yScale(d.y); });

            var svg = d3.select(ReactFauxDOM.createElement("svg"))
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom);

            var chart2 = svg.append("g")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

            chart2.append("rect")
                .attr("class", "overlay")
                .attr("width", width)
                .attr("height", height);

            chart2.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis);

            chart2.append("g")
                .attr("class", "y axis")
                .call(yAxis);

            chart2.append("path")
                .attr("class", "line")
                .datum(measurements)
                .attr("d", lineFunction);

            chart = svg.node().toReact();
        }

        return (
            <div className="details">
                <div>
                    <select onChange={this.metricChanged} value={this.state.selectedMetric}>
                        {this.state.selectedMetric === "" ? <option value="">Choose a metric...</option> : null}
                        {metricOptionTags}
                    </select>
                </div>
                {chart}
            </div>
        );
    }
});

var HealthChecks = React.createClass({
    getInitialState: function() {
        return {
            error: undefined,
            data: undefined
        };
    },

    componentDidMount: function() {
        this.loadData();
    },

    loadData: function() {
        var url = "/api/instances/" + this.props.instanceName + "/healthchecks";
        d3.json(url, this.dataLoaded);
    },

    dataLoaded: function(err, data) {
        if (err) {
            this.setState({ error: err });
        } else {
            this.setState({ data: data });
        }
    },

    render: function () {
        var error = this.state.error,
            data = this.state.data,
            rows = undefined;
        if (error !== undefined) {
            return (
                <div className="alert alert-danger" role="alert">
                    {error}
                </div>
            );
        }

        if (data === undefined) {
            return (
                <p>Loading...</p>
            );
        }

        if (_.isEmpty(data)) {
            return (
                <div className="alert alert-warning">No healthchecks defined</div>
            );
        }

        rows = _.map(data, function (x) {
            var success = x.Success === true,
                status = success ? "OK" : "Failed",
                rowClass = success ? "success" : "danger";
            return (<tr key={x.Name} className={rowClass}><td>{x.Name}</td><td>{status}</td></tr>);
        });

        return (
            <table className="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
        );
    }
});

var Tab = React.createClass({
    getInitialState: function () {
        return {
            name: undefined,
            isActive: false,
            header: undefined
        };
    },
    render: function () {
        var name = this.props.name,
            className = this.props.isActive === true ? "tab-pane active" : "tab-pane";
        return (
            <div role="tabpanel" className={className} id={name}>
                {this.props.children}
            </div>
        );
    }
});

var Tabs = React.createClass({
    getInitialState: function() {
        return {
            activeTabName: undefined
        };
    },
    selected: function(name) {
        this.setState({ activeTabName: name });
    },
    render: function () {
        var self = this;
        var tabHeaders = React.Children.map(this.props.children, function(child) {
            var name = child.props.name,
                isActiveTab = (child.props.isActive === true && self.state.activeTabName === undefined) || self.state.activeTabName === name,
                className = isActiveTab ? "active" : "",
                link = "#" + name,
                header = child.props.header,
                onClick = self.selected.bind(self, name);
            return (
                <li role="presentation" className={className} key={name}>
                    <a href={link} aria-controls={name} role="tab" data-toggle="tab" onClick={onClick}>{header}</a>
                </li>
            );
        });
        return (
            <div>
                <ul className="nav nav-tabs" role="tablist">
                    {tabHeaders}
                </ul>
                <div className="tab-content">
                    {this.props.children}
                </div>
            </div>
        );
    }
});

var divElement = document.getElementById("details");
var instanceName = divElement.getAttribute("data-name");
var tabs = (
    <Tabs>
        <Tab header="Health Checks" name="healthChecks" isActive={true}>
            <HealthChecks instanceName={instanceName} />
        </Tab>
        <Tab header="Metrics" name="metrics">
            <Metrics instanceName={instanceName} />
        </Tab>
    </Tabs>
);

ReactDOM.render(
    tabs,
    divElement
);
