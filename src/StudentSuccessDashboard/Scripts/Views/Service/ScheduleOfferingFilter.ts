/// <reference path="../Filter.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="ScheduleOfferingDataTable.ts" />
/// <reference path="../ssd.ts" />

class ScheduleOfferingFilter extends Filter {
    private searchField: JQuery;
    private typeFilterOptions: JQuery;
    private categoryFilterOptions: JQuery;

    constructor() {
        super();
        this.searchField = $('#autocompleteServiceTypeProviderOrProgramForScheduleServices');
        this.typeFilterOptions = $('.serviceTypeFilter', '#TypeFilter');
        this.categoryFilterOptions = $('.serviceCategoryFilter', '#CategoryFilter');
    }

    Initialize(scheduleOfferingDataTable: ScheduleOfferingDataTable): void {
        var searchFieldUI: any = this.searchField;
        searchFieldUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/ServiceOffering/AutocompleteServiceTypeProviderOrProgram', function () {
            scheduleOfferingDataTable.Refresh()
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
    }

    PushToArray(aoData: any[]): void {
        aoData.push({ "name": "ServiceTypeProviderOrProgram", "value": this.searchField.val() });
        aoData.push({ "name": "ServiceTypeFilters", "value": $('#ServiceTypeFilterValues').text() });
        aoData.push({ "name": "ServiceCategoryFilters", "value": $('#ServiceCategoryFilterValues').text() });
    }
}