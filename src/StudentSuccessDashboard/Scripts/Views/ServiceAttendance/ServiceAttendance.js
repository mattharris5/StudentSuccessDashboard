/// <reference path="../ManagePage.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="ServiceAttendanceDataTable.ts" />
/// <reference path="../ManagePage.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ServiceAttendancePage = (function (_super) {
    __extends(ServiceAttendancePage, _super);
    function ServiceAttendancePage() {
        _super.call(this);
        this._ServiceAttendanceDataTable = new ServiceAttendanceDataTable();
        this._DropDownToggle = $('.dropdown-toggle');
    }
    ServiceAttendancePage.prototype.Initialize = function () {
        this._ServiceAttendanceDataTable.Initialize(null);
        this.SetupControls();
        this.SetupModals();
    };

    ServiceAttendancePage.prototype.SetupControls = function () {
        this._DropDownToggle.dropdown();
    };

    ServiceAttendancePage.prototype.SetupModals = function () {
        var instance = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(instance._ServiceAttendanceDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#addServiceAttendance', '/ServiceAttendance/Create/', true, 'Add Service Attendance', 'Add Service Attendance', '#modalBody', function () {
            CrudUtilities.ModalTitleElement.text('Add Service Attendance');
            var datepickerUI = $('.hasDatePicker');
            datepickerUI.datepicker().css('z-index', '5000');
            $('.durationLinks').click(function () {
                $('#appendedDropdownButton').val($(this).text());
            });
        }, null, null);

        CrudUtilities.WireupCrudModal(instance._ServiceAttendanceDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#serviceAttendances tr .editServiceAttendance', '/ServiceAttendance/Edit/', true, 'Edit Service Attendance', 'Edit Service Attendance', '#modalBody', function () {
            CrudUtilities.ModalTitleElement.text('Edit Service Attendance');
            var datepickerUI = $('.hasDatePicker');
            datepickerUI.datepicker().css('z-index', '5000');
            $('.durationLinks').click(function () {
                $('#appendedDropdownButton').val($(this).text());
            });
        }, null, null);

        CrudUtilities.WireupCrudModal(instance._ServiceAttendanceDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#serviceAttendances tr .deleteServiceAttendance', '/ServiceAttendance/Delete/', true, 'Delete Provider', 'Delete', '#form-addedit', function () {
            CrudUtilities.ModalTitleElement.text('Delete Service Attendance');
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, null, null);
    };
    return ServiceAttendancePage;
})(ManagePage);

$(document).ready(function () {
    new ServiceAttendancePage().Initialize();
});
//# sourceMappingURL=ServiceAttendance.js.map
