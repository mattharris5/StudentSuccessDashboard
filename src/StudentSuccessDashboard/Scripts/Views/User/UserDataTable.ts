/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="UserFilter.ts" />
/// <reference path="UserSelector.ts" />

class UserDataTable extends DataTable {
    private _UserSelector: ISelector;

    constructor(userSelector: ISelector) {
        super($('#users'));
        this._UserSelector = userSelector;
    }

    GetServerDataUrl(): string {
        return "/User/DataTableAjaxHandler";
    }

    GetPrintExportColumnIndices(): number[] {
        return [0, 1, 2, 3, 4, 5, 6, 7];
    }

    CreateColumns(): any[] {
        var columns: any[] = [
            {
                "mData": "Status",
                "mRender": this.RenderStatus,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "LastName"
            },
            {
                "mData": "FirstName"
            },
            {
                "mData": "LastLoginTime",
                "mRender": this.RenderLastLoginTime,
                "bSearchable": false
            },
            {
                "mData": "Email",
                "mRender": this.RenderEmail,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "Roles",
                "mRender": this.RenderRoles,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "Associations",
                "mRender": this.RenderAssociations,
                "bSearchable": false
            },
            {
                "mData": "Comments",
                "mRender": this.RenderComments,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "Status",
                "mRender": this.RenderButtons,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "Id",
                "mRender": this.RenderActions,
                "bSearchable": false,
                "bSortable": false
            }
        ];
        return columns;
    }

    DrawCallback(oSettings: any): void {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(8, !DataTable.IsInPrintExportMode);
        this.DataTableElement.fnSetColumnVis(9, !DataTable.IsInPrintExportMode);
        this._UserSelector.UpdateSelections(false, this._Filter.ToJson());
        this._UserSelector.CheckSelectedElements($('#tbodyUsers').find(':checkbox'));
        var setupTooltip: any = $("[rel=tooltip]");
        setupTooltip.tooltip();
        if (DataTable.IsInPrintExportMode) {
            $('#FilterHeader').show();
            var firstName = $('#userFirstName').val();
            var lastName = $('#userLastName').val();
            var email = $('#email').val();
            var statusFilters = $('#bsmListbsmContainer0 .bsmListItemLabel').get();
            var roleFilters = $('#bsmListbsmContainer1 .bsmListItemLabel').get();
            var associationFilters = $('#bsmListbsmContainer2 .bsmListItemLabel').get();
            var filterHeaderHtml = "";
            if (firstName.length > 0) {
                filterHeaderHtml += "<h3>First Name: " + firstName + "</h3>";
            }
            if (lastName.length > 0) {
                filterHeaderHtml += "<h3>Last Name: " + lastName + "</h3>";
            }
            if (email.length > 0) {
                filterHeaderHtml += "<h3>Email: " + email + "</h3>";
            }
            if (statusFilters.length > 0) {
                filterHeaderHtml += "<h3>Statuses: ";
                for (var i = 0; i < statusFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += statusFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            if (roleFilters.length > 0) {
                filterHeaderHtml += "<h3>Roles: ";
                for (var i = 0; i < roleFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += roleFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            if (associationFilters.length > 0) {
                filterHeaderHtml += "<h3>Associations: ";
                for (var i = 0; i < associationFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += associationFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            $('#FilterHeader').html(filterHeaderHtml);
        }
        else {
            $('#FilterHeader').hide();
        }
    }

    GetTableLanguage(): any {
        return {
            "sLengthMenu": "_MENU_ records per page <span class='legend'><sup><i class='icon-asterisk'></i></sup><strong>Status:</strong>" +
                           "&nbsp;<i class='icon-sign-blank green' title='Active'></i>=Active &nbsp;<i class='icon-sign-blank red' title='Inactive'></i>=Inactive &nbsp;" +
                           "<i class='icon-sign-blank orange' title='Awaiting New Activation'></i>=Awaiting New Activation"
        };
    }

    RenderStatus(data: any, type: string, full: any): string {
        if (DataTable.IsInPrintExportMode) {
            return data;
        }
        var valList = "";
        if (data.length > 1) {
            var buttonColor = "";
            if (data === "Active")
                buttonColor = "green";
            else if (data === "Awaiting Assignment")
                buttonColor = "orange";
            else if (data === "Inactive")
                buttonColor = "red";
            valList = StringExtensions.format('<i class="icon-sign-blank {0}" title="{1}"></i>', buttonColor, data);
        }
        return valList;
    }

    RenderLastLoginTime(data: any, type: string, full: any): string {
        if (data) {
            var formatString = '<a href="/User/LoginAudit/{0}">{1}</a>';
            var displayValue = StringExtensions.parseJsonDateTime(data).toLocaleString();
            return StringExtensions.format(formatString, full.Id, displayValue);
        }
        return data;
    }

    RenderEmail(data: any, type: string, full: any): string {
        if (DataTable.IsInPrintExportMode) {
            return data;
        }
        return StringExtensions.format('<a class="btn btn-mini" href="mailto:{0}"><i class="icon-envelope"></i></a>', data);
    }

    RenderRoles(data: any, type: string, full: any): string {
        var valList = "";
        if (data.length > 0) {
            valList = "<ul>";
            for (var i = 0; i < data.length; i++) {
                if (data[i].length > 0) {
                    valList = valList + StringExtensions.format("<li>{0}</li>", data[i]);
                }
            }
            valList = valList + "</ul>";
        }
        return valList;
    }

    RenderAssociations(data: any, type: string, full: any): string {
        var formattedValue = "";
        if (data > 0) {
            if (DataTable.IsInPrintExportMode) {
                return data;
            }
            var outputButton = "<a class='btn btn-mini user-associations' data-toggle='modal' data-value='{1}' role='button' href='#'>{0}</a>";
            formattedValue = StringExtensions.format(outputButton, data, full.Id);
        }
        return formattedValue;
    }

    RenderComments(data: any, type: string, full: any): string {
        var formattedValue = "";
        if (data) {
            var outputButton = "<i class='icon icon-comment' rel='tooltip' title='{0}'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</i>";
            formattedValue = StringExtensions.format(outputButton, data);
        }
        return formattedValue;
    }

    RenderButtons(data: any, type: string, full: any): string {
        var format = "";
        if (data === "Active") {
            format = "<ul class='buttons clearfix'><li><a class='btn btn-primary btn-mini editUser' data-toggle='modal' data-value='{0}' role='button' href='#'><i class='icon-edit'>" +
                     "</i> Edit</a></li>" +
                     "<li><a class='btn btn-mini' href='/User/AccessAudit/{0}'>Audit Logs</a></li>" +
                     "<li><a class='btn btn-danger btn-mini btnDeactivate' data-toggle='modal' data-value='{0}' role='button' href='#'><i class='icon-remove'></i> Deactivate</a></li></ul>";
        }
        else if (data === "Awaiting Assignment") {
            format = "<ul class='buttons clearfix'><li><a class='btn btn-primary btn-mini btn-warning createActivation' data-toggle='modal' data-value='{0}' role='button' href='#'>" +
                     "<i class='icon-plus'></i> Create New Activation</a></li>" +
                     "<li><a class='btn btn-mini' href='/User/AccessAudit/{0}'>Audit Logs</a></li></ul>";
        }
        else if (data === "Inactive") {
            format = "<ul class='buttons clearfix'><li><a class='btn btn-primary btn-mini editUser' data-toggle='modal' data-value='{0}' role='button' href='#'><i class='icon-edit'>" +
                     "</i> Edit</a></li>" +
                     "<li><a class='btn btn-mini' href='/User/AccessAudit/{0}'>Audit Logs</a></li>" +
                     "<li><a class='btn btn-success btn-mini btnReactivate' data-toggle='modal' data-value='{0}' role='button' href='#'><i class='icon-refresh'></i> Reactivate</a></li></ul>";
        }
        var output: string = StringExtensions.format(format, full.Id);
        return output;
    }

    RenderActions(data: any, type: string, full: any): string {
        return StringExtensions.format('<input type="checkbox" data-value="{0}" />', data);
    }
}