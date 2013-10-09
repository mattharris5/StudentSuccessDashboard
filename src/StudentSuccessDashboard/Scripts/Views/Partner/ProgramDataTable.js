/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PartnerFilter.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ProgramDataTable = (function (_super) {
    __extends(ProgramDataTable, _super);
    function ProgramDataTable() {
        _super.call(this, $('#programs'));
    }
    ProgramDataTable.prototype.GetServerDataUrl = function () {
        return "/Partners/Program/DataTableAjaxHandler";
    };

    ProgramDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2, 3];
    };

    ProgramDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "Name"
            },
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "Contact",
                "mRender": this.RenderContact
            },
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "Schools",
                "mRender": this.RenderList
            },
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "Providers",
                "mRender": this.RenderList
            },
            {
                "bSortable": false,
                "bSearchable": false,
                "mData": "ServiceTypes",
                "mRender": this.RenderList
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

    ProgramDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(5, !DataTable.IsInPrintExportMode);
        if (DataTable.IsInPrintExportMode) {
            $('#ProgramFilterHeader').show();
            var providerName = $('#autocompletePartnerName').val();
            var filterHeaderHtml = "";
            if (providerName.length > 0) {
                filterHeaderHtml += "<h3>Provider Name: " + providerName + "</h3>";
            }
            $('#ProgramFilterHeader').html(filterHeaderHtml);
        } else {
            $('#ProgramFilterHeader').hide();
        }
    };

    ProgramDataTable.prototype.RenderContact = function (data, type, full) {
        return StringExtensions.format("Contact Name: {0}<br/>Phone: {1}<br/>Email: <a href='mailto:{2}'>{2}</a>", data.Name, data.Phone, data.Email);
    };

    ProgramDataTable.prototype.RenderList = function (data, type, full) {
        var valList = "";
        if (data.length > 0) {
            valList = "<ul>";
            for (var i = 0; i < data.length; i++) {
                if (data[i].length > 0) {
                    valList += StringExtensions.format("<li>{0}</li>", data[i]);
                }
            }
            valList += "</ul>";
        }
        return valList;
    };

    ProgramDataTable.prototype.RenderActions = function (data, type, full) {
        var actionButtons = "<ul class='buttons clearfix'>" + "<li><a class='btn btn-primary btn-mini editProgram' role='button' data-toggle='modal' data-value='{0}'><i class='icon-edit'></i> Edit Program Details</a></li>" + "<li><a class='btn btn-danger btn-mini deleteProgram' data-value='{0}'><i class='icon-remove'></i> Delete</a></li></ul>";
        var actionButtonsMarkup = StringExtensions.format(actionButtons, data);
        return actionButtonsMarkup;
    };
    return ProgramDataTable;
})(DataTable);
//# sourceMappingURL=ProgramDataTable.js.map
