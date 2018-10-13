// Write your JavaScript code.
if (document.getElementById("Monitor")) {
    var monitorVue = new Vue({
        el: '#Monitor',
        data: {
            latestReadings: [],
            speedReadings: [],
            deviceReadings: []
        },
        mounted: function () {
            this.fetchDeviceReadings();
            
            setInterval(function () {
                this.fetchDeviceReadings();
            }.bind(this), 1000);
        },
        methods: {
            fetchDeviceReadings() {
                var self = this;
                $.ajax({
                    url: 'api/DeviceReading/',
                    method: 'GET',
                    success: function (data) {
                        self.latestReadings = self.getLatestReadings(data);
                        self.speedReadings = self.getSpeedReadings(data);
                        self.drawSpeedTrendSvg();
                    },
                    error: function (error) {
                        console.log(error);
                    }
                });
            },
            getLatestReadings(data) {
                return data.map(val => {
                    var lastReading = val.readings[val.readings.length - 1];
                    var reducer = (accumulator, currentValue) => accumulator + currentValue;
                    return {
                        speed: lastReading.speed,
                        dateTime: lastReading.dateTime,
                        packageTrackingAlarmState: lastReading.packageTrackingAlarmState,
                        totalBoards: val.readings.map(r => r.currentBoards).reduce(reducer),
                        recipeCount: val.readings.map(r => r.currentRecipeCount).reduce(reducer)
                    };
                });
            },
            getSpeedReadings(data) {
                return data.map(val => {
                    return {
                        id: val.id,
                        values: val.readings.map(r => {
                            return { dateTime: new Date(r.dateTime), speed: +r.speed };
                        })
                    };
                });
            },
            drawSpeedTrendSvg() {
                var trendSvg = d3.select("#Trend_svg"),
                    margin = { top: 20, right: 80, bottom: 30, left: 50 },
                    width = trendSvg.attr("width") - margin.left - margin.right,
                    height = trendSvg.attr("height") - margin.top - margin.bottom,
                    g = trendSvg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

                var x = d3.scaleTime().range([0, width]),
                    y = d3.scaleLinear().range([height, 0]),
                    z = d3.scaleOrdinal(d3.schemeCategory10);

                var line = d3.line()
                    .curve(d3.curveBasis)
                    .x(function (d) { return x(d.dateTime); })
                    .y(function (d) { return y(d.speed); });

                x.domain(d3.extent(this.speedReadings[0].values, function(v) {
                    return v.dateTime;
                }));

                y.domain([
                    d3.min(this.speedReadings, function (c) { return d3.min(c.values, function (d) { return d.speed; }); }),
                    d3.max(this.speedReadings, function (c) { return d3.max(c.values, function (d) { return d.speed; }); })
                ]);

                z.domain(this.speedReadings.map(function (d) { return d.id; }));

                g.append("g")
                    .attr("class", "axis axis--x")
                    .attr("transform", "translate(0," + height + ")")
                    .call(d3.axisBottom(x));

                g.append("g")
                    .attr("class", "axis axis--y")
                    .call(d3.axisLeft(y))
                    .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", "0.71em")
                    .attr("fill", "#000")
                    .text("Conveyor Speed");

                var deviceReading = g.selectAll(".deviceReading")
                    .data(this.speedReadings)
                    .enter().append("g")
                    .attr("class", "deviceReading");

                deviceReading.append("path")
                    .attr("class", "line")
                    .attr("d", function (d) { return line(d.values); })
                    .style("stroke", function (d) { return z(d.id); });

                deviceReading.append("text")
                    .datum(function (d) { return { id: d.id, value: d.values[d.values.length - 1] }; })
                    .attr("transform",
                        function (d) { return "translate(" + x(d.value.dateTime) + "," + y(d.value.speed) + ")"; })
                    .attr("x", 3)
                    .attr("dy", "0.35em")
                    .style("font", "10px sans-serif")
                    .text(function (d) { return d.id; });
            }
        }
    });
}