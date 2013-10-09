/// <reference path="../ManagePage.ts" />
/// <reference path="StudentSelector.ts" />
/// <reference path="StudentFilter.ts" />
/// <reference path="StudentDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentFinderPage = (function (_super) {
    __extends(StudentFinderPage, _super);
    function StudentFinderPage() {
        _super.call(this);
        this._StudentSelector = new StudentSelector();
        this._StudentDataTable = new StudentDataTable(this._StudentSelector);
        this._StudentFilter = new StudentFilter();
    }
    StudentFinderPage.prototype.Initialize = function () {
        this._StudentDataTable.Initialize(this._StudentFilter);
        this.SetupControls();
        this.SetupModals();
    };

    StudentFinderPage.prototype.SetupControls = function () {
        var instance = this;
        var dropDownToggleElements = $('.dropdown-toggle');
        var multiOpenAccordionElements = $('#multiOpenAccordion');

        dropDownToggleElements.dropdown();
        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top'
        });
        this._StudentFilter.Initialize(this._StudentDataTable);
        CrudUtilities.MainWrapElement.on('change', 'tr input:checkbox', function () {
            var checkbox = $(this);
            var removeValue = checkbox.data('value');
            instance._StudentSelector.Remove(removeValue);
            if (checkbox.is(':checked')) {
                checkbox.closest('tr').addClass('row_selected');
                instance._StudentSelector.Add(checkbox.data('value'));
            } else {
                checkbox.closest('tr').removeClass('row_selected');
                $('#chkSelectAll').removeAttr('checked');
            }
        });
        $('#addServiceOfferingToStudents').on('click', function (event) {
            if (instance._StudentSelector.SelectedRows.length == 0) {
                CrudUtilities.DisplayErrorAlert('Add Service Offering', '<ul class="form"><li class="clearfix">Please select a student to add a service offering to</li></ul>');
                event.preventDefault();
                return;
            }
            var postUrl = '/Service/SaveStudentIds/';
            event.preventDefault();
            $.ajax({
                type: 'POST',
                dataType: 'html',
                url: postUrl,
                data: JSON.stringify({ students: instance._StudentSelector.SelectedRows, returnUrl: '/Student/' }),
                contentType: 'application/json',
                success: function (result) {
                    window.location.href = '/Service/ScheduleOffering?ReturnUrl=/Student';
                }
            });
        });
        $('#serviceRequestFlagModal').on('click', function () {
            if (instance._StudentSelector.SelectedRows.length == 0) {
                CrudUtilities.DisplayErrorAlert('Request Service', '<ul class="form"><li class="clearfix">Please select a student to add a service request to</li></ul>');
                return;
            }

            CrudUtilities.DisplayCrudModal(instance._StudentDataTable.DataTableElement, '/ServiceRequest/Create', '/ServiceRequest/Create', 'Request Service', 'Apply Service Request(s)', '#flagModalBody', function () {
                instance._StudentSelector.AddHiddenSelectedElements();
                multiOpenAccordionElements.multiOpenAccordion({
                    active: false
                });
                multiSelect.bsmSelect({
                    addItemTarget: 'top'
                });
            }, function () {
                instance._StudentSelector.Reset();
                instance._StudentDataTable.Refresh();
            }, function () {
                multiSelect.bsmSelect({
                    addItemTarget: 'top'
                });
            });
        });

        $("#chkSelectAll").click(function () {
            $(this).parents('table').find(':checkbox').attr('checked', this.checked);
            if (this.checked) {
                //need to populate with all IDs
                instance._StudentSelector.UpdateSelections(true, instance._StudentFilter.ToJson());
                instance._StudentSelector.CheckSelectedElements($(CrudUtilities.MainWrapSelector + '#tbodyStudents').find(':checkbox'));
            } else {
                instance._StudentSelector.Reset();
            }
        });
    };

    StudentFinderPage.prototype.SetupModals = function () {
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(this._StudentDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#tbodyStudents tr .removeServiceRequest', '/ServiceRequest/Delete/', true, 'Delete Service Request', 'Remove', '#flagModalBody', function () {
            CrudUtilities.ModalTitleElement.text('Remove ' + $('#ServiceRequestToDeleteName').text());
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, null, null);

        CrudUtilities.WireupCrudModal(this._StudentDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#tbodyStudents tr .editServiceRequest', '/ServiceRequest/Edit/', true, 'Edit Service Request', 'Edit Service Request', '#flagModalBody', function () {
            CrudUtilities.ModalTitleElement.text('Edit ' + $('#SelectedServiceTypeId :selected').text() + ' / ' + $('#SelectedSubjectId :selected').text());
        }, null, null);

        CrudUtilities.WireupCrudModal(this._StudentDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#tbodyStudents tr .removeAssignedServiceOffering', '/Service/DeleteScheduledOffering/', true, 'Remove Service Offering', 'Remove Service Offering', '#form-remove', function () {
            CrudUtilities.ModalTitleElement.text('Remove Service Offering ' + $('#scheduledServiceOfferingToDelete').text());
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, null, null);

        CrudUtilities.WireupCrudModal(this._StudentDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#tbodyStudents tr .editAssignedServiceOffering', '/Service/EditScheduledOffering/', true, 'Edit Service Offering', 'Edit Service Offering', '#form-addedit', function () {
            var datepickerUI = $('.hasDatePicker');
            datepickerUI.datepicker().css('z-index', '5000');
            CrudUtilities.ModalTitleElement.text('Edit Service Offering ' + $('#Name').val());
        }, null, null);
    };
    return StudentFinderPage;
})(ManagePage);

$(document).ready(function () {
    new StudentFinderPage().Initialize();
});
//# sourceMappingURL=Finder.js.map
