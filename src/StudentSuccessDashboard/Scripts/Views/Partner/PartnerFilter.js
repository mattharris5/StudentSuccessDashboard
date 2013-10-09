/// <reference path="../Filter.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="ProviderDataTable.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PartnerFilter = (function (_super) {
    __extends(PartnerFilter, _super);
    function PartnerFilter() {
        _super.call(this);
        this._PartnerNameFilterField = $('#autocompletePartnerName');
    }
    PartnerFilter.prototype.Initialize = function (providerDataTable) {
        var partnerNameFilterField = this._PartnerNameFilterField;
        partnerNameFilterField.autocomplete({
            source: '/Partners/Partners/AutocompletePartnerName'
        });
        this._PartnerNameFilterField.on('keyup', function () {
            providerDataTable.Refresh();
        });
    };

    PartnerFilter.prototype.PushToArray = function (aoData) {
        aoData.push({ "name": "PartnerName", "value": this._PartnerNameFilterField.val() });
    };
    return PartnerFilter;
})(Filter);
//# sourceMappingURL=PartnerFilter.js.map
