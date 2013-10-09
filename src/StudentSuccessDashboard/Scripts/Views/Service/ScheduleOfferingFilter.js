/// <reference path="../Filter.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="ScheduleOfferingDataTable.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ScheduleOfferingFilter = (function (_super) {
    __extends(ScheduleOfferingFilter, _super);
    function ScheduleOfferingFilter() {
        _super.call(this);
        this.searchField = $('#autocompleteServiceTypeProviderOrProgramForScheduleServices');
        this.typeFilterOptions = $('.serviceTypeFilter', '#TypeFilter');
        this.categoryFilterOptions = $('.serviceCategoryFilter', '#CategoryFilter');
    }
    ScheduleOfferingFilter.prototype.Initialize = function (scheduleOfferingDataTable) {
        var searchFieldUI = this.searchField;
        searchFieldUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/ServiceOffering/AutocompleteServiceTypeProviderOrProgram', function () {
            scheduleOfferingDataTable.Refresh();
        }));
        searchFieldUI.keyup(function () {
            scheduleOfferingDataTable.Refresh();
        });
        this.typeFilterOptions.change(function () {
            ControlUtilities.UpdateFilter('#TypeFilter input:checkbox:checked', '#ServiceTypeFilterValues');
            scheduleOfferingDataTable.Refresh();
        });
        this.categoryFilterOptions.change(function () {
            ControlUtilities.UpdateFilter('#CategoryFilter input:checkbox:checked', '#ServiceCategoryFilterValues');
            scheduleOfferingDataTable.Refresh();
        });
    };

    ScheduleOfferingFilter.prototype.PushToArray = function (aoData) {
        aoData.push({ "name": "ServiceTypeProviderOrProgram", "value": this.searchField.val() });
        aoData.push({ "name": "ServiceTypeFilters", "value": $('#ServiceTypeFilterValues').text() });
        aoData.push({ "name": "ServiceCategoryFilters", "value": $('#ServiceCategoryFilterValues').text() });
    };
    return ScheduleOfferingFilter;
})(Filter);
//# sourceMappingURL=ScheduleOfferingFilter.js.map
