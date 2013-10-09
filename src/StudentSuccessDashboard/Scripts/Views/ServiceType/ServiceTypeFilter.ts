/// <reference path="../Filter.ts" />
/// <reference path="ServiceTypeDataTable.ts" />
/// <reference path="../ssd.ts" />

class ServiceTypeFilter extends Filter {
    private _AutocompleteServiceTypeName: any;
    private _CategoriesFilter: JQuery;

    constructor() {
        super();
        this._AutocompleteServiceTypeName = $('#autocompleteServiceTypeName');
        this._CategoriesFilter = $('#categories');
    }

    Initialize(serviceTypeDataTable: ServiceTypeDataTable): void {
        this._AutocompleteServiceTypeName.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/ServiceType/AutocompleteServiceTypeName', serviceTypeDataTable.Refresh));
        this._AutocompleteServiceTypeName.keyup(function () {
            serviceTypeDataTable.Refresh();
        });
        this._CategoriesFilter.change(function () {
            serviceTypeDataTable.Refresh();
        });
    }

    PushToArray(aoData: any[]): void {
        aoData.push({ "name": "ServiceTypeName", "value": this._AutocompleteServiceTypeName.val() });
        aoData.push({ "name": 'categories', "value": ExtractSelectListValues(this._CategoriesFilter.find(':selected')) });
    }
}