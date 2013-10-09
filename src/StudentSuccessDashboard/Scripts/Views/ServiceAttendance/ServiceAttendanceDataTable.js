/// <reference path="../DataTable.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ServiceAttendanceDataTable = (function (_super) {
    __extends(ServiceAttendanceDataTable, _super);
    function ServiceAttendanceDataTable() {
        _super.call(this, $('#serviceAttendances'));
    }
    ServiceAttendanceDataTable.prototype.GetServerDataUrl = function () {
        return "/ServiceAttendance/DataTableAjaxHandler";
    };

    ServiceAttendanceDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2, 3];
    };

    ServiceAttendanceDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "mData": "DateAttended",
                "mRender": this.RenderDateAttended,
                "bSearchable": false
            },
            {
                "mData": "Subject",
                "bSearchable": false
            },
            {
                "mData": "Duration",
                "bSearchable": false
            },
            {
                "mData": "Notes",
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Id",
                "mRender": this.RenderActions,
                "bSortable": false,
                "bSearchable": false
            }
        ];
        return columns;
    };

    ServiceAttendanceDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(3, !DataTable.IsInPrintExportMode);
    };

    ServiceAttendanceDataTable.prototype.SetServerParameters = function (aoData) {
        aoData.push({ "name": 'id', "value": $("#StudentAssignedOfferingId").val() });
    };

    ServiceAttendanceDataTable.prototype.RenderDateAttended = function (data, type, full) {
        return StringExtensions.parseJsonDateTime(data).toLocaleString();
    };

    ServiceAttendanceDataTable.prototype.RenderActions = function (data, type, full) {
        var actionButtons = "<ul class='buttons clearfix'><li><a class='btn btn-primary btn-mini editServiceAttendance' data-toggle='modal' role='button' data-value='{0}'>" + "<i class='icon-edit'></i> Edit</a></li>" + "<li><a class='btn btn-danger btn-mini deleteServiceAttendance' data-value='{0}'><i class='icon-remove'></i> Delete</a></li></ul>";
        return StringExtensions.format(actionButtons, data);
        return actionButtons;
    };
    return ServiceAttendanceDataTable;
})(DataTable);
//# sourceMappingURL=ServiceAttendanceDataTable.js.map
