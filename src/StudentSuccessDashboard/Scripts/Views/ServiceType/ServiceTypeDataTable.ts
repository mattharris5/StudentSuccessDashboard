/// <reference path="../DataTable.ts" />
/// <reference path="ServiceTypeFilter.ts" />
/// <reference path="../StringExtensions.ts" />

class ServiceTypeDataTable extends DataTable {
    constructor() {
        super($('#serviceTypes'));
    }

    GetServerDataUrl(): string {
        return "/ServiceType/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0, 1, 2, 3];
    }

    CreateColumns(): any[] {
        var columns: any[] = [
            {
                "mData": "IsPrivate",
                "mRender": this.RenderPrivateFlags,
            },
            {
                "mData": "Name",
                "bSearchable": false
            },
            {
                "mData": "Programs",
                "mRender": this.RenderPrograms,
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Description",
                "bSearchable": false
            }
        ];
        if ($('#addServiceType').length) {
            columns.push({
                "mData": "Id",
                "mRender": this.RenderActions,
                "bSortable": false,
                "bSearchable": false
            });
        }
        return columns;
    }

    DrawCallback(oSettings: any): void {
        $('.DTTT_container').show();
        if ($('#addServiceType').length) {
            this.DataTableElement.fnSetColumnVis(4, !DataTable.IsInPrintExportMode);
        }
        if (DataTable.IsInPrintExportMode) {
            $('#FilterHeader').show();
            var serviceTypeName = $('#autocompleteServiceTypeName').val();
            var categoryFilters = $('#bsmListbsmContainer0 .bsmListItemLabel').get();
            var filterHeaderHtml = "";
            if (serviceTypeName.length > 0) {
                filterHeaderHtml += "<h3>Service Type Name: " + serviceTypeName + "</h3>";
            }
            if (categoryFilters.length > 0) {
                filterHeaderHtml += "<h3>Categories: ";
                for (var i = 0; i < categoryFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += categoryFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            $('#FilterHeader').html(filterHeaderHtml);
        }
        else {
            $('#FilterHeader').hide();
        }
    }

    RenderPrivateFlags(data: any, type: string, full: any): string {
        if (DataTable.IsInPrintExportMode) {
            return data;
        }
        if ($('#addServiceType').length) {
            var output = '<input type="checkbox" class="private-checkbox" data-value="{0}"';
            output = output + (data === true ? ' checked="CHECKED" />' : ' />');
            return StringExtensions.format(output, full.Id);
        }
        else if (data == true)
        {
            return '<i class="icon-ok FulfillmentStatus"></i>';
        }
        return '';
    }

    private RenderPrograms(data: any[], type: string, full: any): string {
        var valList: string = "";
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
        var buttons: string = "<ul class=\"buttons clearfix\">" +
                            "<li><a class='btn btn-primary btn-mini editServiceType' role='button' data-toggle='modal' value=\"{0}\" data-value='{0}'><i class='icon-edit'></i> Edit Service Type</a></li>" +
                            "<li><a class='btn btn-danger btn-mini deleteServiceType' data-value='{0}'><i class='icon-remove'></i> Delete</a></li></ul>";
        return StringExtensions.format(buttons, data);
    }
}