/// <reference path="../Filter.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PublicFieldDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PublicFieldManagePage = (function (_super) {
    __extends(PublicFieldManagePage, _super);
    function PublicFieldManagePage() {
        _super.call(this);
        this._PublicFieldDataTable = new PublicFieldDataTable();
    }
    PublicFieldManagePage.prototype.Initialize = function () {
        this._PublicFieldDataTable.Initialize(new Filter());
        this.SetupControls();
        this.SetupModals();
    };

    PublicFieldManagePage.prototype.SetupControls = function () {
    };

    PublicFieldManagePage.prototype.SetupModals = function () {
        var instance = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._PublicFieldDataTable.DataTableElement, '#createPublicField', null, '/CustomFields/Public/Create', false, 'Create New Public Field', 'Create Public Field', '#form-addedit', function () {
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

        CrudUtilities.WireupCrudModal(instance._PublicFieldDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#publicFields tr .editPublicFields', '/CustomFields/Public/Edit/', true, 'Edit Public Field', 'Edit Public Field', '#form-addedit', function () {
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

        CrudUtilities.WireupCrudModal(instance._PublicFieldDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#publicFields tr .deletePublicFields', '/CustomFields/Public/Delete/', true, 'Delete Public Field', 'Delete', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, null, null);
    };
    return PublicFieldManagePage;
})(ManagePage);

$(document).ready(function () {
    new PublicFieldManagePage().Initialize();
});
//# sourceMappingURL=ManagePublic.js.map
