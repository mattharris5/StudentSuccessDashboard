/// <reference path="../ManagePage.ts" />
/// <reference path="ServiceTypeDataTable.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="../StringExtensions.ts" />

class ServiceTypeManagePage  extends ManagePage {
    private _ServiceTypeFilter: IFilter;
    private _ServiceTypeDataTable: IDataTable;

    constructor() {
        super();
        this._ServiceTypeFilter = new ServiceTypeFilter();
        this._ServiceTypeDataTable = new ServiceTypeDataTable();
    }

    Initialize(): void {
        this._ServiceTypeDataTable.Initialize(this._ServiceTypeFilter);
        this.SetupControls();
        this.SetupModals();
    }

    SetupControls(): void {
        var instance: ServiceTypeManagePage = this;
        $('.container-fluid.col2').addClass('no-legend');
        $('.dropdown-toggle').dropdown();
        var multiOpenAccordion: any = $('#multiOpenAccordion');
        multiOpenAccordion.multiOpenAccordion({
            active: false
        });
        
        this._ServiceTypeFilter.Initialize(this._ServiceTypeDataTable);
        CrudUtilities.MainWrapElement.on('change', '#serviceTypes tr .private-checkbox', function () {
            var id = $(this).data('value');
            var url = '/ServiceType/SetPrivate/' + id;
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: url,
                data: { "IsPrivate": $(this).is(':checked') },
                success: function (result) {
                    if (result) {
                        instance._ServiceTypeDataTable.Refresh();
                    }
                },
                error: function (result) {
                    alert('Could not update favorite setting due to error.');
                    this._ServiceTypeDataTable.Refresh();
                }
            });
        });

        var multiSelect: any = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top',
        });
    }

    SetupModals(): void {
        CrudUtilities.ModalElement.on('hidden', function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        });

        CrudUtilities.WireupCrudModal(this._ServiceTypeDataTable.DataTableElement, '#addServiceType', null, '/ServiceType/Create', false, 'Add Service Type', 'Add Service Type', '#add-svcs',
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

        CrudUtilities.WireupCrudModal(this._ServiceTypeDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#serviceTypes tr .editServiceType', '/ServiceType/Edit/', true, 'Edit Service Type Details', 'Edit Details', '#add-svcs',
                        function () {
                            CrudUtilities.ModalTitleElement.text('Edit Service Type ' + $('#Name').val() + ' Details');
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

        CrudUtilities.WireupCrudModal(this._ServiceTypeDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#serviceTypes tr .deleteServiceType', '/ServiceType/Delete/', true, 'Delete Service Type', 'Delete', '#add-svcs',
                        function () {
                            CrudUtilities.ModalTitleElement.text('Delete Service Type ' + $('#Name').text());
                            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
                        }, null, null);
    }
}

$(document).ready(function () {
    new ServiceTypeManagePage().Initialize();
});