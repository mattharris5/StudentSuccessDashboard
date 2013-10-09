/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ServiceOfferingFileUploadCompletePage = (function (_super) {
    __extends(ServiceOfferingFileUploadCompletePage, _super);
    function ServiceOfferingFileUploadCompletePage() {
        _super.call(this);
    }
    ServiceOfferingFileUploadCompletePage.prototype.Initialize = function () {
        this.SetupControls();
    };

    ServiceOfferingFileUploadCompletePage.prototype.SetupControls = function () {
        $('#redirectToServiceOffering').on('click', function () {
            window.location.href = '/ServiceOffering';
        });
    };
    return ServiceOfferingFileUploadCompletePage;
})(ManagePage);

$(document).ready(function () {
    new ServiceOfferingFileUploadCompletePage().Initialize();
});
//# sourceMappingURL=FileUploadComplete.js.map
