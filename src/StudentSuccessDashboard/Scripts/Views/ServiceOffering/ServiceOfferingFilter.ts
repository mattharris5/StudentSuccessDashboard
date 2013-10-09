/// <reference path="../Filter.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="ServiceOfferingDataTable.ts" />

class ServiceOfferingFilter extends Filter {
    private _SearchField: JQuery;
    private _TypeFilterOptions: JQuery;
    private _CategoryFilterOptions: JQuery;

    constructor() {
        super();
        this._SearchField = $('#autocompleteServiceTypeorProvider');
        this._TypeFilterOptions = $('.serviceTypeFilter', '#TypeFilter');
        this._CategoryFilterOptions = $('.serviceCategoryFilter', '#CategoryFilter');
    }

    Initialize(serviceOfferingDataTable: ServiceOfferingDataTable): void {
        var searchFieldUI: any = this._SearchField;
        searchFieldUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/ServiceOffering/autocompleteServiceTypeProviderOrProgram', serviceOfferingDataTable.Refresh));
        this._SearchField.keyup(function () {
            serviceOfferingDataTable.Refresh();
        });
        this._TypeFilterOptions.change(function () {
            ControlUtilities.UpdateFilter('#TypeFilter input:checkbox:checked', '#ServiceTypeFilterValues');
            serviceOfferingDataTable.Refresh();
        });
        this._CategoryFilterOptions.change(function () {
            ControlUtilities.UpdateFilter('#CategoryFilter input:checkbox:checked', '#ServiceCategoryFilterValues');
            serviceOfferingDataTable.Refresh();
        });
    }

    PushToArray(aoData: any[]): void {
        aoData.push({ "name": "ServiceTypeProviderOrProgram", "value": this._SearchField.val() });
        aoData.push({ "name": "ServiceTypeFilters", "value": $('#ServiceTypeFilterValues').text() });
        aoData.push({ "name": "ServiceCategoryFilters", "value": $('#ServiceCategoryFilterValues').text() });
    }
}