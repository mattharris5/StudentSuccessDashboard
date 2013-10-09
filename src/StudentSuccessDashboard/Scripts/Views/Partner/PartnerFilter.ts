/// <reference path="../Filter.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="ProviderDataTable.ts" />
/// <reference path="../ssd.ts" />

class PartnerFilter extends Filter {
    private _PartnerNameFilterField: JQuery;

    constructor() {
        super();
        this._PartnerNameFilterField = $('#autocompletePartnerName');
    }

    Initialize(providerDataTable: ProviderDataTable): void {
        var partnerNameFilterField: any = this._PartnerNameFilterField;
        partnerNameFilterField.autocomplete({
            source: '/Partners/Partners/AutocompletePartnerName'
        });
        this._PartnerNameFilterField.on('keyup', function () {
            providerDataTable.Refresh();
        });
    }

    PushToArray(aoData: any[]) {
        aoData.push({ "name": "PartnerName", "value": this._PartnerNameFilterField.val() });
    }
}