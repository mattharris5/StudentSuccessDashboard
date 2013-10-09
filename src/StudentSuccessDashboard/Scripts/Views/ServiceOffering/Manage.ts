/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="ServiceOfferingFilter.ts" />
/// <reference path="ServiceOfferingDataTable.ts" />
/// <reference path="../StringExtensions.ts" />

class ServiceOfferingManagePage extends ManagePage {
    private _ServiceOfferingFilter: IFilter; 
    private _ServiceOfferingDataTable: IDataTable; 

    constructor() {
        super();
        this._ServiceOfferingFilter = new ServiceOfferingFilter();
        this._ServiceOfferingDataTable = new ServiceOfferingDataTable();
    }

    Initialize(): void {
        this._ServiceOfferingDataTable.Initialize(this._ServiceOfferingFilter);
        this.SetupControls();
        this.SetupModals();
    }

    SetupControls(): void {
        var instance: ServiceOfferingManagePage = this;
        var dropdownToggleElements: any = $('.dropdown-toggle');
        var multiOpenAccordionElements: any = $('#multiOpenAccordion');
        $('.container-fluid.col2').addClass('no-legend');
        dropdownToggleElements.dropdown();
        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect: any = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top',
        });

        instance._ServiceOfferingFilter.Initialize(instance._ServiceOfferingDataTable);
        CrudUtilities.MainWrapElement.on('change', '#serviceOfferings tr .favorite-checkbox', function () {
            var id = $(this).data('value');
            var url: string = '/ServiceOffering/SetFavorite/' + id;
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: url,
                data: { "IsFavorite": $(this).is(':checked') },
                success: function (result) {
                    if (result) {
                        instance._ServiceOfferingDataTable.Refresh();
                    }
                },
                error: function (result) {
                    alert('Could not update favorite setting due to error.');
                    instance._ServiceOfferingDataTable.Refresh();
                }
            });
        });
    }

    SetupModals(): void {
        var instance: ServiceOfferingManagePage = this;
        
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._ServiceOfferingDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#uploadAssignedServiceOfferings', '/ServiceOffering/FileUpload/', false, 'Upload Assigned Service Offerings', 'Upload', '#form-fileUpload',
            function () {
                CrudUtilities.SubmitButton.off('click');
                CrudUtilities.SubmitButton.on('click', function () {
                    $('#form-fileUpload').submit();
                });
            }, null, null);

        CrudUtilities.WireupCrudModal(instance._ServiceOfferingDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#uploadServiceAttendances', '/ServiceAttendance/FileUpload/', false, 'Upload Service Attendances', 'Upload', '#form-fileUpload',
            function () {
                CrudUtilities.SubmitButton.off('click');
                CrudUtilities.SubmitButton.on('click', function () {
                    $('#form-fileUpload').submit();
                });
            }, null, null);
    }
}

$(document).ready(function () {
    new ServiceOfferingManagePage().Initialize();
});
