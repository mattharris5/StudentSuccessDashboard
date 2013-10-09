/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="PartnerFilter.ts" />
/// <reference path="ProviderDataTable.ts" />
/// <reference path="ProgramDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var PartnerManagePage = (function (_super) {
    __extends(PartnerManagePage, _super);
    function PartnerManagePage() {
        _super.call(this);
        this._PartnerFilter = new PartnerFilter();
        this._ProviderDataTable = new ProviderDataTable();
        this._ProgramDataTable = new ProgramDataTable();
        this._DropDownToggles = $('.dropdown-toggle');
    }
    PartnerManagePage.prototype.Initialize = function () {
        this._ProviderDataTable.Initialize(this._PartnerFilter);
        this._ProgramDataTable.Initialize(this._PartnerFilter);
        this._PartnerFilter.Initialize(this._ProviderDataTable);
        this._PartnerFilter.Initialize(this._ProgramDataTable);
        this.SetupControls();
        this.SetupModals();
    };

    PartnerManagePage.prototype.SetupControls = function () {
        var instance = this;

        $('.container-fluid.col2').addClass('no-legend');
        this._DropDownToggles.dropdown();

        var multiSelect = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top'
        });
    };

    PartnerManagePage.prototype.SetupModals = function () {
        var instance = this;
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });
        CrudUtilities.WireupCrudModal(instance._ProviderDataTable.DataTableElement, '#addProvider', null, '/Partners/Provider/Create', false, 'Add Provider', 'Add Provider', '#form-addedit', function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, function () {
            instance._ProgramDataTable.Refresh();
        }, function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });
        CrudUtilities.WireupCrudModal(instance._ProgramDataTable.DataTableElement, '#addProgram', null, '/Partners/Program/Create', false, 'Add Program', 'Add Program', '#form-addedit', function () {
            var datepickerUI = $('.hasDatePicker');
            datepickerUI.datepicker().css('z-index', '5000');
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, function () {
            instance._ProviderDataTable.Refresh();
        }, function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });
        CrudUtilities.WireupCrudModal(instance._ProviderDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#providers tr .deleteProvider', '/Partners/Provider/Delete/', true, 'Delete Provider', 'Delete', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, function () {
            instance._ProgramDataTable.Refresh();
        }, null);
        CrudUtilities.WireupCrudModal(instance._ProgramDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#programs tr .deleteProgram', '/Partners/Program/Delete/', true, 'Delete Program', 'Delete', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Delete ' + $('#Name').text());
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, function () {
            instance._ProviderDataTable.Refresh();
        }, null);
        CrudUtilities.WireupCrudModal(instance._ProviderDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#providers tr .editProvider', '/Partners/Provider/Edit/', true, 'Edit Provider', 'Edit Details', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Edit Details for ' + $('#Name').val());
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, function () {
            instance._ProgramDataTable.Refresh();
        }, function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });
        CrudUtilities.WireupCrudModal(instance._ProgramDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#programs tr .editProgram', '/Partners/Program/Edit/', true, 'Edit Program', 'Edit Details', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Edit Details for ' + $('#Name').val());
            var datepickerUI = $('.hasDatePicker');
            datepickerUI.datepicker().css('z-index', '5000');
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, function () {
            instance._ProviderDataTable.Refresh();
        }, function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });
    };
    return PartnerManagePage;
})(ManagePage);

$(document).ready(function () {
    new PartnerManagePage().Initialize();
});
//# sourceMappingURL=Manage.js.map
