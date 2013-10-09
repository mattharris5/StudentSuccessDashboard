/// <reference path="../ManagePage.ts" />
/// <reference path="StudentApprovalDataTable.ts" />

class ManageStudentApprovalPage extends ManagePage {
    private _StudentApprovalFilter: IFilter;
    private _StudentApprovalDataTable: IDataTable;

    constructor(studentApprovalDataTable: StudentApprovalDataTable) {
        super();
        if (!studentApprovalDataTable) {
            throw "StudentApprovalDataTable is required.";
        }
        this._StudentApprovalDataTable = studentApprovalDataTable;
        this._StudentApprovalFilter = new StudentApprovalFilter();
    }

    Initialize(): void {
        this._StudentApprovalDataTable.Initialize(this._StudentApprovalFilter);
        this._StudentApprovalFilter.Initialize(this._StudentApprovalDataTable);
        this.SetupControls();
        this.SetupModals();
    }

    SetupControls(): void {
        var instance: ManageStudentApprovalPage = this;
        var multiOpenAccordionElements: any = $('#multiOpenAccordion');
  
        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect: any = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top',
        });
        CrudUtilities.MainWrapElement.on('click', '.addProviders', function () {
            var jqueryThis: JQuery = $(this);
            var id: string = jqueryThis.data("studentid");
            var name: string = jqueryThis.data("studentname");
            instance.AddProviders(id, name);
        });
        CrudUtilities.MainWrapElement.on('change', '#studentApprovals .optOut-checkbox', function () {
            var jqueryThis: JQuery = $(this);
            var id: string = jqueryThis.data('value');
            var hasParentalOptOut: boolean = jqueryThis.is(':checked');
            instance.UpdateOptOut(id, hasParentalOptOut);
        });
        CrudUtilities.MainWrapElement.on('click', '#removeApprovals', function () {
            instance.RemoveAll();
        });
    }

    SetupModals(): void {
        var instance: ManageStudentApprovalPage = this;

        CrudUtilities.WireupCrudModal(this._StudentApprovalDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#removeApprovalsBySchool', '/StudentApproval/RemoveAllProvidersBySchool/', false, 'Select & Remove Provider Approvals', 'Remove Selected Provider Approvals', '#form-remove-all-providers', function () {
            var selectSchoolsField: any = $('#SelectedSchools');
            selectSchoolsField.bsmSelect({
                addItemTarget: 'top',
            });
        }, 
        function () {
            $.ajax({
                url: '/StudentApproval/CountStudentsWithApprovedProviders',
                type: 'POST',
                dataType: 'json',
                success: function (result) {
                    $('#lblApprovalCount').text(result);
                }
            });
        },
            null);

        CrudUtilities.WireupCrudModal(this._StudentApprovalDataTable.DataTableElement,
            CrudUtilities.MainWrapSelector, '#studentApprovals tr .remove', '/StudentApproval/RemoveProvider/',
            true, 'Remove Provider', 'Remove', '#form-addedit',
            function () {
                CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
            }, 
            function () {
                $.ajax({
                    url: '/StudentApproval/CountStudentsWithApprovedProviders',
                    type: 'POST',
                    dataType: 'json',
                    success: function (result) {
                        $('#lblApprovalCount').text(result);
                    }
                });
            },
            null);
    }

    AddProviders(id, name): void {
        CrudUtilities.DisplayCrudModal(this._StudentApprovalDataTable.DataTableElement,
            '/StudentApproval/AddProviders/' + id, '/StudentApproval/AddProviders',
            'Add Approved Provider(s) for ' + name, 'Add Approved Provider(s)', '#form-student-provider',
            function () {
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            }, 
            function () {
                $.ajax({
                    url: '/StudentApproval/CountStudentsWithApprovedProviders',
                    type: 'POST',
                    dataType: 'json',
                    success: function (result) {
                        $('#lblApprovalCount').text(result);
                    }
                });
            },
            null);
    }

    UpdateOptOut(id: string, hasParentalOptOut: boolean): void {
        var instance: ManageStudentApprovalPage = this;
        var url: string = '/StudentApproval/SetOptOut/' + id;
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,
            data: { "HasParentalOptOut": hasParentalOptOut },
            success: function (result) {
                if (result) {
                    instance._StudentApprovalDataTable.Refresh();
                }
            },
            error: function (result) {
                alert('Could not update opt-out setting due to error.');
                instance._StudentApprovalDataTable.Refresh();
            }
        });
    }

    RemoveAll(): void {
        var instance: ManageStudentApprovalPage = this;
        var htmlString = '<form id="form-removeAll"><ul class="form">' +
                '<li id="SelectedUserIds" class="clearfix">Are you sure you wish to remove all student provider approvals?</li></ul></form>';
        CrudUtilities.ModalBodyElement.html(htmlString);
        CrudUtilities.ModalTitleElement.html('Remove All Provider Approvals');
        CrudUtilities.SubmitButton.text('Remove All');
        CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        CrudUtilities.ModalElement.modal({ show: true });
        CrudUtilities.ModalElement.on('click', CrudUtilities.SubmitButtonSelector, function (event) {
            event.preventDefault();
            CrudUtilities.SubmitButton.prop('disabled', true);
            var url = '/StudentApproval/RemoveAllProviders';
            $.ajax({
                type: 'POST',
                url: url,
                traditional: true,
                success: function (result) {
                    CrudUtilities.ModalElement.modal('hide');
                    CrudUtilities.SubmitButton.off('click');
                    instance._StudentApprovalDataTable.Refresh();
                    $.ajax({
                        url: '/StudentApproval/CountStudentsWithApprovedProviders',
                        type: 'POST',
                        dataType: 'json',
                        success: function (result) {
                            $('#lblApprovalCount').text(result);
                        }
                    });
                },
                error: function (result) {
                    CrudUtilities.ModalBodyElement.html(result.responseText);
                    var multiSelect: any = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top',
                    });
                    CrudUtilities.SubmitButton.removeAttr("disabled");
                }
            });
        });
    }
}

$(document).ready(function () {
    var studentApprovalDataTable: StudentApprovalDataTable = new StudentApprovalDataTable();
    new ManageStudentApprovalPage(studentApprovalDataTable).Initialize();
});
