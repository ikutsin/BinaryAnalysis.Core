/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Grid.js" />

// :::: Evaler start
(function ($) {
    $.fn.evaluator = function (options) {
        var opts = $.extend($.fn.evaluator.defaults, options);
        return this.each(function () { //logic with this goes here
            var timer;
            var buffer = "";

            var $this = $(this);
            var $debug = $('<div />').addClass("evaluator"); //.resizable({maxHeight:300,minHeight:300});

            $('<input></input>').attr('type', 'checkbox').click(function (c) {
                clearInterval(timer);
                if (c.target.checked) timer = setInterval(evalContent, 500);
            }).appendTo($debug);

            $('<input></input>').attr('type', 'button').val("Eval").click(function (c) { evalContent(); }).appendTo($debug);
            $debug.append("<br />");

            var $codeArea = $("<textarea />").appendTo($debug);
            $debug.append("<br />");
            var $resulArea = $("<textarea />").appendTo($debug);

            $debug.appendTo($this);

            function evalContent() {
                try {
                    var result = eval($codeArea.val());
                    $resulArea.val(result);
                } catch (ex) {
                    $resulArea.val(ex);
                }
            }

            function trace() {
                var result = "", newline = "\r\n";
                for (i in arguments) {
                    result += BAUI.log.dump(arguments[i]) + newline;
                }
                return result;
            }
        });
    };
    $.fn.evaluator.defaults = {
    };
})(jQuery);
BAUI.debug = {
    Evaler: function () {
        BAUI.toolSet.cleanContainer();
        $("#Debug-template").tmpl({ title: "Evaluator" }).appendTo(BAUI.$Container)
            .append($("<div/>").attr("id", "evaluator"));
        $('#evaluator').evaluator();
    },
    EvalerWindow: function () {
        $.window({
            height: 390, x: 10, y: 10,
            dock: 'bottom',
            title: "Debug window",
            content: $("<div />")
        }).getFrame().evaluator();
    },
    ShowBoxGrid: function (name) {
        if (!name) name = $("body").data("hurl").link.target;
        if (!name) { alert("Error: no name"); return; }
        BAUI.toolSet.cleanContainer();
        $("#Debug-template").tmpl({ title: name.match(/^[\w\.]+/i)[0] }).appendTo(BAUI.$Container);
        var $container = $('#debug-container');

        var $list = $("<table />").attr("id", "grid-list").appendTo($container);
        var $pager = $("<div />").attr("id", "grid-pager").appendTo($container);
        var p = BAUI.grid.getBoxGridParameters($list, $pager, name);

        $list.jqGrid(p)
            .jqGrid('filterToolbar', {}).navGrid("#grid-pager",
		    { view: true, edit: false, search: false, add: false, del: false },
		    {}, /*edit*/{}, /*add*/{},  /*del*/
		    {multipleSearch: true }, /*search*/{closeOnEscape: true} /* view*/);
    },
    EntityList: function () {
        BAUI.toolSet.cleanContainer();
        $("#Debug-template").tmpl({ title: "Available tables" }).appendTo(BAUI.$Container);
        var $container = $('#debug-container');

        var arr = Enumerable.From($.parseJSON(window.external('grid').getAvailableMappings()))
            .Select(function (o) { return { Name: o.match(/^[\w\.]+/i)[0], Type: o }; }).ToArray();
        var $list2 = $("<table />").attr("id", "grid-list").appendTo($container);
        var p = BAUI.grid.getOnePageGridParameters($list2, arr);
        $.extend(p, {
            colNames: ['Name', 'Action'],
            colModel: [
                { index: 'Name', name: 'Name', sortable: false, width: 500 },
                { index: 'Name', name: '', sortable: false, width: 200, formatter: 'multiformat',
                    formatoptions: [
                        { format: 'hurl', options: { name: 'Show grid', action: 'eval', target: 'Type', data: 'BAUI.debug.ShowBoxGrid()'} },
                        { format: 'func', options: function (data) { return '<button onclick="BAUI.debug._backup(\'' + data.Type + '\')">Backup</button>' } }
                    ]
                }
            ]
        });
        $list2.jqGrid(p);

        /*
        datatype: function (postdata) {
        $list2[0].addJSONData(BAUI.grid._toOnePageGridData(arr));
        //act
        var ids = $mgrid.getDataIDs();
        for (var i = 0; i < ids.length; i++) {
        var row_id = ids[i];
        var name = $mgrid.getRowData(row_id).Name;
        $mgrid.setRowData(row_id, { act:
        '<input type="button" value="G" onclick="BAUI.backchannel.send(\'js_showMetricsGraph\',\'Taxon;' + arr[index].Id + ';' + name + '\')" />'
        });
        }*/

    },
    Taxonomy: function () {
        BAUI.toolSet.cleanContainer();
        $("#Debug-template").tmpl({ title: "Taxonomy" }).appendTo(BAUI.$Container);
        var $container = $('#debug-container');

        var $tree = $("<ul />").attr("id", "taxonomy");
        $("<div />").addClass("cell50").append($tree).appendTo($container);
        var $detail = $("<div />").attr("id", "detail").addClass("cell50").appendTo($container);

        $tree.treeview({ isDynamic: true, action: function (o) {
            var path = o.target.parentNode.id
            $detail.empty(); //$(">*", $detail).remove();
            var taxon = $.parseJSON(window.external('tree').getTaxon(path));
            BAUI.log.trace(taxon);
            //header
            $("<div />").html(taxon.text + " : " + taxon.description).appendTo($detail);
            //classifications
            var arr1 = Enumerable.From($.parseJSON(window.external('tree').getClassified(path)))
                .Select("$=>{Name:$.Description}").ToArray();
            var $list1 = $("<table />").attr("id", "grid-class_list").appendTo($detail);
            var p = BAUI.grid.getOnePageGridParameters($list1, arr1);
            $.extend(p, {
                colNames: ['Classifications'],
                colModel: [
                    { index: 'Name', name: 'Name', formatter: 'encode', sortable: false, width: 300 },
                ]
            });
            $list1.jqGrid(p);

            //relations
            var arr2 = Enumerable.From($.parseJSON(window.external('tree').getRelationsByType(path)))
                    .Select("$=>{Name:$.Description}").ToArray();
            var $list2 = $("<table />").attr("id", "grid-rel_list").appendTo($detail);
            var p = BAUI.grid.getOnePageGridParameters($list2, arr2);
            $.extend(p, {
                colNames: ['Relations by type'],
                colModel: [
                    { index: 'Name', name: 'Name', formatter: 'encode', sortable: false, width: 300 },
                ]
            });
            $list2.jqGrid(p);
        }
        });
    },
    _backup: function (type) {
        if (!window.external('backup').hasBackupFor(type)) {
            alert("Sorry no backup defined for:" + type);
            return;
        }
        if (!window.external('backup').backupDefault(type)) {
            return alert("Backup falsed");
            return;
        }
        return alert("Backup done");
    }

};
// :::: Evaler end
