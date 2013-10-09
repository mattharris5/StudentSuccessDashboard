/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PublicFieldDataTable = (function (_super) {
    __extends(PublicFieldDataTable, _super);
    function PublicFieldDataTable() {
        _super.call(this, $('#publicFields'));
    }
    PublicFieldDataTable.prototype.GetServerDataUrl = function () {
        return "/CustomFields/Public/DataTableAjaxHandler";
    };

    PublicFieldDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2];
    };

    PublicFieldDataTable.prototype.CreateColumns = function () {
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

    PublicFieldDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(3, !DataTable.IsInPrintExportMode);
    };

    PublicFieldDataTable.prototype.SetServerParameters = function (aoData) {
    };

    PublicFieldDataTable.prototype.RenderCategories = function (data, type, full) {
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

    PublicFieldDataTable.prototype.RenderActions = function (data, type, full) {
        var buttonOutput = '<ul class="buttons clearfix"><li>' + '<a class ="btn btn-primary btn-mini editPublicFields" role="button" data-value="{0}"><i class ="icon-edit"></i>Edit</a>' + '</li><li>' + '<a class="btn btn-danger btn-mini deletePublicFields" role="button" data-value="{0}"><i class ="icon-remove"></i>Delete</a>' + '</li></ul>';
        return StringExtensions.format(buttonOutput, data);
    };
    return PublicFieldDataTable;
})(DataTable);
//# sourceMappingURL=PublicFieldDataTable.js.map
