/// <reference path="../Filter.ts" />
/// <reference path="ServiceTypeDataTable.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ServiceTypeFilter = (function (_super) {
    __extends(ServiceTypeFilter, _super);
    function ServiceTypeFilter() {
        _super.call(this);
        this._AutocompleteServiceTypeName = $('#autocompleteServiceTypeName');
        this._CategoriesFilter = $('#categories');
    }
    ServiceTypeFilter.prototype.Initialize = function (serviceTypeDataTable) {
        this._AutocompleteServiceTypeName.autocomplete(ControlUtilities.CreateAutocompleteEditMonitorOptions('/ServiceType/AutocompleteServiceTypeName', serviceTypeDataTable.Refresh));
        this._AutocompleteServiceTypeName.keyup(function () {
            serviceTypeDataTable.Refresh();
        });
        this._CategoriesFilter.change(function () {
            serviceTypeDataTable.Refresh();
        });
    };

    ServiceTypeFilter.prototype.PushToArray = function (aoData) {
        aoData.push({ "name": "ServiceTypeName", "value": this._AutocompleteServiceTypeName.val() });
        aoData.push({ "name": 'categories', "value": ExtractSelectListValues(this._CategoriesFilter.find(':selected')) });
    };
    return ServiceTypeFilter;
})(Filter);
//# sourceMappingURL=ServiceTypeFilter.js.map
