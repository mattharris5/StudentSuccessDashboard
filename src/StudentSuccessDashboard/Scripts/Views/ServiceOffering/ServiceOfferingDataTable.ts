/// <reference path="../DataTable.ts" />
/// <reference path="ServiceOfferingFilter.ts" />
/// <reference path="../StringExtensions.ts" />

class ServiceOfferingDataTable extends DataTable {
    constructor() {
        super($('#serviceOfferings'));
    }

    GetServerDataUrl(): string {
        return "/ServiceOffering/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[]{
        return [0, 1, 2];
    }

    DrawCallback(oSettings: any): void {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(4, !DataTable.IsInPrintExportMode);
        if (DataTable.IsInPrintExportMode) {
            $('#FilterHeader').show();
            var serviceTypeProvider = $('#autocompleteServiceTypeorProvider').val();
            var serviceTypeFilters = $('.serviceTypeFilter:input:checkbox:checked').get();
            var serviceCategoryFilters = $('.serviceCategoryFilter:input:checkbox:checked').get();
            var filterHeaderHtml = "";
            if (serviceTypeProvider.length > 0) {
                filterHeaderHtml += "<h3>Service Type or Provider: " + serviceTypeProvider + "</h3>";
            }
            if (serviceTypeFilters.length > 0) {
                filterHeaderHtml += "<h3>Service Types: ";
                for (var i = 0; i < serviceTypeFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += serviceTypeFilters[i].getAttribute('data-value');
                }
                filterHeaderHtml += "</h3>";
            }
            if (serviceCategoryFilters.length > 0) {
                filterHeaderHtml += "<h3>Service Categories: ";
                for (var i = 0; i < serviceCategoryFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += serviceCategoryFilters[i].getAttribute('data-value');
                }
                filterHeaderHtml += "</h3>";
            }
            $('#FilterHeader').html(filterHeaderHtml);
        }
        else {
            $('#FilterHeader').hide();
        }
    }

    CreateColumns(): any[] {
        var columns: any[] = [
            {
                "mData": "IsFavorite",
                "mRender": this.RenderFavorites,
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "ServiceType",
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Provider",
                "bSortable": false,
                "bSearchable": false
            },
            {
                "mData": "Program",
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

    RenderFavorites(data: any, type: string, full: any): string {
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
    }

    RenderActions(data: any, type: string, full: any): string {
        if (full.CanInteract) {
            var buttons = "<ul class=\"buttons clearfix\">";
            buttons += "<li><a class='btn btn-primary btn-mini' data-value='{0}' href='/ServiceOffering/DownloadTemplate/{0}'><i class='icon-download'></i> Download Offering Template</a></li>" +
            "<li><a class='btn btn-primary btn-mini' data-value='{0}' href='/ServiceAttendance/DownloadTemplate/{0}'><i class='icon-download'></i> Download Attendance Template</a></li></ul> ";

            return StringExtensions.format(buttons, data);
        }
        return "";
    }
}