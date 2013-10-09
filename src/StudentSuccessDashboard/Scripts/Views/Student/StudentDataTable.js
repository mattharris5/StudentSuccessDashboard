/// <reference path="../DataTable.ts" />
/// <reference path="StudentSelector.ts" />
/// <reference path="StudentFilter.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentDataTable = (function (_super) {
    __extends(StudentDataTable, _super);
    function StudentDataTable(studentSelector) {
        _super.call(this, $('#students'));
        this._StudentSelector = studentSelector;
    }
    StudentDataTable.prototype.GetServerDataUrl = function () {
        return "/Student/DataTableAjaxHandler";
    };

    StudentDataTable.prototype.GetPrintExportColumnIndices = function () {
        return [0, 1, 2, 3, 4, 5];
    };

    StudentDataTable.prototype.CreateColumns = function () {
        var columns = [
            {
                "sName": "ServiceRequest",
                "bSearchable": false,
                "bSortable": false,
                "fnRender": this.RenderServiceRequest
            },
            {
                "sName": "ID"
            },
            {
                "sName": "Name",
                "fnRender": this.RenderName
            },
            {
                "sName": "Grade",
                "sClass": "StudentGrade"
            },
            {
                "sName": "SchoolName"
            },
            {
                "sName": "ServiceOfferings",
                "bSearchable": false,
                "bSortable": false,
                "fnRender": this.RenderServiceOfferings
            },
            {
                "sName": "Actions",
                "bSearchable": false,
                "bSortable": false,
                "fnRender": this.RenderActions
            }
        ];
        return columns;
    };

    StudentDataTable.prototype.DrawCallback = function (oSettings) {
        this._StudentSelector.UpdateSelections($(CrudUtilities.MainWrapSelector + '#chkSelectAll').is(':checked'), this._Filter.ToJson());
        this._StudentSelector.CheckSelectedElements($(CrudUtilities.MainWrapSelector + '#tbodyStudents').find(':checkbox'));
        $('.DTTT_container').show();
        if (DataTable.IsInPrintExportMode) {
            $('td:has(input[type="checkbox"])').hide();
            $('th:has(input[type="checkbox"])').hide();
            $('#FilterHeader').show();
            var firstName = $('#studentFirstName').val();
            var lastName = $('#studentLastName').val();
            var id = $('#studentID').val();
            var gradeFilters = $('#bsmListbsmContainer0 .bsmListItemLabel').get();
            var schoolFilters = $('#bsmListbsmContainer1 .bsmListItemLabel').get();
            var serviceRequestFilters = $('#bsmListbsmContainer2 .bsmListItemLabel').get();
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
            if (gradeFilters.length > 0) {
                filterHeaderHtml += "<h3>Grades: ";
                for (var i = 0; i < gradeFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += gradeFilters[i].textContent;
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
            if (serviceRequestFilters.length > 0) {
                filterHeaderHtml += "<h3>Service Requests: ";
                for (var i = 0; i < serviceRequestFilters.length; i++) {
                    if (i != 0) {
                        filterHeaderHtml += ", ";
                    }
                    filterHeaderHtml += serviceRequestFilters[i].textContent;
                }
                filterHeaderHtml += "</h3>";
            }
            $('#FilterHeader').html(filterHeaderHtml);
        } else {
            $('th:has(input[type="checkbox"])').show();
            $('#FilterHeader').hide();
        }
    };

    StudentDataTable.prototype.GetTableLanguage = function () {
        return {
            "sLengthMenu": "_MENU_ records per page <span class='legend'><sup><i class='icon-asterisk'></i></sup><strong>Service Request Priority:</strong>" + "&nbsp;<i class='icon-sign-blank red' title='High'></i>=High &nbsp;<i class='icon-sign-blank orange' title='Medium'></i>=Medium &nbsp;" + "<i class='icon-sign-blank green' title='Low'></i>=Low  &nbsp;<i class='icon-sign-blank grey' title='None'></i>=None</span>"
        };
    };

    StudentDataTable.prototype.RenderServiceRequest = function (oObj) {
        var values = oObj.aData[0].split("|");
        var valList = "";
        if (values.length > 1) {
            valList = '<ul class="flags">';
            for (var i = 0; i < values.length; i++) {
                if (values[i] == "Y") {
                    var outputButton = "";
                    var priority = "";
                    if (DataTable.IsInPrintExportMode) {
                        outputButton = '<li>{1}{2}</li>';
                    } else {
                        outputButton = '<li><div class="btn-group">' + '<a class="btn btn-mini {0} dropdown-toggle" data-toggle="dropdown">{1}{2}{4}<span class="caret"></span></a>' + '<ul class="dropdown-menu pull-right"><li><a href="#" class="editServiceRequest" data-value="{3}"><i class="icon-edit"></i> Edit This Service Request</a></li>' + '<li><a href="#" class="removeServiceRequest" data-value="{3}"><i class="icon-remove"></i> Remove This Service Request</a></li></ul></div></li>';
                        priority = StudentDataTable.GetPriorityButtonStyleClass(values[i + 3]);
                    }
                    valList += StringExtensions.format(outputButton, priority, values[i + 2], values[i + 4].length > 0 ? "/" + values[i + 4] : "", values[i + 1], values[i + 5] == "2" ? '<i class="icon-ok FulfillmentStatus"></i>' : values[i + 5] == "3" ? '<i class="icon-remove FulfillmentStatus"></i>' : "");
                } else if (values[i] == "N") {
                    var outputButton = "";
                    if (DataTable.IsInPrintExportMode) {
                        outputButton = '<li>{1}{2}</li>';
                    } else {
                        outputButton = '<li><div class="btn-group">' + '<a class="btn btn-mini {0} dropdown-toggle" data-toggle="dropdown">{1}{2}{4}<span class="caret"></span></a>' + '<ul class="dropdown-menu pull-right"><li><a href="#" class="editServiceRequest" data-value="{3}"><i class="icon-edit"></i> Edit This Service Request</a></li>' + '</ul></div></li>';
                    }
                    var priority = StudentDataTable.GetPriorityButtonStyleClass(values[i + 3]);
                    valList += StringExtensions.format(outputButton, priority, values[i + 2], values[i + 4].length > 0 ? "/" + values[i + 4] : "", values[i + 1], values[i + 5] == "2" ? '<i class="icon-ok FulfillmentStatus"></i>' : values[i + 5] == "3" ? '<i class="icon-remove FulfillmentStatus"></i>' : "");
                }
                i = i + 5;
            }
            valList += "</ul>";
        }
        return valList;
    };

    StudentDataTable.GetPriorityButtonStyleClass = function (value) {
        if (value == "2")
            return "btn-success";
else if (value == "3")
            return "btn-warning";
else if (value == "4")
            return "btn-danger";
        return "";
    };

    StudentDataTable.prototype.RenderName = function (oObj) {
        var values = oObj.aData[2].split("|");
        if (values.length > 1) {
            return StringExtensions.format('<a href="/Student/Details/{1}"><strong>{0}</strong></a>', values[0], values[1]);
        }
        return StringExtensions.format('<strong>{0}</strong>', values[0]);
    };

    StudentDataTable.BuildServiceOfferingsFormatString = function (displayCode) {
        if (displayCode === "Y") {
            if (DataTable.IsInPrintExportMode) {
                return "<li>{0}</li>";
            }
            return "<li><div class='btn-group'><a class='btn btn-mini dropdown-toggle' data-toggle='dropdown' href='#'>{0} <span class='caret'></span></a>" + "<ul class='dropdown-menu pull-right'><li><a href='#' class='editAssignedServiceOffering' data-toggle='modal' data-value='{1}'><i class='icon-edit'></i> Edit This Service Offering</a></li>" + "<li><a href='#' class='removeAssignedServiceOffering' data-toggle='modal' data-value='{1}'><i class='icon-remove'></i> Remove This Service Offering</a></li>" + "<li><a href='/ServiceAttendance/Index/{1}'><i class='icon-eye-open'></i> View Attendance for this Service Offering</a></li></ul></div></li>";
        }
        if (displayCode == "N") {
            "<li><div class='btn-group'><a class='btn btn-mini disabled' href='#'>{0}</a></div></li>";
        }
        return "";
    };

    StudentDataTable.prototype.RenderServiceOfferings = function (oObj) {
        var values = oObj.aData[5].split("|");
        var valList = "";
        if (values.length > 1) {
            valList = "<ul class='assigned-offerings'>";
            for (var i = 0; i < values.length; i++) {
                var outputButtons = StudentDataTable.BuildServiceOfferingsFormatString(values[i]);
                valList += StringExtensions.format(outputButtons, values[i + 2], values[i + 1]);
            }
            valList += "</ul>";
        }
        return valList;
    };

    StudentDataTable.prototype.RenderActions = function (oObj) {
        if (oObj.aData.length > 6) {
            if (oObj.aData[6] != undefined && oObj.aData[6] != null && oObj.aData[6] != "") {
                var value = oObj.aData[6];
                if (value != null && value.length > 0) {
                    return StringExtensions.format('<input type="checkbox" data-value="{0}" />', value);
                }
            }
        }
        return "";
    };
    return StudentDataTable;
})(DataTable);
//# sourceMappingURL=StudentDataTable.js.map
