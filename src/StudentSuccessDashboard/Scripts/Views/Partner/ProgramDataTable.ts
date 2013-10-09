/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PartnerFilter.ts" />

class ProgramDataTable extends DataTable {
    constructor() {
        super($('#programs'));
    }

    GetServerDataUrl(): string {
        return "/Partners/Program/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[]{
        return [0, 1, 2, 3];
    }

    CreateColumns(): any[] {
        var columns: any[] = [
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
    }

    DrawCallback(oSettings: any): void {
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
        }
        else {
            $('#ProgramFilterHeader').hide();
        }
    }

    private RenderContact(data: any, type: string, full: any): string {
        return StringExtensions.format("Contact Name: {0}<br/>Phone: {1}<br/>Email: <a href='mailto:{2}'>{2}</a>", data.Name, data.Phone, data.Email);
    }

    private RenderList(data: any[], type: string, full: any): string {
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
    }

    private RenderActions(data: any, type: string, full: any): string {
        var actionButtons = "<ul class='buttons clearfix'>" +
                "<li><a class='btn btn-primary btn-mini editProgram' role='button' data-toggle='modal' data-value='{0}'><i class='icon-edit'></i> Edit Program Details</a></li>" +
                "<li><a class='btn btn-danger btn-mini deleteProgram' data-value='{0}'><i class='icon-remove'></i> Delete</a></li></ul>";
        var actionButtonsMarkup = StringExtensions.format(actionButtons, data);
        return actionButtonsMarkup;
    }
}