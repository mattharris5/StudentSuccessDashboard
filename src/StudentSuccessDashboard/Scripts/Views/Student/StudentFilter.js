/// <reference path="../Filter.ts" />
/// <reference path="StudentDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentFilter = (function (_super) {
    __extends(StudentFilter, _super);
    function StudentFilter() {
        _super.call(this);
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
    StudentFilter.prototype.Initialize = function (studentDataTable) {
        var firstNameFilterUI = this._FirstNameFilter;
        var lastNameFilterUI = this._LastNameFilter;
        var idFilterUI = this._IdFilter;
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
    };

    StudentFilter.prototype.PushToArray = function (aoData) {
        var instance = this;
        aoData.push({ "name": 'firstName', "value": instance._FirstNameFilter.val() });
        aoData.push({ "name": 'lastName', "value": instance._LastNameFilter.val() });
        aoData.push({ "name": 'ID', "value": instance._IdFilter.val() });
        aoData.push({ "name": 'grades', "value": ExtractSelectListValues(instance._GradesFilter.find(':selected')) });
        aoData.push({ "name": 'schools', "value": ExtractSelectListValues(instance._SchoolsFilter.find(':selected')) });
        aoData.push({ "name": 'priorities', "value": ExtractSelectListValues(instance._PrioritiesFilter.find(':selected')) });
        aoData.push({ "name": 'requestStatuses', "value": ExtractSelectListValues(instance._RequestStatusesFilter.find(':selected')) });
        aoData.push({ "name": 'serviceTypes', "value": ExtractSelectListValues(instance._ServiceTypesFilter.find(':selected')) });
        aoData.push({ "name": 'subjects', "value": ExtractSelectListValues(instance._SubjectsFilter.find(':selected')) });
    };

    StudentFilter.prototype.ToJson = function () {
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
    };
    return StudentFilter;
})(Filter);
//# sourceMappingURL=StudentFilter.js.map
