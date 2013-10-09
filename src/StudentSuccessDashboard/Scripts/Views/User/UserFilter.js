/// <reference path="../Filter.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var UserFilter = (function (_super) {
    __extends(UserFilter, _super);
    function UserFilter() {
        _super.call(this);
        this._FirstNameFilterField = $('#userFirstName');
        this._LastNameFilterField = $('#userLastName');
        this._EmailFilterField = $('#email');
        this._StatusFilterSelect = $('#status');
        this._SchoolsFilterSelect = $('#schools');
        this._RolesFilterSelect = $('#roles');
    }
    UserFilter.prototype.Initialize = function (dt) {
        this.SetupControls(dt);
        this.Reset();
    };

    UserFilter.prototype.SetupControls = function (dt) {
        var autoCompleteFirstName = this._FirstNameFilterField;
        var autoCompleteLastName = this._LastNameFilterField;
        var autoCompleteEmail = this._EmailFilterField;
        autoCompleteFirstName.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/User/AutocompleteFirstName', dt.fnFilter));
        autoCompleteLastName.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/User/AutocompleteLastName', dt.fnFilter));
        autoCompleteEmail.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/User/AutocompleteEmail', dt.fnFilter));

        this._FirstNameFilterField.add(this._LastNameFilterField).add(this._EmailFilterField).keyup(function () {
            dt.fnFilter($(this).val());
        });
        this._StatusFilterSelect.add(this._SchoolsFilterSelect).add(this._RolesFilterSelect).change(function () {
            dt.fnFilter($(this).val());
        });
    };

    UserFilter.prototype.PushToArray = function (aoData) {
        aoData.push({ "name": 'firstName', "value": this._FirstNameFilterField.val() });
        aoData.push({ "name": 'lastName', "value": this._LastNameFilterField.val() });
        aoData.push({ "name": 'email', "value": this._EmailFilterField.val() });
        aoData.push({ "name": 'status', "value": ExtractSelectListValues(this._StatusFilterSelect.find(':selected')) });
        aoData.push({ "name": 'schools', "value": ExtractSelectListValues(this._SchoolsFilterSelect.find(':selected')) });
        aoData.push({ "name": 'roles', "value": ExtractSelectListValues(this._RolesFilterSelect.find(':selected')) });
    };

    UserFilter.prototype.ToJson = function () {
        return {
            FirstName: this._FirstNameFilterField.val(),
            LastName: this._LastNameFilterField.val(),
            Email: this._EmailFilterField.val(),
            Status: ExtractSelectListValues(this._StatusFilterSelect.find(':selected')),
            Schools: ExtractSelectListValues(this._SchoolsFilterSelect.find(':selected')),
            Roles: ExtractSelectListValues(this._RolesFilterSelect.find(':selected'))
        };
    };
    return UserFilter;
})(Filter);
//# sourceMappingURL=UserFilter.js.map
