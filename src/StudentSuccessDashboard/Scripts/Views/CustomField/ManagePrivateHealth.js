/// <reference path="../Filter.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PrivateHealthFieldDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PrivateHealthFieldManagePage = (function (_super) {
    __extends(PrivateHealthFieldManagePage, _super);
    function PrivateHealthFieldManagePage() {
        _super.call(this);
        this._PrivateHealthFieldDataTable = new PrivateHealthFieldDataTable();
    }
    PrivateHealthFieldManagePage.prototype.Initialize = function () {
        this._PrivateHealthFieldDataTable.Initialize(new Filter());
        this.SetupControls();
        this.SetupModals();
    };

    PrivateHealthFieldManagePage.prototype.SetupControls = function () {
    };

    PrivateHealthFieldManagePage.prototype.SetupModals = function () {
        var instance = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._PrivateHealthFieldDataTable.DataTableElement, '#createPrivateHealthField', null, '/CustomFields/PrivateHealth/Create', false, 'Create New Private and Health Field', 'Create Private and Health Field', '#form-addedit', function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, null, function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });

        CrudUtilities.WireupCrudModal(instance._PrivateHealthFieldDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#privateHealthFields tr .editPrivateHealthFields', '/CustomFields/PrivateHealth/Edit/', true, 'Edit Private and Health Field', 'Edit Private and Health Field', '#form-addedit', function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, null, function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });

        CrudUtilities.WireupCrudModal(instance._PrivateHealthFieldDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#privateHealthFields tr .deletePrivateHealthFields', '/CustomFields/PrivateHealth/Delete/', true, 'Delete Private and Health Field', 'Delete', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, null, null);
    };
    return PrivateHealthFieldManagePage;
})(ManagePage);

$(document).ready(function () {
    new PrivateHealthFieldManagePage().Initialize();
});
//# sourceMappingURL=ManagePrivateHealth.js.map
