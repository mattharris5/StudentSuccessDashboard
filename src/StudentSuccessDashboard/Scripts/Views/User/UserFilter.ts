/// <reference path="../Filter.ts" />
/// <reference path="../ssd.ts" />

class UserFilter extends Filter {
    private _FirstNameFilterField: JQuery;
    private _LastNameFilterField: JQuery;
    private _EmailFilterField: JQuery;
    private _StatusFilterSelect: JQuery;
    private _SchoolsFilterSelect: JQuery;
    private _RolesFilterSelect: JQuery;

    constructor() {
        super();
        this._FirstNameFilterField = $('#userFirstName');
        this._LastNameFilterField = $('#userLastName');
        this._EmailFilterField = $('#email');
        this._StatusFilterSelect = $('#status');
        this._SchoolsFilterSelect = $('#schools');
        this._RolesFilterSelect = $('#roles');
    }
    
    Initialize(dt: any): void {
        this.SetupControls(dt);
        this.Reset();
    }

    SetupControls(dt: any): void {
        var autoCompleteFirstName: any = this._FirstNameFilterField;
        var autoCompleteLastName: any = this._LastNameFilterField;
        var autoCompleteEmail: any = this._EmailFilterField;
        autoCompleteFirstName.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/User/AutocompleteFirstName', dt.fnFilter));
        autoCompleteLastName.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/User/AutocompleteLastName', dt.fnFilter));
        autoCompleteEmail.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/User/AutocompleteEmail', dt.fnFilter));

        this._FirstNameFilterField.add(this._LastNameFilterField).add(this._EmailFilterField).keyup(function () {
            dt.fnFilter($(this).val());
        });
        this._StatusFilterSelect.add(this._SchoolsFilterSelect).add(this._RolesFilterSelect).change(function () {
            dt.fnFilter($(this).val());
        });
    }

    PushToArray(aoData: any[]): void {
        aoData.push({ "name": 'firstName', "value": this._FirstNameFilterField.val() });
        aoData.push({ "name": 'lastName', "value": this._LastNameFilterField.val() });
        aoData.push({ "name": 'email', "value": this._EmailFilterField.val() });
        aoData.push({ "name": 'status', "value": ExtractSelectListValues(this._StatusFilterSelect.find(':selected')) });
        aoData.push({ "name": 'schools', "value": ExtractSelectListValues(this._SchoolsFilterSelect.find(':selected')) });
        aoData.push({ "name": 'roles', "value": ExtractSelectListValues(this._RolesFilterSelect.find(':selected')) });
    }

    ToJson():any {
        return {
            FirstName: this._FirstNameFilterField.val(),
            LastName: this._LastNameFilterField.val(),
            Email: this._EmailFilterField.val(),
            Status: ExtractSelectListValues(this._StatusFilterSelect.find(':selected')),
            Schools: ExtractSelectListValues(this._SchoolsFilterSelect.find(':selected')),
            Roles: ExtractSelectListValues(this._RolesFilterSelect.find(':selected'))
        };
    }
}
