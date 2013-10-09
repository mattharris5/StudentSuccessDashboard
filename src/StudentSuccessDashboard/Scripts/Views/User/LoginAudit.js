/// <reference path="../ssd.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="LoginAuditDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var LoginAuditPage = (function (_super) {
    __extends(LoginAuditPage, _super);
    function LoginAuditPage() {
        _super.call(this);
        this._LoginAuditDataTable = new LoginAuditDataTable();
    }
    LoginAuditPage.prototype.Initialize = function () {
        this._LoginAuditDataTable.Initialize(null);
    };
    LoginAuditPage.accessDataArray = new Array();
    return LoginAuditPage;
})(ManagePage);

$(document).ready(function () {
    new LoginAuditPage().Initialize();
});
//# sourceMappingURL=LoginAudit.js.map
