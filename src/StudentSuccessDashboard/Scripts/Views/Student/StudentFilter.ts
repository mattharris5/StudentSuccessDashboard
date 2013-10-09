/// <reference path="../Filter.ts" />
/// <reference path="StudentDataTable.ts" />

class StudentFilter extends Filter {
    
    private _FirstNameFilter: JQuery;
    private _LastNameFilter: JQuery;
    private _IdFilter: JQuery;
    private _GradesFilter: JQuery;
    private _SchoolsFilter: JQuery;
    private _PrioritiesFilter: JQuery;
    private _RequestStatusesFilter: JQuery;
    private _ServiceTypesFilter: JQuery;
    private _SubjectsFilter: JQuery;

    constructor() {
        super();
        this._FirstNameFilter = $('#studentFirstName');
        this._LastNameFilter = $('#studentLastName');
        this._IdFilter = $('#studentID');
        this._GradesFilter = $('#grades');
        this._SchoolsFilter = $('#schools');
        this._PrioritiesFilter = $('#priorities');
        this._RequestStatusesFilter = $('#requestStatuses');
        this._ServiceTypesFilter = $('#serviceTypeFilterList');
        this._SubjectsFilter = $('#subjects');
    }

    Initialize(studentDataTable: IDataTable): void {
        var firstNameFilterUI: any = this._FirstNameFilter;
        var lastNameFilterUI: any = this._LastNameFilter;
        var idFilterUI: any = this._IdFilter;
        firstNameFilterUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/Student/AutocompleteFirstName', function () {
            studentDataTable.Refresh();
        }));
        lastNameFilterUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/Student/AutocompleteLastName', function () {
            studentDataTable.Refresh();
        }));
        idFilterUI.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/Student/AutocompleteID', function () {
            studentDataTable.Refresh();
        }));
        this._FirstNameFilter.add(this._LastNameFilter).add(this._IdFilter).keyup(function () {
            studentDataTable.Refresh();
        });
        this._GradesFilter.add(this._SchoolsFilter).add(this._PrioritiesFilter).add(this._RequestStatusesFilter).add(this._ServiceTypesFilter).add(this._SubjectsFilter).change(function () {
            studentDataTable.Refresh();
        });
    }

    PushToArray(aoData: any[]): void {
        var instance: StudentFilter = this;
        aoData.push({ "name": 'firstName', "value": instance._FirstNameFilter.val() });
        aoData.push({ "name": 'lastName', "value": instance._LastNameFilter.val() });
        aoData.push({ "name": 'ID', "value": instance._IdFilter.val() });
        aoData.push({ "name": 'grades', "value": ExtractSelectListValues(instance._GradesFilter.find(':selected')) });
        aoData.push({ "name": 'schools', "value": ExtractSelectListValues(instance._SchoolsFilter.find(':selected')) });
        aoData.push({ "name": 'priorities', "value": ExtractSelectListValues(instance._PrioritiesFilter.find(':selected')) });
        aoData.push({ "name": 'requestStatuses', "value": ExtractSelectListValues(instance._RequestStatusesFilter.find(':selected')) });
        aoData.push({ "name": 'serviceTypes', "value": ExtractSelectListValues(instance._ServiceTypesFilter.find(':selected')) });
        aoData.push({ "name": 'subjects', "value": ExtractSelectListValues(instance._SubjectsFilter.find(':selected')) });
    }

    ToJson(): any {
        return {
            firstName: this._FirstNameFilter.val(),
            lastName: this._LastNameFilter.val(),
            ID: this._IdFilter.val(),
            grades: ExtractSelectListValues(this._GradesFilter.find(':selected')),
            schools: ExtractSelectListValues(this._SchoolsFilter.find(':selected')),
            priorities: ExtractSelectListValues(this._PrioritiesFilter.find(':selected')),
            requestStatuses: ExtractSelectListValues(this._RequestStatusesFilter.find(':selected')),
            serviceTypes: ExtractSelectListValues(this._ServiceTypesFilter.find(':selected')),
            subjects: ExtractSelectListValues(this._SubjectsFilter.find(':selected'))
        };
    }
}