/// <reference path="../ManagePage.ts" />
/// <reference path="ScheduleOfferingFilter.ts" />
/// <reference path="ScheduleOfferingDataTable.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="../StringExtensions.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ScheduleOfferingPage = (function (_super) {
    __extends(ScheduleOfferingPage, _super);
    function ScheduleOfferingPage() {
        _super.call(this);
        this._ScheduleOfferingFilter = new ScheduleOfferingFilter();
        this._ScheduleOfferingDataTable = new ScheduleOfferingDataTable();
    }
    ScheduleOfferingPage.prototype.Initialize = function () {
        this._ScheduleOfferingDataTable.Initialize(this._ScheduleOfferingFilter);
        this.SetupControls();
    };

    ScheduleOfferingPage.prototype.SetupControls = function () {
        var instance = this;
        var dropDownToggleElements = $('.dropdown-toggle');
        dropDownToggleElements.dropdown();
        var multiOpenAccordionElements = $('#multiOpenAccordion');
        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top'
        });
        this._ScheduleOfferingFilter.Initialize(this._ScheduleOfferingDataTable);
        CrudUtilities.MainWrapElement.on('change', '#serviceOfferingOptions tr .favorite-checkbox', function () {
            var id = $(this).data('value');
            var url = '/ServiceOffering/SetFavorite/' + id;
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: url,
                data: { "IsFavorite": $(this).is(':checked') },
                success: function (result) {
                    if (result) {
                        instance._ScheduleOfferingDataTable.Refresh();
                        instance.UpdateFavorites();
                    }
                },
                error: function (result) {
                    alert('Could not update favorite setting due to error.');
                    instance._ScheduleOfferingDataTable.Refresh();
                }
            });
        });
        $('#favorite-assignment #Favorites').change(function () {
            var id = $(this).val();
            if (id) {
                CrudUtilities.DisplayCrudModal(null, '/Service/CreateScheduledOffering/' + id, '/Service/CreateScheduledOffering/', 'Add Service Offering to Students', 'Add Service Offering', '#form-addedit', function () {
                    $("#selectedStudents").appendTo('#form-addedit');
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                    var datepickerUI = $('.hasDatePicker');
                    datepickerUI.datepicker().css('z-index', '5000');
                }, function () {
                    var returnUrl = $('#ReturnUrl').val();
                    if (returnUrl == null || returnUrl == undefined) {
                        window.location.href = '/Student/';
                    } else {
                        window.location.href = returnUrl;
                    }
                }, function () {
                    var multiOpenAccordionElements = $('#multiOpenAccordion');
                    multiOpenAccordionElements.multiOpenAccordion({
                        active: 'none'
                    });
                    multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
                    var multiSelect = $("select[multiple]");
                    multiSelect.bsmSelect({
                        addItemTarget: 'top'
                    });
                });
            }
        });
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });
        CrudUtilities.WireupCrudModal(null, CrudUtilities.MainWrapSelector, '#serviceOfferingOptions tr .scheduleOffering', '/Service/CreateScheduledOffering/', true, 'Add Service Offering to Students', 'Add Service Offering', '#form-addedit', function () {
            $("#selectedStudents").appendTo('#form-addedit');
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
            var datepickerUI = $('.hasDatePicker');
            datepickerUI.datepicker().css('z-index', '5000');
        }, function () {
            var returnUrl = $('#ReturnUrl').val();
            if (returnUrl == null || returnUrl == undefined) {
                window.location.href = '/Student/';
            } else {
                window.location.href = returnUrl;
            }
        }, function () {
            var multiOpenAccordionElements = $('#multiOpenAccordion');
            multiOpenAccordionElements.multiOpenAccordion({
                active: 'none'
            });
            multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
            var multiSelect = $("select[multiple]");
            multiSelect.bsmSelect({
                addItemTarget: 'top'
            });
        });
    };

    ScheduleOfferingPage.prototype.UpdateFavorites = function () {
        var instance = this;
        $.get('/ServiceOffering/Favorites', function (data) {
            $('.favorites').html(data);
            $('#favorite-assignment #Favorites').on("change", function () {
                var id = $(this).val();
                if (id) {
                    CrudUtilities.DisplayCrudModal(null, '/Service/CreateScheduledOffering/' + id, '/Service/CreateScheduledOffering/', 'Add Service Offering to Students', 'Add Service Offering', '#form-addedit', function () {
                        $("#selectedStudents").appendTo('#form-addedit');
                        var multiSelect = $("select[multiple]");
                        multiSelect.bsmSelect({
                            addItemTarget: 'top'
                        });
                        var datepickerUI = $('.hasDatePicker');
                        datepickerUI.datepicker().css('z-index', '5000');
                    }, function () {
                        var returnUrl = $('#ReturnUrl').val();
                        if (returnUrl == null || returnUrl == undefined) {
                            window.location.href = '/Student/';
                        } else {
                            window.location.href = returnUrl;
                        }
                    }, function () {
                        var multiOpenAccordionElements = $('#multiOpenAccordion');
                        multiOpenAccordionElements.multiOpenAccordion({
                            active: 'none'
                        });
                        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
                        var multiSelect = $("select[multiple]");
                        multiSelect.bsmSelect({
                            addItemTarget: 'top'
                        });
                    });
                }
            });
        });
    };
    return ScheduleOfferingPage;
})(ManagePage);
;

$(document).ready(function () {
    new ScheduleOfferingPage().Initialize();
});
//# sourceMappingURL=ScheduleOffering.js.map
