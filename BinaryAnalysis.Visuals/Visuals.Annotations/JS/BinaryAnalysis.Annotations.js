/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Grid.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\Redist\jquery.tmpl.js" />

BAUI.annotations = {
    ShowGraph: function (infos) {
        if (!infos) infos = $("body").data("hurl").link.target;
        if (!infos) { alert("Error: no infos"); return; }

        var info = Enumerable.From(infos.split(';')).Where('$!=""').ToArray();
        var entries = $.parseJSON(window.external("metrics").getMetricsEntries(info[0], info[1], info[2]));

        BAUI.toolSet.cleanContainer();
        $('#MetricsGraphs-template').tmpl().appendTo(BAUI.$Container);
        var $container = $('#metricsGrid');

        $('<div />').attr('id', 'metricsPlot').css({ height: '400px', width: '600px' }).appendTo($container);
        var values = Enumerable.From(entries).Select('[BAUI.grid.formatter["dateInt"]($.RecordDate),$.Value]').ToArray();
        var plot1 = $.jqplot('metricsPlot', [values], { title: infos,
            axes: { xaxis: { renderer: $.jqplot.DateAxisRenderer} },
            series: [{ color: '#5FAB78', lineWidth: 1, markerOptions: { style: 'dot'}}]
        });
    },
    Health: function () {
        BAUI.toolSet.cleanContainer();
        $('#HealthGraphs-template').tmpl().appendTo(BAUI.$Container);
        var $container = $('#healthGrid');

        var arr = $.parseJSON(window.external("metrics").getHealthItems());
        for (var i in arr) {
            (function (index) {
                $('<h2 />').text(arr[index].Name).appendTo($container);
                var $mgrid = $("<table />").attr("id", "healthGrid-" + index).appendTo($container);

                //'<input type="button" value="G" onclick="BAUI.backchannel.send(\'js_showMetricsGraph\',\'Taxon;' + arr[index].Id + ';' + name + '\')" />'
                var p = BAUI.grid.getOnePageGridParameters($mgrid, arr[index].Arr);
                $.extend(p,
                {
                    colNames: $.parseJSON(window.external("metrics").getMetricsColNames())
                        .concat(["Actions"]),
                    colModel: $.parseJSON(window.external("metrics").getMetricsColModel())
                        .concat([{ index: 'Name', name: '', sortable: false, width: 100, formatter: 'hurl',
                            formatoptions: { name: 'Graph', action: 'eval', data: 'BAUI.annotations.ShowGraph()',
                                target: function (d) { return 'Taxon;' + arr[index].Id + ';' + d.Name; }
                            }
                        }])
                });
                $mgrid.jqGrid(p);
                BAUI.grid._applyCustomGridValues($mgrid);
            })(i);
        }
    }
}
