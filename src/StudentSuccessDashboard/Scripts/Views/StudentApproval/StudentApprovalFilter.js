/// <reference path="../Filter.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="StudentApprovalDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentApprovalFilter = (function (_super) {
    __extends(StudentApprovalFilter, _super);
    function StudentApprovalFilter() {
        _super.call(this);
        this._FirstNameFilter = $('#firstName');
        this._LastNameFilter = $('#lastName');
        this._IdFilter = $('#id');
        this._SchoolsFilter = $('#schoolsFilterList');
        this._ProvidersFilter = $('#providerFilterList');
    }
    StudentApprovalFilter.prototype.Initialize = function (studentApprovalDataTable) {
        var firstNameFilterUI = this._FirstNameFilter;
        var lastNameFilterUI = this._LastNameFilter;
        var idNameFilterUI = this._IdFilter;
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
    };

    StudentApprovalFilter.prototype.PushToArray = function (aoData) {
        aoData.push({ "name": 'firstName', "value": this._FirstNameFilter.val() });
        aoData.push({ "name": 'lastName', "value": this._LastNameFilter.val() });
        aoData.push({ "name": 'ID', "value": this._IdFilter.val() });
        aoData.push({ "name": 'schools', "value": ExtractSelectListValues(this._SchoolsFilter.find(':selected')) });
        aoData.push({ "name": 'providers', "value": ExtractSelectListValues(this._ProvidersFilter.find(':selected')) });
    };
    return StudentApprovalFilter;
})(Filter);
//# sourceMappingURL=StudentApprovalFilter.js.map
