/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery-1.5.1-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\jquery.linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\vsdoc\linq-vsdoc.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Bootstrap.js" />
/// <reference path="..\..\..\Components\BinaryAnalysis.UI\JS\BinaryAnalysis.Grid.js" />

BAUI.scriptingHelper = {
    startStandalone: function () {
        window.external("scriptingHelper").startStandalone();
    }
}

