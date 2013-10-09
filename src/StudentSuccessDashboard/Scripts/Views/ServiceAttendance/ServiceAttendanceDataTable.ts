/// <reference path="../DataTable.ts" />
/// <reference path="../ssd.ts" />

class ServiceAttendanceDataTable extends DataTable {
    constructor() {
        super($('#serviceAttendances'));
    }

    GetServerDataUrl(): string {
        return "/ServiceAttendance/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0, 1, 2, 3];
    }

    CreateColumns(): any[]{
        var columns: any[] = [
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
    }

    DrawCallback(oSettings: any): void {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(3, !DataTable.IsInPrintExportMode);
    }

    SetServerParameters(aoData: any[]): void {
        aoData.push({ "name": 'id', "value": $("#StudentAssignedOfferingId").val() });
    }

    RenderDateAttended(data: any, type: string, full: any): string {
        return StringExtensions.parseJsonDateTime(data).toLocaleString();
    }

    private RenderActions(data: any, type: string, full: any): string {
        var actionButtons = "<ul class='buttons clearfix'><li><a class='btn btn-primary btn-mini editServiceAttendance' data-toggle='modal' role='button' data-value='{0}'>" +
                            "<i class='icon-edit'></i> Edit</a></li>" +
                            "<li><a class='btn btn-danger btn-mini deleteServiceAttendance' data-value='{0}'><i class='icon-remove'></i> Delete</a></li></ul>";
        return StringExtensions.format(actionButtons, data);
        return actionButtons;
    }
}