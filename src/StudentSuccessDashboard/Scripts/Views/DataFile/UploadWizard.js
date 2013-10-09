/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var UploadWizardPage = (function (_super) {
    __extends(UploadWizardPage, _super);
    function UploadWizardPage() {
        _super.call(this);
    }
    UploadWizardPage.prototype.Initialize = function () {
        this.SetupControls();
    };

    UploadWizardPage.prototype.SetupControls = function () {
        var setupTooltips = $("[rel=tooltip]");
        setupTooltips.tooltip();
    };
    return UploadWizardPage;
})(ManagePage);
//# sourceMappingURL=UploadWizard.js.map
