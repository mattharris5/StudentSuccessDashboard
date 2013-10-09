/// <reference path="AccessAudit.ts" />
/// <reference path="../DataTable.ts" />

class AccessAuditDataTable extends DataTable{
    constructor(){
        super($("#accessAudit"));
    }

    GetServerDataUrl(): string {
        return "/User/AccessAuditDataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0, 1, 2, 3];
    }

    CreateColumns(): any[]{
        var columns: any[] = [
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

    RenderRoles(data: any): string {
        var parser=new DOMParser();
        var xmlDoc=parser.parseFromString(data,"text/xml");
        var html = "";
        var x=xmlDoc.getElementsByTagName("roles");
        for (var i=0; i < x.length; i++)
        { 
            html += x[i].childNodes[0].attributes["name"].value;
        }
        return html;
    }

    RenderAssociations(data: any): string {
        var parser=new DOMParser();
        var xmlDoc=parser.parseFromString(data,"text/xml");
        var x=xmlDoc.getElementsByTagName("providers");
        AccessAuditPage.accessDataArray.push(data.toString());
        if (x[0].childNodes[0] != undefined) {
            return "<a class='btn btn-mini user-associations associations' data-toggle='modal' role='button' data-value='" + (AccessAuditPage.accessDataArray.length - 1) + "'>" + x[0].childNodes.length + "</a>"
        }
        x=xmlDoc.getElementsByTagName("schools");
        if (x[0].childNodes[0] != undefined) {
            return "<a class='btn btn-mini user-associations associations' data-toggle='modal' role='button' data-value='" + (AccessAuditPage.accessDataArray.length - 1) + "'>" + x[0].childNodes.length + "</a>"
        }
        return "";
    }

    RenderActive(data: any): string {
        return (data == true) ? '<i class="icon-ok"></i>' : "";
    }
    
}