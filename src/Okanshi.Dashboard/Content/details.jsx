var LineChart = require("./charts.jsx").LineChart;

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

var tabs = (
    <Tabs>
        <Tab header="Health Checks" name="healthChecks" isActive={true}></Tab>
        <Tab header="Metrics" name="metrics" />
    </Tabs>
);

ReactDOM.render(
    tabs,
    document.getElementById("test")
);
