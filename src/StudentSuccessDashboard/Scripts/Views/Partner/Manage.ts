/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PartnerFilter.ts" />
/// <reference path="ProviderDataTable.ts" />
/// <reference path="ProgramDataTable.ts" />

class PartnerManagePage extends ManagePage {
    private _PartnerFilter: IFilter;
    private _ProviderDataTable: IDataTable;
    private _ProgramDataTable: IDataTable;
    private _DropDownToggles: any;

    constructor() {
        super();
        this._PartnerFilter = new PartnerFilter();
        this._ProviderDataTable = new ProviderDataTable();
        this._ProgramDataTable = new ProgramDataTable();
        this._DropDownToggles = $('.dropdown-toggle');
    }

    Initialize(): void {
        this._ProviderDataTable.Initialize(this._PartnerFilter);
        this._ProgramDataTable.Initialize(this._PartnerFilter);
        this._PartnerFilter.Initialize(this._ProviderDataTable);
        this._PartnerFilter.Initialize(this._ProgramDataTable);
        this.SetupControls();
        this.SetupModals();
    }

    SetupControls(): void {
        var instance: PartnerManagePage = this;

        $('.container-fluid.col2').addClass('no-legend');
        this._DropDownToggles.dropdown();
        
        var multiSelect: any = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top',
        });
    }

    SetupModals(): void {
        var instance: PartnerManagePage = this;
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });
        CrudUtilities.WireupCrudModal(instance._ProviderDataTable.DataTableElement,
            '#addProvider', null, '/Partners/Provider/Create', false, 'Add Provider', 'Add Provider', '#form-addedit',
            function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            },
            function () {
                instance._ProgramDataTable.Refresh();
            },
            function () {
            var multiSelect: any = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top',
            });
        });
        CrudUtilities.WireupCrudModal(instance._ProgramDataTable.DataTableElement,
            '#addProgram', null, '/Partners/Program/Create', false, 'Add Program', 'Add Program', '#form-addedit',
            function () {
                var datepickerUI: any = $('.hasDatePicker');
                datepickerUI.datepicker().css('z-index', '5000');
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            }, function () {
                instance._ProviderDataTable.Refresh();
            },
            function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            });
        CrudUtilities.WireupCrudModal(instance._ProviderDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#providers tr .deleteProvider', '/Partners/Provider/Delete/', true, 'Delete Provider', 'Delete', '#form-addedit',
            function () {
                CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
                CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
            },
            function () {
                instance._ProgramDataTable.Refresh();
            }, null);
        CrudUtilities.WireupCrudModal(instance._ProgramDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#programs tr .deleteProgram', '/Partners/Program/Delete/', true, 'Delete Program', 'Delete', '#form-addedit',
            function () {
                CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
                CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
            },
            function () {
                instance._ProviderDataTable.Refresh();
            }, null);
        CrudUtilities.WireupCrudModal(instance._ProviderDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#providers tr .editProvider', '/Partners/Provider/Edit/', true, 'Edit Provider', 'Edit Details', '#form-addedit', function () {
                CrudUtilities.ModalTitleElement.text('Edit Details for ' + $('#Name').val());
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            },
            function () {
                instance._ProgramDataTable.Refresh();
            },
            function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            });
        CrudUtilities.WireupCrudModal(instance._ProgramDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#programs tr .editProgram', '/Partners/Program/Edit/', true, 'Edit Program', 'Edit Details', '#form-addedit', function () {
                CrudUtilities.ModalTitleElement.text('Edit Details for ' + $('#Name').val());
                var datepickerUI: any = $('.hasDatePicker');
                datepickerUI.datepicker().css('z-index', '5000');
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            },
            function () {
                instance._ProviderDataTable.Refresh();
            },
            function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            });
    }
}

$(document).ready(function () {
    new PartnerManagePage().Initialize();
});
