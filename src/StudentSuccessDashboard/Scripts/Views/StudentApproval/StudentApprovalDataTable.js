/// <reference path="../DataTable.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="StudentApprovalFilter.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentApprovalDataTable = (function (_super) {
    __extends(StudentApprovalDataTable, _super);
    function StudentApprovalDataTable() {
        _super.call(this, $('#studentApprovals'));
    }
    StudentApprovalDataTable.prototype.GetServerDataUrl = function () {
        return "/StudentApproval/DataTableAjaxHandler";
    };

    StudentApprovalDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2, 3, 4, 6];
    };

    StudentApprovalDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "mData": "StudentSISId"
            },
            {
                "mData": "Name"
            },
            {
                "mData": "Grade",
                "sClass": "StudentGrade"
            },
            {
                "mData": "School"
            },
            {
                "mData": "ApprovedProviders",
                "mRender": this.RenderApprovedProviders,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "Id",
                "mRender": this.RenderActions,
                "bSearchable": false,
                "bSortable": false
            },
            {
                "mData": "HasParentalOptOut",
                "mRender": this.RenderOptOut,
                "bSearchable": false,
                "bSortable": false
            }
        ];
        return columns;
    };

    StudentApprovalDataTable.prototype.DrawCallback = function (oSettings) {
        $('.DTTT_container').show();
        this.DataTableElement.fnSetColumnVis(5, !DataTable.IsInPrintExportMode);
        if (DataTable.IsInPrintExportMode) {
            $('#FilterHeader').show();
            var firstName = $('#firstName').val();
            var lastName = $('#lastName').val();
            var id = $('#id').val();
            var providerFilters = $('#bsmListbsmContainer0 .bsmListItemLabel').get();
            var schoolFilters = $('#bsmListbsmContainer1 .bsmListItemLabel').get();
            var filterHeaderHtml = "";
            if (firstName.length > 0) {
                filterHeaderHtml += "<h3>First Name: " + firstName + "</h3>";
            }
            if (lastName.length > 0) {
                filterHeaderHtml += "<h3>Last Name: " + lastName + "</h3>";
            }
            if (id.length > 0) {
                filterHeaderHtml += "<h3>ID: " + id + "</h3>";
            }
            if (providerFilters.length > 0) {
                filterHeaderHtml += "<h3>Providers: ";
                for (var i = 0; i < providerFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += providerFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            if (schoolFilters.length > 0) {
                filterHeaderHtml += "<h3>Schools: ";
                for (var i = 0; i < schoolFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += schoolFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            $('#FilterHeader').html(filterHeaderHtml);
        } else {
            $('#FilterHeader').hide();
        }
    };

    StudentApprovalDataTable.prototype.RenderApprovedProviders = function (data, type, full) {
        var approvedProvidersMarkup = "<ul>";
        var approvedProviderFormat = "<li>{1}<a class='remove' title='remove' data-value='{0}' href='#'><i class='icon icon-remove-sign red'></i></a></li>";
        for (var i = 0; i < data.length; i++) {
            approvedProvidersMarkup = approvedProvidersMarkup + StringExtensions.format(approvedProviderFormat, full.Id + '?providerId=' + data[i].Id, data[i].Name);
        }
        approvedProvidersMarkup = approvedProvidersMarkup + "</ul>";
        return approvedProvidersMarkup;
    };

    StudentApprovalDataTable.prototype.RenderActions = function (data, type, full) {
        var actionButtonsFormat = "<a class='btn btn-primary btn-mini addProviders' role='button' data-toggle='modal' data-studentid=\"{0}\" data-studentname=\"{1}\"><i class='icon-plus'></i> Add Approved Provider(s)</a>";
        var actionButtonsMarkup = StringExtensions.format(actionButtonsFormat, data, full.Name);
        return actionButtonsMarkup;
    };

    StudentApprovalDataTable.prototype.RenderOptOut = function (data, type, full) {
        if (DataTable.IsInPrintExportMode) {
            return data;
        }
        var optOutMarkup = StringExtensions.format("<input class='optOut-checkbox' type='checkbox' data-value='{0}'", full.Id);
        if (data) {
            optOutMarkup = optOutMarkup + " checked='CHECKED'";
        }
        optOutMarkup = optOutMarkup + " />";
        return optOutMarkup;
    };
    return StudentApprovalDataTable;
})(DataTable);
//# sourceMappingURL=StudentApprovalDataTable.js.map
