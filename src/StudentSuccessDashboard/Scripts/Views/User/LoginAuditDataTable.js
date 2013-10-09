/// <reference path="../StringExtensions.ts" />
/// <reference path="../DataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var LoginAuditDataTable = (function (_super) {
    __extends(LoginAuditDataTable, _super);
    function LoginAuditDataTable() {
        _super.call(this, $("#loginAudit"));
    }
    LoginAuditDataTable.prototype.GetServerDataUrl = function () {
        return "/User/LoginAuditDataTableAjaxHandler";
    };

    LoginAuditDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0];
    };

    LoginAuditDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "mData": "CreateTime",
                "mRender": this.RenderCreateTime,
                "bSearchable": false
            }
        ];
        return columns;
    };

    LoginAuditDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
    };

    LoginAuditDataTable.prototype.SetServerParameters = function (aoData) {
        aoData.push({ "name": 'id', "value": $("#Id").val() });
    };

    LoginAuditDataTable.prototype.RenderCreateTime = function (data) {
        return StringExtensions.parseJsonDateTime(data).toLocaleString();
    };
    return LoginAuditDataTable;
})(DataTable);
//# sourceMappingURL=LoginAuditDataTable.js.map
