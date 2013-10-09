/// <reference path="../ManagePage.ts" />
/// <reference path="StudentApprovalDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ManageStudentApprovalPage = (function (_super) {
    __extends(ManageStudentApprovalPage, _super);
    function ManageStudentApprovalPage(studentApprovalDataTable) {
        _super.call(this);
        if (!studentApprovalDataTable) {
            throw "StudentApprovalDataTable is required.";
        }
        this._StudentApprovalDataTable = studentApprovalDataTable;
        this._StudentApprovalFilter = new StudentApprovalFilter();
    }
    ManageStudentApprovalPage.prototype.Initialize = function () {
        this._StudentApprovalDataTable.Initialize(this._StudentApprovalFilter);
        this._StudentApprovalFilter.Initialize(this._StudentApprovalDataTable);
        this.SetupControls();
        this.SetupModals();
    };

    ManageStudentApprovalPage.prototype.SetupControls = function () {
        var instance = this;
        var multiOpenAccordionElements = $('#multiOpenAccordion');

        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top'
        });
        CrudUtilities.MainWrapElement.on('click', '.addProviders', function () {
            var jqueryThis = $(this);
            var id = jqueryThis.data("studentid");
            var name = jqueryThis.data("studentname");
            instance.AddProviders(id, name);
        });
        CrudUtilities.MainWrapElement.on('change', '#studentApprovals .optOut-checkbox', function () {
            var jqueryThis = $(this);
            var id = jqueryThis.data('value');
            var hasParentalOptOut = jqueryThis.is(':checked');
            instance.UpdateOptOut(id, hasParentalOptOut);
        });
        CrudUtilities.MainWrapElement.on('click', '#removeApprovals', function () {
            instance.RemoveAll();
        });
    };

    ManageStudentApprovalPage.prototype.SetupModals = function () {
        var instance = this;

        CrudUtilities.WireupCrudModal(this._StudentApprovalDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#removeApprovalsBySchool', '/StudentApproval/RemoveAllProvidersBySchool/', false, 'Select & Remove Provider Approvals', 'Remove Selected Provider Approvals', '#form-remove-all-providers', function () {
            var selectSchoolsField = $('#SelectedSchools');
            selectSchoolsField.bsmSelect({
                addItemTarget: 'top'
            });
        }, function () {
            $.ajax({
                url: '/StudentApproval/CountStudentsWithApprovedProviders',
                type: 'POST',
                dataType: 'json',
                success: function (result) {
                    $('#lblApprovalCount').text(result);
                }
            });
        }, null);

        CrudUtilities.WireupCrudModal(this._StudentApprovalDataTable.DataTableElement, CrudUtilities.MainWrapSelector, '#studentApprovals tr .remove', '/StudentApproval/RemoveProvider/', true, 'Remove Provider', 'Remove', '#form-addedit', function () {
            CrudUtilities.SubmitButton.removeClass('btn-primary').addClass('btn-danger');
        }, function () {
            $.ajax({
                url: '/StudentApproval/CountStudentsWithApprovedProviders',
                type: 'POST',
                dataType: 'json',
                success: function (result) {
                    $('#lblApprovalCount').text(result);
                }
            });
        }, null);
    };

    ManageStudentApprovalPage.prototype.AddProviders = function (id, name) {
        CrudUtilities.DisplayCrudModal(this._StudentApprovalDataTable.DataTableElement, '/StudentApproval/AddProviders/' + id, '/StudentApproval/AddProviders', 'Add Approved Provider(s) for ' + name, 'Add Approved Provider(s)', '#form-student-provider', function () {
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        }, function () {
            $.ajax({
                url: '/StudentApproval/CountStudentsWithApprovedProviders',
                type: 'POST',
                dataType: 'json',
                success: function (result) {
                    $('#lblApprovalCount').text(result);
                }
            });
        }, null);
    };

    ManageStudentApprovalPage.prototype.UpdateOptOut = function (id, hasParentalOptOut) {
        var instance = this;
        var url = '/StudentApproval/SetOptOut/' + id;
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
    };

    ManageStudentApprovalPage.prototype.RemoveAll = function () {
        var instance = this;
        var htmlString = '<form id="form-removeAll"><ul class="form">' + '<li id="SelectedUserIds" class="clearfix">Are you sure you wish to remove all student provider approvals?</li></ul></form>';
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
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                    CrudUtilities.SubmitButton.removeAttr("disabled");
                }
            });
        });
    };
    return ManageStudentApprovalPage;
})(ManagePage);

$(document).ready(function () {
    var studentApprovalDataTable = new StudentApprovalDataTable();
    new ManageStudentApprovalPage(studentApprovalDataTable).Initialize();
});
//# sourceMappingURL=Manage.js.map
