/// <reference path="../Filter.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="StudentApprovalDataTable.ts" />

class StudentApprovalFilter extends Filter {
    private _FirstNameFilter: JQuery;
    private _LastNameFilter: JQuery;
    private _IdFilter: JQuery;
    private _SchoolsFilter: JQuery;
    private _ProvidersFilter: JQuery;

    constructor() {
        super();
        this._FirstNameFilter = $('#firstName');
        this._LastNameFilter = $('#lastName');
        this._IdFilter = $('#id');
        this._SchoolsFilter = $('#schoolsFilterList');
        this._ProvidersFilter = $('#providerFilterList');
    }

    Initialize(studentApprovalDataTable: StudentApprovalDataTable): void {
        var firstNameFilterUI: any = this._FirstNameFilter;
        var lastNameFilterUI: any = this._LastNameFilter;
        var idNameFilterUI: any = this._IdFilter;
        firstNameFilterUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/Student/AutocompleteFirstName', function () {
            studentApprovalDataTable.Refresh();
        }));
        lastNameFilterUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/Student/AutocompleteLastName', function () {
            studentApprovalDataTable.Refresh();
        }));
        idNameFilterUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/Student/AutocompleteID', function () {
            studentApprovalDataTable.Refresh();
        }));
        this._FirstNameFilter.add(this._LastNameFilter).add(this._IdFilter).keyup(function () {
            studentApprovalDataTable.Refresh();
        });
        this._SchoolsFilter.add(this._ProvidersFilter).change(function () {
            studentApprovalDataTable.Refresh();
        });
    }

    PushToArray(aoData: any[]): void {
        aoData.push({ "name": 'firstName', "value": this._FirstNameFilter.val() });
        aoData.push({ "name": 'lastName', "value": this._LastNameFilter.val() });
        aoData.push({ "name": 'ID', "value": this._IdFilter.val() });
        aoData.push({ "name": 'schools', "value": ExtractSelectListValues(this._SchoolsFilter.find(':selected')) });
        aoData.push({ "name": 'providers', "value": ExtractSelectListValues(this._ProvidersFilter.find(':selected')) });
    }
}