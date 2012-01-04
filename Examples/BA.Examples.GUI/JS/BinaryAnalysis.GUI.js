/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Grid.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Form.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\Redist\jquery.tmpl.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.validate-vsdoc.js" />

BAUI.test = {
    Form: function () {
        BAUI.toolSet.cleanContainer();
        $("#FormTest-template").tmpl().appendTo(BAUI.$Container);
        BAUI.form.addSubmitHandler('#myform');
    }
}