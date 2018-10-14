// Write your JavaScript code.
if (document.getElementById("Monitor")) {
    var monitorVue = new Vue({
        el: '#Monitor',
        data: {
            latestReadings: null,
            transitionSettings: {
                duration: 1000,
                ease: d3.easeLinear
            },
            speedTrendSvg: {
                dict: {},
                value: null
            },
            totalBoardSvg: {
                value: null
            },
            legend: {
                value: null
            }
        },
        mounted: function () {
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
                    success: function(data) {
                        self.latestReadings = data;

                        if (!self.speedTrendSvg.value) {
                            self.speedTrendSvg.value = self.getSpeedTrendSvg();
                        } else {
                            self.speedTrendSvg.value.update(data);
                        }
                        if (!self.legend.value) {
                            self.legend.value = self.getLegend();
                        }
                        if (!self.totalBoardSvg.value) {
                            self.totalBoardSvg.value = self.getTotalBoardBarChartSvg();
                        } else {
                            self.totalBoardSvg.value.update(data);
                        }
                    },
                    error: function(error) {
                        console.log(error);
                    }
                });
            },
            updateDictForSpeedTrendSvg(data) {
                for (var i = 0; i < data.length; i++) {
                    var current = data[i];
                    var reading = {
                        dateTime: current.dateTime,
                        speed: current.speed
                    };
                    var dataAssociatedWithCurrentId = this.speedTrendSvg.dict[current.id];
                    if (!dataAssociatedWithCurrentId) {
                        this.speedTrendSvg.dict[current.id] = {
                            readings: [reading]
                        };
                    } else {
                        dataAssociatedWithCurrentId.readings.push(reading);
                    }
                }
            },
            getSpeedTrendSvg() {

                var svgSettings = this.getSvgSettingsByTarget("#Trend_svg"),
                    maximumDataLength = 30,
                    transitionSettings = this.transitionSettings,
                    timeBeforeDuration = new Date(Date.now() - transitionSettings.duration);

                var xScale = d3.scaleTime()
                    .range([0, svgSettings.width])
                    .domain([timeBeforeDuration - (maximumDataLength - 2), timeBeforeDuration - transitionSettings.duration]);

                var xAxis = d3.axisBottom(xScale);

                var xAxisSection = svgSettings.container.append("g")
                    .attr("class", "x axis")
                    .attr("transform", "translate(0," + svgSettings.height + ")")
                    .call(xAxis);

                var yScale = d3.scaleLinear()
                    .range([svgSettings.height, 0])
                    .domain([0, 100]);

                svgSettings.container.append("g")
                    .attr("class", "y axis")
                    .call(d3.axisLeft(yScale))
                    .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", "0.71em")
                    .attr("fill", "#000")
                    .text("Conveyor Speed");

                var line = d3.line()
                    .curve(d3.curveBasis)
                    .x(function (d) { return xScale(new Date(d.dateTime)); })
                    .y(function (d) { return yScale(d.speed); });

                var paths = svgSettings.container
                    .append("svg")
                    .append("g");

                var svgDict = this.speedTrendSvg.dict;
                var self = this;
                var lineChart = {};
                lineChart.update = (data) => {

                    self.updateDictForSpeedTrendSvg(data);

                    var now = new Date();
                    var colorFactor = 1;

                    for (var name in svgDict) {
                        var device = svgDict[name];
                        if (!device.path) {
                            var path = paths
                                .append('path')
                                .data(device.readings)
                                .attr('class', 'line')
                                .style('stroke', this.getColorByFactor(colorFactor++));
                            svgDict[name] = Object.assign(svgDict[name], { path: path });
                        }
                        device.path
                            .datum(device.readings)
                            .attr('d', line);

                        if (device.readings.length > maximumDataLength) {
                            device.readings.shift();
                        }
                    }

                    //// Shift domain
                    xScale.domain([
                        now - (maximumDataLength - 2) * transitionSettings.duration,
                        now - transitionSettings.duration
                    ]);

                    // Slide x-axis left
                    xAxisSection
                        .transition()
                        .duration(transitionSettings.duration)
                        .ease(transitionSettings.ease)
                        .call(xAxis);

                    //// Slide paths left
                    paths.attr('transform', null)
                        .transition()
                        .duration(transitionSettings.duration)
                        .ease(transitionSettings.ease)
                        .attr('transform', 'translate(' + xScale(now - (maximumDataLength - 1) * transitionSettings.duration) + ')');
                };

                return lineChart;
            },
            getTotalBoardBarChartSvg() {

                var target = "#Bar_svg";
                var dataset = this.latestReadings.map((d) => ({
                    id: d.id,
                    boards: d.totalBoards
                }));

                var svgSettings = this.getSvgSettingsByTarget(target);
                var barSettings = {
                    minimumPadding: 50,
                    maximumWidth: 50
                };
                barSettings = Object.assign(barSettings,
                {
                    padding: d3.min([svgSettings.width / (dataset.length * 2), barSettings.minimumPadding]),
                    width: d3.min([svgSettings.width / dataset.length - barSettings.padding, barSettings.maximumWidth])
                });

                // x axis
                var xScale = d3.scaleBand()
                    .domain(dataset.map((d) => d.id))
                    .range([0, svgSettings.width])
                    .padding(barSettings.padding);

                var xAxis = d3.axisBottom()
                    .scale(xScale);

                var xAxisSection = svgSettings.container.append("g")
                    .attr("class", "x axis")
                    .attr("transform", "translate(0," + svgSettings.height + ")")
                    .call(xAxis);

                xAxisSection.selectAll("text")
                    .attr("transform", "rotate(355)");
                xAxisSection.append("text")
                    .attr("dx", "-1.5em")
                    .attr("color", "white")
                    .attr("x", svgSettings.width)
                    .attr("y", -5)
                    .text("Device");

                // y axis
                var yScale = d3.scaleLinear()
                    .domain([0, d3.max(dataset, (d) => d.boards)])
                    .range([svgSettings.height, 0]);
                
                var yAxis = d3.axisLeft()
                    .scale(yScale);

                var yAxisSection = svgSettings.container.append("g")
                    .attr("class", "y axis")
                    .call(yAxis);

                yAxisSection.append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", "0.71em")
                    .attr("fill", "#000")
                    .text("Total Boards");
                
                // bar creation
                var rects = svgSettings.container.selectAll("rect")
                    .data(dataset)
                    .enter()
                    .append("rect")
                    .attr("class", "bar")
                    .attr("x", (d) => xScale(d.id) - barSettings.width / 2)
                    .attr("y", (d) => yScale(d.boards))
                    .attr("width", barSettings.width)
                    .attr("height", (d) => svgSettings.height - yScale(d.boards))
                    .attr("fill", (d, i) => this.getColorByFactor(i + 1));

                // text label
                var texts = svgSettings.container.append("g")
                    .selectAll("text")
                    .data(dataset)
                    .enter()
                    .append("text")
                    .text((d) => d.boards)
                    .attr("x", (d => xScale(d.id)))
                    .attr("y", (d) => yScale(d.boards) - 5)
                    .attr("font-family", "sans-serif")
                    .attr("text-anchor", "middle");

                var barChart = {};
                var transitionSettings = this.transitionSettings;
                barChart.update = (data) => {

                    var newDataSet = data.map((d) => ({
                        id: d.id,
                        boards: d.totalBoards
                    }));

                    yScale.domain([0, d3.max(newDataSet, (d) => d.boards)]);
                    yAxis.scale(yScale);
                    yAxisSection
						.transition()
                        .duration(transitionSettings.duration)
                        .ease(transitionSettings.ease)
						.call(yAxis);

                    rects.data(newDataSet)
                        .transition()
                        .duration(transitionSettings.duration)
                        .ease(transitionSettings.ease)
                        .attr("y", function(d) { return yScale(d.boards); })
                        .attr("height", (d) => svgSettings.height - yScale(d.boards));

                    texts.data(newDataSet)
                        .text((d) => d.boards)
                        .attr("y", (d) => yScale(d.boards) - 5);            
                };

                return barChart;
            },
            getLegend() {
                var table = d3.select("#legend")
                    .append("table")
                    .attr('class', 'trend legend');

                var trs = table
                    .append("tbody").selectAll("tr")
                    .data(this.latestReadings)
                    .enter()
                    .append("tr");

                trs.append("td").append("svg")
                    .attr("width", '16')
                    .attr("height", '16')
                    .append("rect")
                    .attr("width", '16')
                    .attr("height", '16')
                    .attr("fill", (d, i) => this.getColorByFactor(i + 1));

                trs.append("td")
                    .text(function (d) { return d.id; });

                return table;
            },
			getSvgSettingsByTarget(target) {

                var svg = d3.select(target),
                    margin = { top: 20, right: 80, bottom: 30, left: 25 },
                    width = 480 - margin.left - margin.right,
                    height = 500 - margin.top - margin.bottom,
                    g = svg.append("g").attr("transform", "translate(" + margin.left + "," + margin.top + ")");

                return {
                    svg: svg,
                    margin: margin,
                    width: width,
                    height: height,
                    container: g
                };
            },
            getColorByFactor(factor) {
                return "rgb(" + (factor * 520) % 255 + ", " + (factor * 320) % 255 + ", " + (factor * 720) % 255 + ")";
            }
        }
    });
}