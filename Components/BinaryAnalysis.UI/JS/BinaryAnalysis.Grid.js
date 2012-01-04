/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\jquery.tmpl.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\jquery-showhide.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\modernizr.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\Superfish.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\jquery.blockui.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\dateFormat.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />

if (!BAUI) BAUI = {};

// --- FORMATTERS
$.extend($.fn.fmatter, {
    raw: function (cellvalue, options, rowdata) {
        return cellvalue;
    },
    //name, target, +hurl(action: 'eval', data: 'BAUI.actions.TextDialog()')
    hurl: function (cellvalue, options, rowdata) {
        var params = {}
        if (typeof (options.colModel.formatoptions.target) == "function") {
            $.extend(params, options.colModel.formatoptions, { target: options.colModel.formatoptions.target(rowdata) });
        } else {
            $.extend(params, options.colModel.formatoptions, { target: rowdata[options.colModel.formatoptions.target] });
        }
        var evalutaion = '$.hurl("update", ' + JSON.stringify(params) + ');';
        return '<button onclick=\'' + evalutaion + '\'>' + options.colModel.formatoptions.name + '</button>';
    },
    func: function (cellvalue, options, rowdata) {
        //BAUI.log.trace(options.colModel.formatoptions);
        return options.colModel.formatoptions(rowdata, cellvalue, options)
    },
    multiformat: function (cellvalue, options, rowdata) {
        var result = "";
        for (var i in options.colModel.formatoptions) {
            var params = { colModel: { formatoptions: options.colModel.formatoptions[i].options} };
            result += $.fn.fmatter[options.colModel.formatoptions[i].format](cellvalue, params, rowdata);
        }
        return result;
    },
    encode: function (cellvalue, options, rowdata) {
        return BAUI.toolSet.escape(cellvalue ? String(cellvalue) : null);
    },
    date: function (cellvalue, options, rowdata) {
        var date = new Date(parseInt(cellvalue.substr(6)));
        try {
            return date.format('yyyy-mm-dd hh:MM');
        } catch (ex) { return '-'; }
    },
    dateInt: function (cellvalue, options, rowdata) {
        return parseInt(cellvalue.substr(6));
    },
    eval: function (cellvalue, options, rowdata) {
        if (!cellvalue) return "-undefined-";
        return eval(cellvalue.substring(1, cellvalue.length - 1));
    },
    array: function (cellvalue, options, rowdata) {
        return '<span class="sparkleArray">' + cellvalue + '</span>';
    },
    enumer: function (cellvalue, options, rowdata) {
        var enumName = options.colModel.formatoptions.name;
        return BAUI.grid.getEnumValue(enumName, cellvalue);
    },
    variant: function (cellvalue, options, rowdata) {
        if (!cellvalue) return "-";
        if (typeof (cellvalue) == 'number') {
            if (cellvalue > 621355968000000000) {
                var date = new Date((cellvalue - 621355968000000000) / 10000);
                try {
                    return date.format('yyyy-mm-dd hh:MM');
                } catch (ex) { return '-'; }
            }
            return cellvalue;
        }
        return cellvalue;
    }
});
// --- FORMATTERS end

// :::: context start
BAUI.grid = {
    _queryPostData: function (boxType, postdata) {
        var en = Enumerable.From(postdata) //http://neue.cc/reference.htm
			.Where(function (o) { return o.Key.charAt(0) != '_' && o.Key.charAt(0) == o.Key.charAt(0).toUpperCase(); })
			.Select("k=>k.Key+'='+k.Value").ToString("|")
        var result = window.external('grid').getEntries(boxType, postdata.rows, postdata.page, postdata.sidx,
		postdata.sord == 'asc', postdata._search, en);
        return $.parseJSON(result);
    },
    _jsonReader: { root: "rows", page: "page", total: "total", records: "records", repeatitems: false },
    _toOnePageGridData: function (arr) { return { page: 1, total: arr.length, records: arr.length, rows: arr} },
    getOnePageGridParameters: function (gridSelector, arr, datatypeCallback) {
        return {
            datatype: function (postdata) {
                $(gridSelector)[0].addJSONData(BAUI.grid._toOnePageGridData(arr));
                if (datatypeCallback) datatypeCallback();
            },
            rowNum: -1,
            jsonReader: BAUI.grid._jsonReader,
            viewrecords: true,
            height: "100%"
            //subgrid: true,
            //caption: "Cation"
        };
    },
    getBoxGridParameters: function (gridSelector, pagerSelector, boxName, datatypeCallback) {
        return {
            datatype: function (postdata) {
                $(gridSelector)[0].addJSONData(BAUI.grid._queryPostData(boxName, postdata));
                if (datatypeCallback) datatypeCallback();
            },
            rowNum: 10,
            rowList: [10, 20, 30],
            jsonReader: BAUI.grid._jsonReader,
            viewrecords: true,
            height: "100%",
            pager: $(pagerSelector)[0],
            colNames: $.parseJSON(window.external('grid').getColNames(boxName)),
            colModel: $.parseJSON(window.external('grid').getColModel(boxName))
        };
    },
    setEditableInline: function (selector, ps) {
        $.extend(ps, {
            onSelectRow: function (id) {
                var lastSel = $(selector).data("lastSel");
                if (id && id !== lastSel) {
                    $(selector).saveRow(lastSel, true, true, 'clientArray');
                    $(selector).data("lastSel", id);
                }
                $(selector).editRow(id, true, null, null, 'clientArray');
            }
        });
    },
    getEnumValue: function (enumName, cellvalue) { return window.external('grid').getEnumValue(enumName, cellvalue) },
    formatter: $.fn.fmatter,
    //BAUI.context.getModelFormat = function (name, type, opts) { return $.extend($.parseJSON(window.external('db').getModelFormat(name, type)), opts); };
    _applyCustomGridValues: function ($mgrid) {
        $(".sparkleArray", $mgrid[0]).each(function () {
            var $this = $(this);
            $this.sparkline(eval("[0," + $this.text() + "]"), { type: 'line', barColor: 'green' });
        });
    }
};


// :::: context end
