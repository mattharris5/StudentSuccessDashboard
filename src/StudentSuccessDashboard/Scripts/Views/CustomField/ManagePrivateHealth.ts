/// <reference path="../Filter.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PrivateHealthFieldDataTable.ts" />

class PrivateHealthFieldManagePage extends ManagePage {
    private _PrivateHealthFieldDataTable: IDataTable;

    constructor() {
        super();
        this._PrivateHealthFieldDataTable = new PrivateHealthFieldDataTable();
    }

    Initialize(): void {
        this._PrivateHealthFieldDataTable.Initialize(new Filter());
        this.SetupControls();
        this.SetupModals();
    }

    SetupControls(): void {
        
    }

    SetupModals(): void {
        var instance: PrivateHealthFieldManagePage = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._PrivateHealthFieldDataTable.DataTableElement,
            '#createPrivateHealthField', null, '/CustomFields/PrivateHealth/Create', false, 'Create New Private and Health Field', 'Create Private and Health Field', '#form-addedit',
            function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            }, null, function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            });

        CrudUtilities.WireupCrudModal(instance._PrivateHealthFieldDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#privateHealthFields tr .editPrivateHealthFields', '/CustomFields/PrivateHealth/Edit/', true, 'Edit Private and Health Field', 'Edit Private and Health Field', '#form-addedit', function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            }, null, function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            });

        CrudUtilities.WireupCrudModal(instance._PrivateHealthFieldDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#privateHealthFields tr .deletePrivateHealthFields', '/CustomFields/PrivateHealth/Delete/', true, 'Delete Private and Health Field', 'Delete', '#form-addedit',
            function () {
                CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
                CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
            }, null, null);
    }
}

$(document).ready(function () {
    new PrivateHealthFieldManagePage().Initialize();
});