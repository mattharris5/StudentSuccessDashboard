/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="ServiceOfferingFilter.ts" />
/// <reference path="ServiceOfferingDataTable.ts" />
/// <reference path="../StringExtensions.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ServiceOfferingManagePage = (function (_super) {
    __extends(ServiceOfferingManagePage, _super);
    function ServiceOfferingManagePage() {
        _super.call(this);
        this._ServiceOfferingFilter = new ServiceOfferingFilter();
        this._ServiceOfferingDataTable = new ServiceOfferingDataTable();
    }
    ServiceOfferingManagePage.prototype.Initialize = function () {
        this._ServiceOfferingDataTable.Initialize(this._ServiceOfferingFilter);
        this.SetupControls();
        this.SetupModals();
    };

    ServiceOfferingManagePage.prototype.SetupControls = function () {
        var instance = this;
        var dropdownToggleElements = $('.dropdown-toggle');
        var multiOpenAccordionElements = $('#multiOpenAccordion');
        $('.container-fluid.col2').addClass('no-legend');
        dropdownToggleElements.dropdown();
        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top'
        });

        instance._ServiceOfferingFilter.Initialize(instance._ServiceOfferingDataTable);
        CrudUtilities.MainWrapElement.on('change', '#serviceOfferings tr .favorite-checkbox', function () {
            var id = $(this).data('value');
            var url = '/ServiceOffering/SetFavorite/' + id;
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
    };

    ServiceOfferingManagePage.prototype.SetupModals = function () {
        var instance = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._ServiceOfferingDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#uploadAssignedServiceOfferings', '/ServiceOffering/FileUpload/', false, 'Upload Assigned Service Offerings', 'Upload', '#form-fileUpload', function () {
            CrudUtilities.SubmitButton.off('click');
            CrudUtilities.SubmitButton.on('click', function () {
                $('#form-fileUpload').submit();
            });
        }, null, null);

        CrudUtilities.WireupCrudModal(instance._ServiceOfferingDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#uploadServiceAttendances', '/ServiceAttendance/FileUpload/', false, 'Upload Service Attendances', 'Upload', '#form-fileUpload', function () {
            CrudUtilities.SubmitButton.off('click');
            CrudUtilities.SubmitButton.on('click', function () {
                $('#form-fileUpload').submit();
            });
        }, null, null);
    };
    return ServiceOfferingManagePage;
})(ManagePage);

$(document).ready(function () {
    new ServiceOfferingManagePage().Initialize();
});
//# sourceMappingURL=Manage.js.map
