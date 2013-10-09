/// <reference path="AccessAudit.ts" />
/// <reference path="../DataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var AccessAuditDataTable = (function (_super) {
    __extends(AccessAuditDataTable, _super);
    function AccessAuditDataTable() {
        _super.call(this, $("#accessAudit"));
    }
    AccessAuditDataTable.prototype.GetServerDataUrl = function () {
        return "/User/AccessAuditDataTableAjaxHandler";
    };

    AccessAuditDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2, 3];
    };

    AccessAuditDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "mData": "CreatingUser",
                "bSearchable": false
            },
            {
                "mData": "CreateTime",
                "mRender": this.RenderCreateTime,
                "bSearchable": false
            },
            {
                "mData": "AccessData",
                "mRender": this.RenderRoles,
                "bSearchable": false
            },
            {
                "mData": "AccessData",
                "mRender": this.RenderAssociations,
                "bSearchable": false
            },
            {
                "mData": "UserActive",
                "mRender": this.RenderActive,
                "bSearchable": false
            }
        ];
        return columns;
    };

    AccessAuditDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
    };

    AccessAuditDataTable.prototype.SetServerParameters = function (aoData) {
        aoData.push({ "name": 'id', "value": $("#Id").val() });
    };

    AccessAuditDataTable.prototype.RenderCreateTime = function (data) {
        return StringExtensions.parseJsonDateTime(data).toLocaleString();
    };

    AccessAuditDataTable.prototype.RenderRoles = function (data) {
        var parser = new DOMParser();
        var xmlDoc = parser.parseFromString(data, "text/xml");
        var html = "";
        var x = xmlDoc.getElementsByTagName("roles");
        for (var i = 0; i < x.length; i++) {
            html += x[i].childNodes[0].attributes["name"].value;
        }
        return html;
    };

    AccessAuditDataTable.prototype.RenderAssociations = function (data) {
        var parser = new DOMParser();
        var xmlDoc = parser.parseFromString(data, "text/xml");
        var x = xmlDoc.getElementsByTagName("providers");
        AccessAuditPage.accessDataArray.push(data.toString());
        if (x[0].childNodes[0] != undefined) {
            return "<a class='btn btn-mini user-associations associations' data-toggle='modal' role='button' data-value='" + (AccessAuditPage.accessDataArray.length - 1) + "'>" + x[0].childNodes.length + "</a>";
        }
        x = xmlDoc.getElementsByTagName("schools");
        if (x[0].childNodes[0] != undefined) {
            return "<a class='btn btn-mini user-associations associations' data-toggle='modal' role='button' data-value='" + (AccessAuditPage.accessDataArray.length - 1) + "'>" + x[0].childNodes.length + "</a>";
        }
        return "";
    };

    AccessAuditDataTable.prototype.RenderActive = function (data) {
        return (data == true) ? '<i class="icon-ok"></i>' : "";
    };
    return AccessAuditDataTable;
})(DataTable);
//# sourceMappingURL=AccessAuditDataTable.js.map
