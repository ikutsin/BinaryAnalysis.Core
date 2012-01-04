/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\jquery.validate-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\jquery-deps.js" />

// :::: commons
JSON = {
    stringify: function (obj) {
        var t = typeof (obj);
        if (t != "object" || obj === null) {
            // simple data type
            if (t == "string") obj = '"' + obj + '"';
            return String(obj);
        }
        else {
            // recurse array or object
            var n, v, json = [], arr = (obj && obj.constructor == Array);
            for (n in obj) {
                v = obj[n]; t = typeof (v);
                if (t == "string") v = '"' + v + '"';
                else if (t == "object" && v !== null) v = JSON.stringify(v);
                json.push((arr ? "" : '"' + n + '":') + String(v));
            }
            return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
        }
    }
}

// :::: commons end

BAUI = {};
// :::: logging start
if (window.external && window.external('logging')) {
    BAUI.log = {
        info: function (o) { window.external('logging').info(o); },
        warn: function (o) { window.external('logging').warn(o); },
        error: function (o) { window.external('logging').error(o); },
        debug: function (o) { window.external('logging').debug(o); }
    };
} else if (window.console) {
    BAUI.log = {
        info: function (o) { window.console.log("info: ", o); },
        warn: function (o) { window.console.log("warn: ", o); },
        error: function (o) { window.console.log("error: ", o); },
        debug: function (o) { window.console.log("debug: ", o); }
    };
} else {
    BAUI.log = {
        info: function (o) { window.alert("info: " + o); },
        warn: function (o) { window.alert("warn: " + o); },
        error: function (o) { window.alert("error: " + o); },
        debug: function (o) { window.alert("debug: " + o); }
    };
    BAUI.log.error("Logging not found");
}
BAUI.log.trace = function (arr, level) {
    BAUI.log.debug(BAUI.log.dump(arr, level));
}
BAUI.log.dump = function (arr, level, depth) {
    var dumped_text = "";
    var newline = "\r\n";
    if (!depth && depth != 0) depth = 3;
    if (!level) level = 0;

    if (depth == 0) return dumped_text;

    //The padding given at the beginning of the line.
    var level_padding = "";
    for (var j = 0; j < level + 1; j++) level_padding += "    ";

    if (typeof (arr) == 'object') { //Array/Hashes/Objects 
        for (var item in arr) {
            try {
                var value = arr[item];
                if (typeof (value) == 'object') { //If it is an array,
                    dumped_text += level_padding + "'" + item + "' (type:" + typeof (value) + ") " + newline;
                    dumped_text += BAUI.log.dump(value, level + 1, depth - 1);
                } else if (typeof (value) == 'function') { //If it is an function,
                    dumped_text += level_padding + "'" + item + "' (function)" + newline;
                } else {
                    dumped_text += level_padding + "'" + item + "' => \"" + value + "\"" + newline;
                }
            } catch (ex) {
                dumped_text += level_padding + "'" + item + "' (ex:" + ex + ") " + newline;
            }
        }
    } else { //Stings/Chars/Numbers etc.
        dumped_text = "===>" + arr + "<===(" + typeof (arr) + ")";
    }
    return dumped_text;
}
if (!window.console) {
    console = {
        log: function () { BAUI.log.trace(arguments[2]); }
    };
}
// :::: logging end

BAUI.$Container = {}
BAUI.toolSet = {
    loadAndShowTemplate: function (name, opts, callback) {
        $.deps.load("Templates/" + name + ".htm", function () {
            BAUI.toolSet.cleanContainer();
            $("#" + name + "-template").tmpl(opts).appendTo(BAUI.$Container);
        });
    },
    cleanContainer: function () {
        //$(">*", BAUI.$Container).remove();
        BAUI.$Container.empty();
    },
    isEmptyObject: function (obj) { //http://neue.cc/reference.htm
        for (var prop in obj) {
            //if (Object.prototype.hasOwnProperty.call(obj, prop)) {
            return false;
            //}
        }
        return true;
    },
    escape: function (val) {
        return $('<div/>').text(val ? String(val) : "null").html();
    }
}

BAUI.actions = {
    TextDialog: function (text) {
        if (!text) text = unescape($("body").data("hurl").link.target);
        if (!text) { alert("Error: no text"); return; }
        window.external('core').showTextDialog(text);
    },
    About: function () {
        $.deps.load("Templates/About.htm", function () {
            var opts = {
                "userAgent": navigator.userAgent,
                "platform": navigator.platform,
                "cookieEnabled": navigator.cookieEnabled,
                "appVersion": navigator.appVersion,
                "platformappName": navigator.appName,
                "appCodeName": navigator.appCodeName,
                "Modernizr": Modernizr
            };
            BAUI.toolSet.cleanContainer();
            $("#About-template").tmpl(opts).appendTo(BAUI.$Container);
            $("#content").spoiler({ title: "Browser info" });
        });
    }
}

// :::: navigation
BAUI.navigation = {
    init: function () {
        $.hurl();
        $("body").bind('hurl', function (event) {
            var link = $("body").data("hurl").link;
            BAUI.log.trace(link);
            BAUI.log.debug("--Nav :");
            //todo: save last page for further start
            if (link.action == 'eval' && link.data !== undefined) {
                $.globalEval(link.data + ";");
            }
        });
        if ($("body").data("hurl").link.action !== undefined) {
            $.hurl("update", $("body").data("hurl").link);
        } else {
            //TODO: default location
        }
        this.buildMenu("#menu");
    },
    menuAction: function (str) {
        try {
            $.hurl("update", $.parseJSON(str));
        } catch (ex) {
            BAUI.log.error("menuAction:" + ex.message);
        }
    },
    buildMenu: function (selector) {
        var $menu = $("<ul />").addClass("sf-menu");
        var buildMenu = function ($elem, itemsArray) {
            for (i in itemsArray) {
                var item = itemsArray[i];
                var $a = $("<a/>").html(item.name);
                var $li = $('<li/>').append($a).appendTo($elem);
                if (item.action) $a.click((function (ia) { return function () { BAUI.navigation.menuAction(ia); } })(item.action));
                if (item.menuitems !== undefined) {
                    var $m = $("<ul />").appendTo($li);
                    buildMenu($m, item.menuitems);
                }
            }
        }
        buildMenu($menu, $.parseJSON(window.external('core').getMenu()));
        $menu.appendTo($(selector)).superfish();
    }
}
// :::: navigation end


//bootsrap
$(document).ready(function () {
    $.deps.init('', $.parseJSON(window.external('core').getDependencies()));
    $.deps.load("Templates/Navigation.htm", function () {
        $("#loadingMessage").fadeOut("slow", function () {
            BAUI.$Container = $("#container");
            BAUI.$Container.html($("#Navigation-template").tmpl().html());
            BAUI.navigation.init();
        });
    });
});
