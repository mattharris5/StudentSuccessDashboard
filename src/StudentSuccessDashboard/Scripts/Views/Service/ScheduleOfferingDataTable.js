/// <reference path="../DataTable.ts" />
/// <reference path="ScheduleOfferingFilter.ts" />
/// <reference path="../StringExtensions.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ScheduleOfferingDataTable = (function (_super) {
    __extends(ScheduleOfferingDataTable, _super);
    function ScheduleOfferingDataTable() {
        _super.call(this, $('#serviceOfferingOptions'));
    }
    ScheduleOfferingDataTable.prototype.GetServerDataUrl = function () {
        return "/ServiceOffering/DataTableAjaxHandler";
    };

    ScheduleOfferingDataTable.prototype.GetPrintExportColumnIndices = function () {
        return null;
    };

    ScheduleOfferingDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "mData": "IsFavorite",
                "bSortable": false,
                "bSearchable": false,
                "mRender": this.RenderFavorites
            },
            {
                "mData": "IsPrivate",
                "bSortable": false,
                "bSearchable": false,
                "mRender": this.RenderPrivate
            },
            {
                "mData": "ServiceType"
            },
            {
                "mData": "Provider"
            },
            {
                "mData": "Program"
            },
            {
                "mData": "Id",
                "bSortable": false,
                "bSearchable": false,
                "mRender": this.RenderActions
            }
        ];
        return columns;
    };

    ScheduleOfferingDataTable.prototype.DrawCallback = function (oSettings) {
    };

    ScheduleOfferingDataTable.prototype.RenderFavorites = function (data, type, full) {
        if (full.CanInteract) {
            if (DataTable.IsInPrintExportMode) {
                return data;
            }
            if (data) {
                return StringExtensions.format("<input name='IsFavorite' class='favorite-checkbox' type='checkbox' checked='CHECKED' data-value='{0}' />", full.Id);
            }
            return StringExtensions.format("<input name='IsFavorite' class='favorite-checkbox' type='checkbox' data-value='{0}' />", full.Id);
        }
        return "";
    };

    ScheduleOfferingDataTable.prototype.RenderPrivate = function (data, type, full) {
        return (full.IsPrivate) ? '<i class="icon-ok"></i>' : "";
    };

    ScheduleOfferingDataTable.prototype.RenderActions = function (data, type, full) {
        if (full.CanInteract) {
            var serviceOfferingId = full.Id;
            if (serviceOfferingId != null) {
                var buttons = "<ul class=\"buttons clearfix\">" + "<li><a class='btn btn-primary btn-mini scheduleOffering' role='button' data-toggle='modal' data-value='{0}'><i class='icon-plus'></i> Add This Service Offering</a></li></ul>";
                return StringExtensions.format(buttons, serviceOfferingId);
            }
        }
        return "";
    };
    return ScheduleOfferingDataTable;
})(DataTable);
//# sourceMappingURL=ScheduleOfferingDataTable.js.map
