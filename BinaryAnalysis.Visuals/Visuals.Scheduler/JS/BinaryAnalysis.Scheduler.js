/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Grid.js" />

BAUI.scheduler = {
    RunGetSettings: function (name, params) {
        if (!name) name = $("body").data("hurl").link.target;
        if (!name) { alert("Error: no name"); return; }

        var settings = $.parseJSON(window.external("scheduler").runGetSettings(name, params ? JSON.stringify(params) : undefined));
        var messageState = Enumerable.From(settings).First('$=>$.Key=="task_lastMessageState"').Value

        if (!(messageState < 4)
            && window.confirm("Error happened. Show settings?")) {
            this._showSettings(settings);
            return null;
        }

        return Enumerable.From(settings).ToObject("$.Key", "$.Value");
    },
    Run: function (name, params) {
        if (!name) name = $("body").data("hurl").link.target;
        if (!name) { alert("Error: no name"); return; }

        var settings = $.parseJSON(window.external("scheduler").run(name, params ? JSON.stringify(params) : undefined));
        if (settings) { //show settings
            this._showSettings(settings);
            return false;
        }
        return true;
    },
    _showSettings: function (settings) {
        //trace(Enumerable.From(Enumerable.From(settings).First('$=>$.Key=="task_msg_default"').Value).Select("$,i=>{Name:$.Key,Value:typeof($.Value)=='object'&&isEmptyObject($.Value)?'-':$.Value}").ToArray());
        BAUI.toolSet.cleanContainer();
        $("#ScheduleSettings-template").tmpl().appendTo(BAUI.$Container);
        var $container = $('#schedule-settings');
        var $list = $("<table />").attr("id", "grid-list").appendTo($container);

        var arr = Enumerable.From(settings)
            .Select("$=>{Name:$.Key,Type:typeof($.Value),Value:typeof($.Value)=='object'&&BAUI.toolSet.isEmptyObject($.Value)?'-':$.Value}")
            .ToArray();

        var p = $.extend(BAUI.grid.getOnePageGridParameters($list, arr), {
            colNames: ['Name', 'Value'],
            colModel: [
                { index: 'Name', name: 'Name', sortable: false, width: 300 },
                { index: 'Value', name: 'Value', sortable: false, width: 300 }
    		],
            subGrid: true,
            subGridRowExpanded: function (subgrid_id, row_id) {
                var name = $list.getRowData(row_id).Name;
                var val = Enumerable.From(settings).First('$=>$.Key=="' + name + '"').Value;
                if (typeof (val) == 'object') {
                    var subgrid_table_id = subgrid_id + "_t";
                    $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table>");
                    var sarr = Enumerable.From(val)
    						.Select("$,i=>{Name:$.Key,Value:typeof($.Value)=='object'&&isEmptyObject($.Value)?'-':$.Value}")
    						.ToArray();
                    var psselector = "#" + subgrid_table_id;
                    var ps = $.extend(BAUI.grid.getOnePageGridParameters(psselector, sarr), {
                        colNames: ['Name', 'Value', "Show"],
                        colModel: [
                            { index: 'Name', name: 'Name', sortable: false, width: 100 },
                            { index: 'Value', name: 'Value', sortable: false, width: 400 },
                            { index: 'Actions', name: '', sortable: false, width: 100, formatter: 'hurl',
                                formatoptions: { name: 'Show', action: 'eval', data: 'BAUI.actions.TextDialog()', target: function (v) {
                                    return escape(v.Value);
                                }
                                }
                            }
                        ]
                    });
                    $("#" + subgrid_table_id).jqGrid(ps);
                } else { //valuetype
                    $("#" + subgrid_id).html(name + ' = ' + val);
                }
            }
        });
        $list.jqGrid(p);
    },
    Available: function () {
        BAUI.toolSet.cleanContainer();
        $("#AvailableSchedules-template").tmpl().appendTo(BAUI.$Container);
        var $container = $('#schedule-container');
        var $list = $("<table />").attr("id", "grid-list").appendTo($container);

        var p = $.extend(BAUI.grid.getOnePageGridParameters($list, $.parseJSON(window.external("scheduler").available())), {
            colNames: ['Name'],
            colModel: [{ index: 'Name', name: 'Name', sortable: false, width: 600}],
            subGridRowExpanded: function (subgrid_id, row_id) {
                var subgrid_table_id = subgrid_id + "_t";
                $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "'></table>");
                var name = $list.getRowData(row_id).Name;
                var srprops = $.parseJSON(window.external("scheduler").propsRequired(name));
                var sprops = $.parseJSON(window.external("scheduler").props(name));
                var spe = Enumerable.From(srprops).Select('$.Name')
                var sarr = Enumerable.From(sprops)
                    .Select(function (o) { return { Name: o.Name, Value: o.Value, Rank: spe.Contains(o.Name) ? 1 : 0} })
                    .Where("$.Rank>0").ToArray()
                var ps = $.extend(BAUI.grid.getOnePageGridParameters("#" + subgrid_table_id, sarr), {
                    colNames: ['Name', 'Value'],
                    colModel: [
						                { index: 'Name', name: 'Name', sortable: false, width: 300 },
						                { index: 'Value', name: 'Value', sortable: false, width: 300, cellsubmit: 'clientArray', editable: true, edittype: 'text' }
					                ]
                });
                BAUI.grid.setEditableInline("#" + subgrid_table_id, ps, true);
                $("#" + subgrid_table_id).jqGrid(ps);

                $('<button />').html("Run")
				.click(function () {
				    var grid = $("#" + subgrid_table_id);
				    for (var i in grid.getDataIDs()) {
				        //$(grid).restoreRow(i, false, false, 'clientArray');
				        $(grid).saveRow(i, true, 'clientArray');
				    }
				    var paramz = Enumerable.From(grid.getRowData()).ToObject("$.Name", "$.Value");
				    BAUI.scheduler.Run(name, paramz);
				})
				.appendTo($("#" + subgrid_id));
            },
            subGrid: true
        });
        $list.jqGrid(p);
    }
}

