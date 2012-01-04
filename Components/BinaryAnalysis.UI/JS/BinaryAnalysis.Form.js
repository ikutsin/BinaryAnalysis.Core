/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\jquery.tmpl.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\jquery.deserialize.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\jquery-showhide.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\modernizr.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\Superfish.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\jquery.blockui.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\Redist\dateFormat.js" />
/// <reference path="..\..\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />

// :::: context start
BAUI.form = {
    addSubmitHandler: function (selector, callback) {
        $(selector).validate({
            submitHandler: function (form) {
                callback(form, $(form).serialize())
                return false;
            }
        });
    },
    deleteConfirm: function (typeName, id, onSuccess) {
        var con = window.confirm("Are you sure to delete this element?");
        if (con) {
            window.external('form').deleteEntity(id, typeName);
            if (onSuccess) onSuccess();
        }
    },
    addEdit: function (selector, typeName, id, onSuccess) {
        var entity = window.external('form').getEntity(id, typeName);
        $(selector).unserializeForm(entity);
        BAUI.form.addNew(selector, typeName, onSuccess);
    },
    addNew: function (selector, typeName, onSuccess) {
        BAUI.form.addSubmitHandler(selector, function (f, fd) {
            window.external('form').saveEntity(fd, typeName);
            if (onSuccess) onSuccess(f);
        });
    }
};
// :::: context end

