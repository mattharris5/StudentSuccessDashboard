/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PartnerFilter.ts" />

class ProviderDataTable extends DataTable {
    constructor() {
        super($('#providers'));
    }

    GetServerDataUrl(): string {
        return "/Partners/Provider/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0, 1, 2];
    }

    CreateColumns(): any[] {
        var columns: any[] = [
            {
                "mData": "Name",
                "mRender": this.RenderProvider,
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Contact",
                "mRender": this.RenderContact,
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Schools",
                "mRender": this.RenderSchools,
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Programs",
                "mRender": this.RenderPrograms,
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
        this.DataTableElement.fnSetColumnVis(4, !DataTable.IsInPrintExportMode);
        if (DataTable.IsInPrintExportMode) {
            $('#ProviderFilterHeader').show();
            var providerName = $('#autocompletePartnerName').val();
            var filterHeaderHtml = "";
            if (providerName.length > 0) {
                filterHeaderHtml += "<h3>Provider Name: " + providerName + "</h3>";
            }
            $('#ProviderFilterHeader').html(filterHeaderHtml);
        }
        else {
            $('#ProviderFilterHeader').hide();
        }
    }

    RenderProvider(data: any, type: string, full: any): string {
        var value = data;
        if (full.Website) {
            value = StringExtensions.format("<a href='{1}' target='_blank'>{0}</a>", data, AddHyperlinkScheme(full.Website));
        }
        if (full.Address.Street && full.Address.City && full.Address.State && full.Address.Zip) {
            return StringExtensions.format("{0}<br />{1}<br />{2}, {3} {4}", value, full.Address.Street, full.Address.City, full.Address.State, full.Address.Zip);
        }
        return value;
    }

    RenderContact(data: any, type: string, full: any): string {
        var value = "";
        if (data.Name) {
            value = "Contact Name: " + data.Name;
        }
        if (data.Phone) {
            if (value.length > 0) {
                value = value + "<br />";
            }
            value = value + "Phone: " + data.Phone;
        }
        if (data.Email) {
            if (value.length > 0) {
                value = value + "<br />";
            }
            value = StringExtensions.format("{0}Email: <a href='mailto:{1}'>{1}</a>", value, data.Email);
        }
        return value;
    }

    private RenderSchools(data: any[], type: string, full: any): string {
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

    private RenderPrograms(data: any[], type: string, full: any): string {
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
        if (full.AccessMode === "All" || full.AccessMode === "Edit") {
            var actionButtons = "";
            if (full.AccessMode === "All") {
                actionButtons = "<ul class='buttons clearfix'>" +
                    "<li><a class='btn btn-primary btn-mini editProvider' role='button' data-toggle='modal' data-value='{0}'><i class='icon-edit'></i> Edit Provider Details</a></li>" +
                    "<li><a class='btn btn-danger btn-mini deleteProvider' data-value='{0}'><i class='icon-remove'></i> Delete</a></li></ul>";
            }
            else {
                actionButtons = "<ul class='buttons clearfix'>" +
                    "<li><a class='btn btn-primary btn-mini editProvider' role='button' data-toggle='modal' data-value='{0}'><i class='icon-edit'></i> Edit Provider Details</a></li></ul>";
            }
            var actionButtonsMarkup = StringExtensions.format(actionButtons, data, full.Name);
            return actionButtonsMarkup;
        }
        return "";
    }
}