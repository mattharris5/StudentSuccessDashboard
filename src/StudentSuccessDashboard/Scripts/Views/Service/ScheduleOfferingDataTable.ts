/// <reference path="../DataTable.ts" />
/// <reference path="ScheduleOfferingFilter.ts" />
/// <reference path="../StringExtensions.ts" />

class ScheduleOfferingDataTable extends DataTable {
    constructor() {
        super($('#serviceOfferingOptions'));
    }

    GetServerDataUrl(): string {
        return "/ServiceOffering/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return null;
    }

    CreateColumns(): any[] {
        var columns: any[] = [
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
    }

    DrawCallback(oSettings: any): void {
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

    private RenderPrivate(data: any, type: string, full: any): string {
        return (full.IsPrivate) ? '<i class="icon-ok"></i>' : "";
    }

    private RenderActions(data: any, type: string, full: any): string {
        if (full.CanInteract) {
            var serviceOfferingId: string = full.Id;
            if (serviceOfferingId != null) {
                var buttons: string = "<ul class=\"buttons clearfix\">" +
                    "<li><a class='btn btn-primary btn-mini scheduleOffering' role='button' data-toggle='modal' data-value='{0}'><i class='icon-plus'></i> Add This Service Offering</a></li></ul>";
                return StringExtensions.format(buttons, serviceOfferingId);
            }
        }
        return "";
    }
}