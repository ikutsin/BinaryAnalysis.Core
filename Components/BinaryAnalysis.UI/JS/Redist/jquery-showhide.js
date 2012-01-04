/*
jquery-showhide
A more robust replacement for jQuery's .toggle() function.
Built by Dana Woodman.
*/
(function ($) {

    $.fn.spoiler = function (options) {
        var opts = $.extend($.fn.spoiler.defaults, options);
        return this.each(function () { //logic with this goes here
            var $this = $(this);
            var $target = $this.children().first();

            $this.addClass("spoiler");

            var $minus = $('<div/>').addClass("ui-state-default ui-corner-all").css("cursor", "hand");
            $minus.append($('<span/>').css("float", "left").addClass("ui-icon ui-icon-minus"));
            $minus.append(opts.title);

            var $plus = $('<div/>').addClass("ui-state-default ui-corner-all").css("cursor", "hand");
            $plus.append($('<span/>').css("float", "left").addClass("ui-icon ui-icon-plus"));
            $plus.append(opts.title);

            var $trigger = $("<div/>");
            if (opts.default_open) $trigger.prepend($minus);
            else $trigger.prepend($plus);
            $this.prepend($trigger);

            $trigger.showhide({ target_obj: $target, default_open: opts.default_open });
            $trigger.bind("onHide", function () { $trigger.empty().append($plus); });
            $trigger.bind("onShow", function () { $trigger.empty().append($minus); });
        });
    };

    $.fn.spoiler.defaults = {
        title: "No name",
        default_open: true
    };


    $.fn.showhide = function (options) {

        // Compile default options and user specified options.
        var opts = $.extend({}, $.fn.showhide.defaults, options);

        return $(this).each(function () {

            var obj = $(this);

            // Build element specific options.
            obj.o = $.meta ? $.extend({}, opts, $this.data()) : opts;

            // If there is a custom target object, use that, if not, use ".next()"
            if (obj.o.target_obj) {
                obj.o.target = eval(obj.o.target_obj); // Turns the string into executable JavaScript code.
            } else {
                obj.o.target = obj.next();
            };

            // Set expiration of cookie, if it exists.
            if (obj.o.cookie_expires) {
                obj.o.cookie_options = { expires: obj.o.cookie_expires };
            }

            // Show function.
            show = function (object) {
                object.removeClass(object.o.plus_class);
                object.addClass(object.o.minus_class);
                if (object.o.minus_text) {
                    object.text(object.o.minus_text);
                };
                object.o.target.removeClass(object.o.hide_class);
                object.o.target.addClass(object.o.show_class);
                if (object.o.use_cookie) {
                    $.cookie(object.o.cookie_name, 'visible', object.o.cookie_options);
                };
                if (obj.o.focus_target) {
                    obj.o.focus_target.focus();
                };
                object.trigger("onShow");
            };

            // Hide function.
            hide = function (object) {
                object.removeClass(object.o.minus_class);
                object.addClass(object.o.plus_class);
                if (object.o.plus_text) {
                    object.text(object.o.plus_text);
                };
                object.o.target.removeClass(object.o.show_class);
                object.o.target.addClass(object.o.hide_class);
                if (object.o.use_cookie) {
                    $.cookie(object.o.cookie_name, 'hidden', object.o.cookie_options);
                };
                object.trigger("onHide");
            };

            // Using cookies.
            if (obj.o.use_cookie) {

                // Check for cookie. If set, set default state.
                if ($.cookie(obj.o.cookie_name)) {
                    if ($.cookie(obj.o.cookie_name) == 'visible') {
                        show(obj);
                    }
                    else if ($.cookie(obj.o.cookie_name) == 'hidden') {
                        hide(obj);
                    }

                    // Shouldn't get here. If it does, set value to default.
                    else {
                        $.cookie(obj.o.cookie_name, obj.o.cookie_value);
                        if ($.cookie(obj.o.cookie_name) == 'visible') {
                            show(obj);

                        }
                        else if ($.cookie(obj.o.cookie_name) == 'hidden') {
                            hide(obj);

                        }
                    };
                }

                // If no cookie, set default state by setting.
                else {
                    $.cookie(obj.o.cookie_name, obj.o.cookie_value);
                    if (obj.o.default_open) {
                        show(obj);
                    }
                    else {
                        hide(obj);
                    }
                }

                // Handle clicks on toggle object.
                obj.click(function () {
                    if ($.cookie(obj.o.cookie_name) == 'visible') {
                        hide(obj);
                        return false;
                    }
                    else {
                        show(obj);
                        return false;
                    };
                });
            }

            // Not using cookies.
            else {
                if (obj.o.default_open) { show(obj); }
                else { hide(obj); }

                // Handle clicks on toggle object.
                obj.click(function () {
                    if (obj.o.target.hasClass(obj.o.hide_class)) {
                        show(obj);
                        return false;
                    }
                    else if (obj.o.target.hasClass(obj.o.show_class)) {
                        hide(obj);
                        return false;
                    };
                });
            };

        });
    };

    $.fn.showhide.defaults = {
        target_obj: null, // An optional string of a jQuery object that will be show/hidden.
        focus_target: null, // An optional target jQuery object to focus text onto when showing the hidden element.
        default_open: true, // If true, the target will be show, if false it will be hidden by default. This is overridden by the cookie, if it is set.
        show_class: 'showhide-show', // Class name to get applied to the link that shows the target.
        hide_class: 'showhide-hide', // Class name to get applied to the link that hides the target.
        plus_class: 'showhide-plus', // Class name to get applied to the link that indicates it is expandable.
        plus_text: null,
        minus_class: 'showhide-minus', // Class name to get applied to the link that indicates it is contractable.
        minus_text: null,
        use_cookie: false, // If true, use a cookie to store the last chosen state. If false, do not use a cookie.
        cookie_expires: 365, // Number of days until cookie expires
        cookie_name: 'show_target' // A name for the cookie, this must be unique.
    };
})(jQuery);
