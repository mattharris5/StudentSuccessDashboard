/// <reference path="../Filter.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="ServiceOfferingDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ServiceOfferingFilter = (function (_super) {
    __extends(ServiceOfferingFilter, _super);
    function ServiceOfferingFilter() {
        _super.call(this);
        this._SearchField = $('#autocompleteServiceTypeorProvider');
        this._TypeFilterOptions = $('.serviceTypeFilter', '#TypeFilter');
        this._CategoryFilterOptions = $('.serviceCategoryFilter', '#CategoryFilter');
    }
    ServiceOfferingFilter.prototype.Initialize = function (serviceOfferingDataTable) {
        var searchFieldUI = this._SearchField;
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
    };

    ServiceOfferingFilter.prototype.PushToArray = function (aoData) {
        aoData.push({ "name": "ServiceTypeProviderOrProgram", "value": this._SearchField.val() });
        aoData.push({ "name": "ServiceTypeFilters", "value": $('#ServiceTypeFilterValues').text() });
        aoData.push({ "name": "ServiceCategoryFilters", "value": $('#ServiceCategoryFilterValues').text() });
    };
    return ServiceOfferingFilter;
})(Filter);
//# sourceMappingURL=ServiceOfferingFilter.js.map
