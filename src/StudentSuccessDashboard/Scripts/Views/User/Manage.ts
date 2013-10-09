/// <reference path="../ManagePage.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="UserFilter.ts" />
/// <reference path="UserSelector.ts" />
/// <reference path="UserDataTable.ts" />

class UserManagePage extends ManagePage {
    private _UserFilter: IFilter;
    private _UserSelector: ISelector;
    private _UserDataTable: IDataTable;

    constructor() {
        super();
        this._UserFilter = new UserFilter();
        this._UserSelector = new UserSelector();
        this._UserDataTable = new UserDataTable(this._UserSelector);
    }

    Initialize(): void {
        this._UserDataTable.Initialize(this._UserFilter);
        this.SetupControls();
    }

    SetupControls(): void {
        var instance: UserManagePage = this;
        $('#modal').addClass('userManageModal');
        var setupTooltips: any = $("[rel=tooltip]");
        setupTooltips.tooltip();
        $('.dropdown-toggle').dropdown();
        instance._UserFilter.Initialize(instance._UserDataTable.DataTableElement);
        var multiSelect: any = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top',
        });
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
            CrudUtilities.SubmitButton.off('click');
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(CrudUtilities.SubmitButtonSelector + ',' + CrudUtilities.CancelButtonSelector).show();
        });
        CrudUtilities.MainWrapElement.on('click', '#users tr .user-associations', function () {
            var value = $(this).data('value');
            $.get('/User/UserAssociations/' + value, function (data) {
                CrudUtilities.ModalElement.modal({ show: true });
                CrudUtilities.ModalBodyElement.html(data);
                CrudUtilities.ModalTitleElement.text('Associations for ' + $('#UserName').val());
                CrudUtilities.SubmitButton.hide();
                CrudUtilities.CancelButton.text('Close');
            });
        });
        CrudUtilities.MainWrapElement.on('change', 'input:checkbox', function () {
            instance._UserSelector.Remove($(this).data('value'));
            if ($(this).is(':checked')) {
                $(this).closest('tr').addClass('row_selected');
                instance._UserSelector.Add($(this).data('value'));
            }
            else {
                $(this).closest('tr').removeClass('row_selected');
                $('#chkSelectAll').removeAttr('checked');
            }
        });
        $("#chkSelectAll").click(function () {
            $(this).parents('table').find(':checkbox').attr('checked', this.checked);
            if (this.checked) {
                //need to populate with all IDs
                instance._UserSelector.UpdateSelections(true, instance._UserFilter.ToJson());
            }
            else {
                instance._UserSelector.Reset();
            }
        });
        CrudUtilities.MainWrapElement.on('click', '.btnReactivate, .btnDeactivate', function () {
            instance.ToggleActivation($(this).data('value'), $(this).is('.btnReactivate'))


        });
        CrudUtilities.MainWrapElement.on('click', '.activateUsers, .deactivateUsers', function () {
            var ActivationString = $(this).data('name');
            var ActiveStatus = $(this).is('.activateUsers');

            CrudUtilities.DisplayCrudModal(instance._UserDataTable.DataTableElement, '/User/MultiUserActivation?activationString=' + ActivationString +
                '&activeStatus=' + ActiveStatus,
                '/User/MultiToggleActivation', 'User Activation', 'Submit', '#form-activateUsers',
                function () {
                    instance._UserSelector.AddHiddenSelectedElements();
                    var newInput = $("<input type='hidden' value='" + ActiveStatus + "' />").attr("id", "activeStatus").attr("name", "activeStatus");
                    $('li#SelectedUserIds').after(newInput);
                }, function () {
                    $('#chkSelectAll').removeAttr('checked');
                    instance._UserSelector.Reset();
                    instance._UserDataTable.Refresh();
                }, function () {
                    var multiSelect: any = $("select[multiple]");
                    multiSelect.bsmSelect({ addItemTarget: 'top', });
                }
            );
        });
        CrudUtilities.MainWrapElement.on('click', '.createActivation, .editUser', function () {
            instance.ManageUserRoles($(this).data('value'),
                $(this).is('.createActivation') ? 'Save Activation' : 'Update Roles',
                $(this).is('.createActivation') ? 'Create New Activation for' : 'Edit User Roles for',
                $(this).is('.createActivation') ? '/User/CreateRole' : '/User/EditRole');
        });
    }

    SetupShow(): void {
        var show = {
            schools: false,
            providers: false
        };
        var radioButtons: JQuery = $('#form-UserActivation input[name=PostedRoles]');
        $.each(radioButtons, function () {
            if (this.checked === true) {
                var radioButtonLabel: JQuery = $("label[for='" + $(this).attr('id') + "']");
                switch (radioButtonLabel.text()) {
                    case "Site Coordinator":
                        show.schools = true;
                        show.providers = false;
                        break;
                    case "Provider":
                        show.providers = true;
                        show.schools = false;
                        break;
                }
            }
        });
        if (show.schools) {
            $('#schoolSelection').slideDown('fast');
            this.SetupGroupSelection();
        } else {
            $('#schoolSelection').slideUp('fast');
            $('#no_box').slideUp('fast');
        }
        if (show.providers) {
            $('#providerSelection').slideDown('fast');
        } else {
            $('#providerSelection').slideUp('fast');
        }
    }

    SetupGroupSelection(): void {
        $.each($("input[name='group_name']"), function () {
            if ($(this).attr("checked") === "checked") {
                var selected_radio_value = $(this).val();
                if (selected_radio_value == 'Yes') {
                    $("#yes_box").show("slow");
                    $("#no_box").hide("slow");
                }
                else if (selected_radio_value == 'No') {
                    $("#no_box").show("slow");
                    $("#yes_box").hide("slow");
                }
                return false;
            }
        });
    }

    ToggleActivation(value: string, activeFlag: boolean): void {
        var instance = this;
        var url = '/User/ToggleActivation';
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,
            data: {
                Id: value,
                activeStatus: activeFlag
            },
            success: function (result) {
                instance._UserDataTable.Refresh();
            },
            error: function (result) {
                // TODO: do something....
            }
        });
    }

    ManageUserRoles(userId: string, submitButtonText: string, title: string, postUrl: string): void {
        var instance: UserManagePage = this;
        var getUrl = postUrl + '/' + userId;
        $.get(getUrl, function (data) {
            CrudUtilities.ModalElement.modal({ show: true });
            CrudUtilities.ModalBodyElement.html(data);
            $("#yes_box").hide();
            $("#no_box").hide();
            title = title + ' ' + $("#Name").val();
            $('#UserId').val(userId);
            $("#radAllSchools").attr('checked', 'checked');
            $("#radSelSchools").removeAttr('checked');
            if (!($("#SelectedSchoolIds option:not(:selected)").length == 0)) {
                $.each($("#SelectedSchoolIds option"), function () {
                    if ($(this).attr('selected') == "selected") {
                        $("#radSelSchools").attr('checked', 'checked');
                        $("#radAllSchools").removeAttr('checked');
                        return false;
                    }
                });
            }
            instance.SetupShow();
            instance.SetupGroupSelection();
            CrudUtilities.MainWrapElement.on('change', "input[name=PostedRoles]", function () {
                instance.SetupShow();
            });
            CrudUtilities.MainWrapElement.on('change', 'input[name=group_name]', function () {
                instance.SetupGroupSelection();
            });
            CrudUtilities.ModalTitleElement.html(title);
            CrudUtilities.SubmitButton.text(submitButtonText);
            var multiSelect: any = $("select[multiple]");
            CrudUtilities.MainWrapElement.on('click', CrudUtilities.SubmitButtonSelector, function (event) {
                event.preventDefault();
                CrudUtilities.SubmitButton.prop('disabled', true);
                if ($("#radAllSchools").is(':checked')) {
                    $.each($("#SelectedSchoolIds option:selected"), function () {
                        $(this).removeAttr('selected');
                    });
                    $("#allSchoolsSelected").attr("value", "true");
                }
                else {
                    $("#allSchoolsSelected").attr("value", "false");
                }
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: postUrl,
                    data: $('#form-UserActivation').serialize(),
                    success: function (result) {
                        CrudUtilities.MainWrapElement.off('click', CrudUtilities.SubmitButtonSelector);
                        CrudUtilities.SubmitButton.removeAttr("disabled");
                        CrudUtilities.ModalElement.modal('hide');
                        instance._UserSelector.Reset();
                        instance._UserDataTable.Refresh();
                    },
                    error: function (result) {
                        CrudUtilities.ModalBodyElement.html(result.responseText);
                        multiSelect.bsmSelect({
                            addItemTarget: 'top',
                        });
                        CrudUtilities.SubmitButton.removeAttr("disabled");
                    }
                });
            });
            CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                CrudUtilities.ModalElement.off('hidden');
                CrudUtilities.SubmitButton.off('click');
                CrudUtilities.SubmitButton.removeAttr("disabled");
            });
            instance._UserFilter.Reset();
            multiSelect.bsmSelect({
                addItemTarget: 'top',
            });
        });
    }
}

$(document).ready(function () {
    new UserManagePage().Initialize();
});
