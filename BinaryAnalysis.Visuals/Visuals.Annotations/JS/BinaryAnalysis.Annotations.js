/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Grid.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\Redist\jquery.tmpl.js" />

BAUI.annotations = {
    _stateClass: "BinaryAnalysis.Data.State.StateBoxMap, BinaryAnalysis.Data",
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
    ShowState: function (id) {
        if (!id) id = $("body").data("hurl").link.target;
        if (!id) { alert("Error: no id"); return; }
        BAUI.actions.BrowserWindow("State ID=" + id, window.external("state").loadFixedStateResponse(id));
    },
    ShowStateInEditor: function (id) {
        if (!id) id = $("body").data("hurl").link.target;
        if (!id) { alert("Error: no id"); return; }
        BAUI.actions.TextDialog(window.external("state").loadStateResponse(id));
    },
    _deleteState: function (id) {
        BAUI.form.deleteConfirm(this._stateClass, id, function () {
            $.hurl("update", { "action": 'eval', 'data': 'BAUI.annotations.StateBrowser()' });
        });
    },
    StateBrowser: function () {
        BAUI.toolSet.cleanContainer();
        $("#StateGrid-template").tmpl().appendTo(BAUI.$Container);
        var $container = $("#stateGrid");

        var $list = $("<table />").attr("id", "grid-list").appendTo($container);
        var $pager = $("<div />").attr("id", "grid-pager").appendTo($container);

        var p = $.extend(BAUI.grid.getBoxGridParameters($list, $pager, this._stateClass), {
            colNames: ['Id', 'Name', 'Description', "DieDate", "Actions"],
            colModel: [
                { index: 'Id', name: 'Id', sortable: false, width: 40 },
                { index: 'Name', name: 'Name', formatter: 'string', sortable: false, width: 200 },
                { index: 'Description', name: 'Description', formatter: 'string', sortable: false, width: 200 },
                { index: 'DieDate', name: 'DieDate', formatter: 'date', sortable: false, width: 150 },
                { name: 'act', name: 'actions', index: 'act', width: 120, formatter: 'multiformat',
                    formatoptions: [
                        { format: 'hurl', options: { name: 'I', action: 'eval', target: 'Id', data: 'BAUI.annotations.ShowState()'} },
                        { format: 'hurl', options: { name: 'T', action: 'eval', target: 'Id', data: 'BAUI.annotations.ShowStateInEditor()'} },
						{ format: 'func', options: function (s) {
						    return '<input type="button" value="D" onclick="BAUI.annotations._deleteState(\'' + s.Id + '\');" />'
						}
						}
                    ]
                }
            ]
        });

        $list.jqGrid(p).jqGrid('filterToolbar', {}).navGrid("#grid-pager",
		{ view: true, edit: false, search: false, add: false, del: false },
		{}, /*edit*/{}, /*add*/{},  /*del*/
		{multipleSearch: true }, /*search*/{closeOnEscape: true} /* view*/);
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
