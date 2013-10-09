/// <reference path="../Filter.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PublicFieldDataTable.ts" />

class PublicFieldManagePage extends ManagePage {
    private _PublicFieldDataTable: IDataTable;

    constructor() {
        super();
        this._PublicFieldDataTable = new PublicFieldDataTable();
    }

    Initialize(): void {
        this._PublicFieldDataTable.Initialize(new Filter());
        this.SetupControls();
        this.SetupModals();
    }

    SetupControls(): void {
        
    }

    SetupModals(): void {
        var instance: PublicFieldManagePage = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._PublicFieldDataTable.DataTableElement,
            '#createPublicField', null, '/CustomFields/Public/Create', false, 'Create New Public Field', 'Create Public Field', '#form-addedit',
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

        CrudUtilities.WireupCrudModal(instance._PublicFieldDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#publicFields tr .editPublicFields', '/CustomFields/Public/Edit/', true, 'Edit Public Field', 'Edit Public Field', '#form-addedit', function () {
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

        CrudUtilities.WireupCrudModal(instance._PublicFieldDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#publicFields tr .deletePublicFields', '/CustomFields/Public/Delete/', true, 'Delete Public Field', 'Delete', '#form-addedit',
            function () {
                CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
                CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
            }, null, null);
    }
}

$(document).ready(function () {
    new PublicFieldManagePage().Initialize();
});