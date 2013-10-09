/// <reference path="../StringExtensions.ts" />
/// <reference path="../DataTable.ts" />

class LoginAuditDataTable extends DataTable {
    constructor() {
        super($("#loginAudit"));
    }

    GetServerDataUrl(): string {
        return "/User/LoginAuditDataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0];
    }

    CreateColumns(): any[] {
        var columns: any[] = [
            {
                "mData": "CreateTime",
                "mRender": this.RenderCreateTime,
                "bSearchable": false
            }
        ];
        return columns;
    }

    DrawCallback(oSettings: any): void {
        $('.DTTT_container').show();
    }

    SetServerParameters(aoData: any[]): void {
        aoData.push({ "name": 'id', "value": $("#Id").val() });
    }

    RenderCreateTime(data: any): string {
        return StringExtensions.parseJsonDateTime(data).toLocaleString();
    }
}