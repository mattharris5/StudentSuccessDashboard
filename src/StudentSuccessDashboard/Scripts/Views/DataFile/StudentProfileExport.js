/// <reference path="../ManagePage.ts" />
/// <reference path="../StringExtensions.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentProfileExportPage = (function (_super) {
    __extends(StudentProfileExportPage, _super);
    function StudentProfileExportPage() {
        _super.call(this);
    }
    StudentProfileExportPage.prototype.Initialize = function () {
        this.SetupModals();
    };

    StudentProfileExportPage.prototype.SetupModals = function () {
        var instance = this;
        $('#btnSubmit').attr('disabled', 'disabled');
        $('#btnSubmit').click(function () {
            $('#school-filter-selections a').remove();
            $('#grade-filter-selections a').remove();
            $('#standard-field-selections a').remove();
            $('#service-type-field-selections a').remove();
            $('#custom-field-selections a').remove();

            CrudUtilities.ModalElement.modal({ show: true });
            CrudUtilities.ModalBodyElement.html("Processing file...");
            CrudUtilities.ModalTitleElement.html('Processing');
            CrudUtilities.SubmitButton.text('Return Home');
            CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
            $('#btnClear').hide();
            $('#btnCancelModal').hide();
            $('#btnSubmit').hide();
            CrudUtilities.SubmitButton.on('click', function (event) {
                window.location.href = '/';
            });
            CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                CrudUtilities.SubmitButton.removeAttr("disabled");
                CrudUtilities.ModalElement.off('hidden');
                CrudUtilities.SubmitButton.off('click');
            }).on('show', CrudUtilities.ModalSelector, function () {
                $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
            });
        });
        $('#school-filter-selections a').click(function () {
            var getUrl = '/School/Selector';
            $.get(getUrl, function (data) {
                var dataString = data;
                if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                    if ($("#btnClear").get().length == 0) {
                        $('.modal-footer').prepend('<button id="btnClear" class="btn btn-link">Clear Selections</button>');
                    }
                    $("#btnClear").on('click', function (event) {
                        instance.ClearSelections();
                    });
                    CrudUtilities.ModalElement.modal({ show: true });
                    CrudUtilities.ModalBodyElement.html(data);
                    CrudUtilities.ModalTitleElement.html('Schools');
                    CrudUtilities.SubmitButton.text('Submit');
                    CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
                    var existingSelections = $('#schoolsList div').get();
                    for (var i = 0; i < existingSelections.length; i++) {
                        var existingSelectionId = existingSelections[i].attributes[0].value;
                        $('#SelectedSchoolIds option[value="' + existingSelectionId + '"]').attr('selected', 'selected');
                    }
                    CrudUtilities.SubmitButton.on('click', function (event) {
                        var selected = $('#SelectedSchoolIds option:selected');
                        var container = $('#schoolsList');
                        event.preventDefault();
                        if (selected.length > 0) {
                            instance.UpdateSelections(selected, container, "SelectedSchoolIds");
                        } else {
                            container.html('<h5 class="muted"><em>You haven\'t added any filters</em></h5>');
                        }
                        instance.EnableSubmitButton();
                        CrudUtilities.ModalElement.modal('hide');
                    });
                    CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                        CrudUtilities.SubmitButton.removeAttr("disabled");
                        CrudUtilities.ModalElement.off('hidden');
                        CrudUtilities.SubmitButton.off('click');
                    }).on('show', CrudUtilities.ModalSelector, function () {
                        $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
                    });
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                    $('.bsmSelect').attr('multiple', 'multiple');
                    $('.bsmSelect').addClass('selectorBsmSelectWithFilter');
                    $('.bsmList').addClass('selectorBsmList');
                    $('.selectorBsmSelectWithFilter option[value=""]').click(function () {
                        instance.SelectAll("SelectedSchoolIds", "selectorBsmSelectWithFilter");
                    });
                    $('.selectorBsmSelectWithFilter option[value!=""]').click(function () {
                        $('.selectorBsmSelectWithFilter option[value=""]').removeAttr('selected');
                    });
                    $('.selectorBsmSelectWithFilter option[value=""]').removeAttr('selected');

                    $("#txtFilter").keyup(function () {
                        instance.FilterOptions();
                    });
                } else {
                    $(document.body).html(data);
                }
            });
        });
        $('#grade-filter-selections a').click(function () {
            var getUrl = '/Grade/Selector';
            $.get(getUrl, function (data) {
                var dataString = data;
                if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                    if ($("#btnClear").get().length == 0) {
                        $('.modal-footer').prepend('<button id="btnClear" class="btn btn-link">Clear Selections</button>');
                    }
                    $("#btnClear").on('click', function (event) {
                        instance.ClearSelections();
                    });
                    CrudUtilities.ModalElement.modal({ show: true });
                    CrudUtilities.ModalBodyElement.html(data);
                    CrudUtilities.ModalTitleElement.html('Grade Level');
                    CrudUtilities.SubmitButton.text('Submit');
                    CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
                    var existingSelections = $('#gradesList div').get();
                    for (var i = 0; i < existingSelections.length; i++) {
                        var existingSelectionId = existingSelections[i].attributes[0].value;
                        $('#SelectedGrades option[value="' + existingSelectionId + '"]').attr('selected', 'selected');
                    }
                    CrudUtilities.SubmitButton.on('click', function (event) {
                        var selected = $('#SelectedGrades option:selected');
                        var container = $('#gradesList');
                        event.preventDefault();
                        if (selected.length > 0) {
                            instance.UpdateSelections(selected, container, "SelectedGrades");
                        } else {
                            container.html('<h5 class="muted"><em>You haven\'t added any filters</em></h5>');
                        }
                        instance.EnableSubmitButton();
                        CrudUtilities.ModalElement.modal('hide');
                    });
                    CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                        CrudUtilities.SubmitButton.removeAttr("disabled");
                        CrudUtilities.ModalElement.off('hidden');
                        CrudUtilities.SubmitButton.off('click');
                    }).on('show', CrudUtilities.ModalSelector, function () {
                        $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
                    });
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                    $('.bsmSelect').attr('multiple', 'multiple');
                    $('.bsmSelect').addClass('selectorBsmSelect');
                    $('.bsmList').addClass('selectorBsmList');
                    $('.selectorBsmSelect option[value=""]').click(function () {
                        instance.SelectAll("SelectedGrades", "selectorBsmSelect");
                    });
                    $('.selectorBsmSelect option[value!=""]').click(function () {
                        $('.selectorBsmSelect option[value=""]').removeAttr('selected');
                    });
                    $('.selectorBsmSelect option[value=""]').removeAttr('selected');

                    $("#txtFilter").keyup(function () {
                        instance.FilterOptions();
                    });
                } else {
                    $(document.body).html(data);
                }
            });
        });
        $('#standard-field-selections a').click(function () {
            var modalDom = '<select id="SelectedStandardFieldIds" multiple="multiple" name="SelectedStandardFieldIds" title="All">' + '<option value="DOB">Date of Birth</option>' + '<option value="ParentName">Parent\'s Name</option>' + '</select>';
            if ($("#btnClear").get().length == 0) {
                $('.modal-footer').prepend('<button id="btnClear" class="btn btn-link">Clear Selections</button>');
            }
            $("#btnClear").on('click', function (event) {
                instance.ClearSelections();
            });
            CrudUtilities.ModalElement.modal({ show: true });
            CrudUtilities.ModalBodyElement.html(modalDom);
            CrudUtilities.ModalTitleElement.html('Standard Information Fields');
            CrudUtilities.SubmitButton.text('Submit');
            CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
            var existingSelections = $('#standardFieldsList div').get();
            for (var i = 0; i < existingSelections.length; i++) {
                var existingSelectionId = existingSelections[i].attributes[0].value;
                $('#SelectedStandardFieldIds option[value="' + existingSelectionId + '"]').attr('selected', 'selected');
            }
            CrudUtilities.SubmitButton.on('click', function (event) {
                var selected = $('#SelectedStandardFieldIds option:selected');
                var container = $('#standardFieldsList');
                event.preventDefault();
                if (selected.length > 0) {
                    instance.UpdateStandardFields(selected, container);
                } else {
                    container.html('<h5 class="muted"><em>You haven\'t added any fields</em></h5>');
                }
                instance.EnableSubmitButton();
                CrudUtilities.ModalElement.modal('hide');
            });
            CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                CrudUtilities.SubmitButton.removeAttr("disabled");
                CrudUtilities.ModalElement.off('hidden');
                CrudUtilities.SubmitButton.off('click');
            }).on('show', CrudUtilities.ModalSelector, function () {
                $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
            });
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
            $('.bsmSelect').attr('multiple', 'multiple');
            $('.bsmSelect').addClass('selectorBsmSelect');
            $('.bsmList').addClass('selectorBsmList');
            $('.selectorBsmSelect option[value=""]').click(function () {
                instance.SelectAll("SelectedStandardFieldIds", "selectorBsmSelect");
            });
            $('.selectorBsmSelect option[value!=""]').click(function () {
                $('.selectorBsmSelect option[value=""]').removeAttr('selected');
            });
            $('.selectorBsmSelect option[value=""]').removeAttr('selected');

            $("#txtFilter").keyup(function () {
                instance.FilterOptions();
            });
        });
        $('#service-type-field-selections a').click(function () {
            var getUrl = '/ServiceType/Selector';
            $.get(getUrl, function (data) {
                var dataString = data;
                if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                    if ($("#btnClear").get().length == 0) {
                        $('.modal-footer').prepend('<button id="btnClear" class="btn btn-link">Clear Selections</button>');
                    }
                    $("#btnClear").on('click', function (event) {
                        instance.ClearSelections();
                    });
                    CrudUtilities.ModalElement.modal({ show: true });
                    CrudUtilities.ModalBodyElement.html(data);
                    CrudUtilities.ModalTitleElement.html('Service Type Fields');
                    CrudUtilities.SubmitButton.text('Submit');
                    CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
                    var existingSelections = $('#serviceTypesList div').get();
                    for (var i = 0; i < existingSelections.length; i++) {
                        var existingSelectionId = existingSelections[i].attributes[0].value;
                        $('#SelectedServiceTypeIds option[value="' + existingSelectionId + '"]').attr('selected', 'selected');
                    }
                    CrudUtilities.SubmitButton.on('click', function (event) {
                        var selected = $('#SelectedServiceTypeIds option:selected');
                        var container = $('#serviceTypesList');
                        event.preventDefault();
                        if (selected.length > 0) {
                            instance.UpdateSelections(selected, container, "SelectedServiceTypeIds");
                        } else {
                            container.html('<h5 class="muted"><em>You haven\'t added any fields</em></h5>');
                        }
                        instance.EnableSubmitButton();
                        CrudUtilities.ModalElement.modal('hide');
                    });
                    CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                        CrudUtilities.SubmitButton.removeAttr("disabled");
                        CrudUtilities.ModalElement.off('hidden');
                        CrudUtilities.SubmitButton.off('click');
                    }).on('show', CrudUtilities.ModalSelector, function () {
                        $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
                    });
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                    $('.bsmSelect').attr('multiple', 'multiple');
                    $('.bsmSelect').addClass('selectorBsmSelect');
                    $('.bsmList').addClass('selectorBsmList');
                    $('.selectorBsmSelect option[value=""]').click(function () {
                        instance.SelectAll("SelectedServiceTypeIds", "selectorBsmSelect");
                    });
                    $('.selectorBsmSelect option[value!=""]').click(function () {
                        $('.selectorBsmSelect option[value=""]').removeAttr('selected');
                    });
                    $('.selectorBsmSelect option[value=""]').removeAttr('selected');

                    $("#txtFilter").keyup(function () {
                        instance.FilterOptions();
                    });
                } else {
                    $(document.body).html(data);
                }
            });
        });
        $('#custom-field-selections a').click(function () {
            var getUrl = '/CustomField/Selector';
            $.get(getUrl, function (data) {
                var dataString = data;
                if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                    if ($("#btnClear").get().length == 0) {
                        $('.modal-footer').prepend('<button id="btnClear" class="btn btn-link">Clear Selections</button>');
                    }
                    $("#btnClear").on('click', function (event) {
                        instance.ClearSelections();
                    });
                    CrudUtilities.ModalElement.modal({ show: true });
                    CrudUtilities.ModalBodyElement.html(data);
                    CrudUtilities.ModalTitleElement.html('Custom Fields');
                    CrudUtilities.SubmitButton.text('Submit');
                    CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
                    var existingSelections = $('#customFieldsList div').get();
                    for (var i = 0; i < existingSelections.length; i++) {
                        var existingSelectionId = existingSelections[i].attributes[0].value;
                        $('#SelectedCustomFieldIds option[value="' + existingSelectionId + '"]').attr('selected', 'selected');
                    }
                    CrudUtilities.SubmitButton.on('click', function (event) {
                        var selected = $('#SelectedCustomFieldIds option:selected');
                        var container = $('#customFieldsList');
                        event.preventDefault();
                        if (selected.length > 0) {
                            instance.UpdateSelections(selected, container, "SelectedCustomFieldIds");
                        } else {
                            container.html('<h5 class="muted"><em>You haven\'t added any fields</em></h5>');
                        }
                        instance.EnableSubmitButton();
                        CrudUtilities.ModalElement.modal('hide');
                    });
                    CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                        CrudUtilities.SubmitButton.removeAttr("disabled");
                        CrudUtilities.ModalElement.off('hidden');
                        CrudUtilities.SubmitButton.off('click');
                    }).on('show', CrudUtilities.ModalSelector, function () {
                        $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
                    });
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                    $('.bsmSelect').attr('multiple', 'multiple');
                    $('.bsmSelect').addClass('selectorBsmSelectWithFilter');
                    $('.bsmList').addClass('selectorBsmList');
                    $('.selectorBsmSelectWithFilter option[value=""]').click(function () {
                        instance.SelectAll("SelectedCustomFieldIds", "selectorBsmSelectWithFilter");
                        $('.selectorBsmSelectWithFilter option[value=""]').removeAttr('selected');
                    });
                    $('.selectorBsmSelectWithFilter option[value!=""]').click(function () {
                        $('.selectorBsmSelectWithFilter option[value=""]').removeAttr('selected');
                    });
                    $('.selectorBsmSelectWithFilter option[value=""]').removeAttr('selected');

                    $("#txtFilter").keyup(function () {
                        instance.FilterOptions();
                    });
                } else {
                    $(document.body).html(data);
                }
            });
        });
    };

    StudentProfileExportPage.prototype.EnableSubmitButton = function () {
        var schools = $('#schoolsList div').get();
        if (schools.length > 0) {
            $('#btnSubmit').removeClass('disabled');
            $('#btnSubmit').removeAttr('disabled');
        } else {
            $('#btnSubmit').addClass('disabled');
            $('#btnSubmit').attr('disabled', 'disabled');
        }
    };

    StudentProfileExportPage.prototype.SelectAll = function (idName, className) {
        var instance = this;
        $('#' + idName + ' option').attr('selected', 'selected');
        $('#' + idName).trigger('change');
        $('.' + className + ' option[value=""]').click(function () {
            instance.SelectAll(idName, className);
        });
        $('.' + className + ' option[value!=""]').click(function () {
            $('.' + className + ' option[value=""]').removeAttr('selected');
        });
    };

    StudentProfileExportPage.prototype.ClearSelections = function () {
        $('#SelectedSchoolIds option').removeAttr('selected');
        $('#SelectedCustomFieldIds option').removeAttr('selected');
        $('#SelectedServiceTypeIds option').removeAttr('selected');
        $('#SelectedStandardFieldIds option').removeAttr('selected');
        $('#SelectedGrades option').removeAttr('selected');
        $('.bsmListItem').remove();
        $('.bsmOptionDisabled').removeAttr('disabled');
        $('.bsmOptionDisabled').removeClass('bsmOptionDisabled');
    };

    StudentProfileExportPage.prototype.UpdateStandardFields = function (selections, container) {
        var selectedOptions = selections.get();
        container.html('');
        for (var i = 0; i < selectedOptions.length; i++) {
            container.append('<div value="' + selectedOptions[i].value + '">' + selectedOptions[i].text + '</div>');
            if (selectedOptions[i].value === "DOB") {
                container.append("<input type='hidden' value='true' name='BirthDateIncluded'/>");
            }
            if (selectedOptions[i].value === "ParentName") {
                container.append("<input type='hidden' value='true' name='ParentNameIncluded'/>");
            }
        }
    };

    StudentProfileExportPage.prototype.UpdateSelections = function (selections, container, selectionName) {
        var selectedOptions = selections.get();
        container.html('');
        for (var i = 0; i < selectedOptions.length; i++) {
            container.append('<div value="' + selectedOptions[i].value + '">' + selectedOptions[i].text + '</div>');
            container.append("<input type='hidden' value='" + selectedOptions[i].value + "' name='" + selectionName + "'/>");
        }
    };

    StudentProfileExportPage.prototype.FilterOptions = function () {
        var filterText = $("#txtFilter").val();
        var selectedItems = $('.selectorBsmSelectWithFilter option').get();
        for (var i = 0; i < selectedItems.length; i++) {
            if (i != 0 && selectedItems[i].text.toUpperCase().search(filterText.toUpperCase()) === -1) {
                $(selectedItems[i]).hide();
            } else if (i != 0) {
                $(selectedItems[i]).show();
            }
        }
    };
    return StudentProfileExportPage;
})(ManagePage);

$(document).ready(function () {
    new StudentProfileExportPage().Initialize();
});
//# sourceMappingURL=StudentProfileExport.js.map
