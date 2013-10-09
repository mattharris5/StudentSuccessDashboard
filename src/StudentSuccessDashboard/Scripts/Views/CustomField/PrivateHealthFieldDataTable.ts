/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />

class PrivateHealthFieldDataTable extends DataTable {
    constructor() {
        super($('#privateHealthFields'));
    }

    GetServerDataUrl(): string {
        return "/CustomFields/PrivateHealth/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0, 1, 2];
    }

    CreateColumns(): any[] {
        var columns: any[] = [
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
    }

    DrawCallback(oSettings: any): void {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(3, !DataTable.IsInPrintExportMode);
    }

    SetServerParameters(aoData: any[]): void {
    }

    private RenderCategories(data: any[], type: string, full: any): string {
        var valList: string = "";
        if (data.length > 0) {
            valList = "<ul>";
            for (var i = 0; i < data.length; i++) {
                valList = valList + StringExtensions.format("<li>{0}</li>", data[i]);
            }
            valList = valList + "</ul>";
        }
        return valList;
    }

    private RenderActions(data: any, type: string, full: any): string {
        var buttonOutput = '<ul class="buttons clearfix"><li>' +
                        '<a class ="btn btn-primary btn-mini editPrivateHealthFields" role="button" data-value="{0}"><i class ="icon-edit"></i>Edit</a>' +
                        '</li><li>' +
                        '<a class="btn btn-danger btn-mini deletePrivateHealthFields" role="button" data-value="{0}"><i class ="icon-remove"></i>Delete</a>' +
                        '</li></ul>';
        return StringExtensions.format(buttonOutput, data);
    }
}