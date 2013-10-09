/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PrivateHealthFieldDataTable = (function (_super) {
    __extends(PrivateHealthFieldDataTable, _super);
    function PrivateHealthFieldDataTable() {
        _super.call(this, $('#privateHealthFields'));
    }
    PrivateHealthFieldDataTable.prototype.GetServerDataUrl = function () {
        return "/CustomFields/PrivateHealth/DataTableAjaxHandler";
    };

    PrivateHealthFieldDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2];
    };

    PrivateHealthFieldDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "bSearchable": false,
                "mData": "Name"
            },
            {
                "bSearchable": false,
                "mData": "Type"
            },
            {
                "bSearchable": false,
                "mData": "Provider"
            },
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "Categories",
                "mRender": this.RenderCategories
            },
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "Id",
                "mRender": this.RenderActions
            }
        ];
        return columns;
    };

    PrivateHealthFieldDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(3, !DataTable.IsInPrintExportMode);
    };

    PrivateHealthFieldDataTable.prototype.SetServerParameters = function (aoData) {
    };

    PrivateHealthFieldDataTable.prototype.RenderCategories = function (data, type, full) {
        var valList = "";
        if (data.length > 0) {
            valList = "<ul>";
            for (var i = 0; i < data.length; i++) {
                valList = valList + StringExtensions.format("<li>{0}</li>", data[i]);
            }
            valList = valList + "</ul>";
        }
        return valList;
    };

    PrivateHealthFieldDataTable.prototype.RenderActions = function (data, type, full) {
        var buttonOutput = '<ul class="buttons clearfix"><li>' + '<a class ="btn btn-primary btn-mini editPrivateHealthFields" role="button" data-value="{0}"><i class ="icon-edit"></i>Edit</a>' + '</li><li>' + '<a class="btn btn-danger btn-mini deletePrivateHealthFields" role="button" data-value="{0}"><i class ="icon-remove"></i>Delete</a>' + '</li></ul>';
        return StringExtensions.format(buttonOutput, data);
    };
    return PrivateHealthFieldDataTable;
})(DataTable);
//# sourceMappingURL=PrivateHealthFieldDataTable.js.map
