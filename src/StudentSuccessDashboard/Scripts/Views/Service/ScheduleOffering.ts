/// <reference path="../ManagePage.ts" />
/// <reference path="ScheduleOfferingFilter.ts" />
/// <reference path="ScheduleOfferingDataTable.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="../StringExtensions.ts" />

class ScheduleOfferingPage extends ManagePage {
    private _ScheduleOfferingFilter: IFilter;
    private _ScheduleOfferingDataTable: IDataTable;

    constructor() {
        super();
        this._ScheduleOfferingFilter = new ScheduleOfferingFilter();
        this._ScheduleOfferingDataTable = new ScheduleOfferingDataTable();
    }

    Initialize(): void {
        this._ScheduleOfferingDataTable.Initialize(this._ScheduleOfferingFilter);
        this.SetupControls();
    }

    SetupControls(): void {
        var instance: ScheduleOfferingPage = this;
        var dropDownToggleElements: any = $('.dropdown-toggle');
        dropDownToggleElements.dropdown();
        var multiOpenAccordionElements: any = $('#multiOpenAccordion');
        multiOpenAccordionElements.multiOpenAccordion({
            active: 'none'
        });
        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
        var multiSelect: any = $("select[multiple]");
        multiSelect.bsmSelect({
            addItemTarget: 'top',
        });
        this._ScheduleOfferingFilter.Initialize(this._ScheduleOfferingDataTable);
        CrudUtilities.MainWrapElement.on('change', '#serviceOfferingOptions tr .favorite-checkbox', function () {
            var id = $(this).data('value');
            var url: string = '/ServiceOffering/SetFavorite/' + id;
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
                CrudUtilities.DisplayCrudModal(null,
                    '/Service/CreateScheduledOffering/' + id, '/Service/CreateScheduledOffering/', 'Add Service Offering to Students',
                    'Add Service Offering', '#form-addedit', function () {
                        $("#selectedStudents").appendTo('#form-addedit');
                        var multiSelect: any = $("select[multiple]");
                        multiSelect.bsmSelect({
                            addItemTarget: 'top',
                        });
                        var datepickerUI: any = $('.hasDatePicker');
                        datepickerUI.datepicker().css('z-index', '5000');
                    }, function () {
                        var returnUrl = $('#ReturnUrl').val();
                        if (returnUrl == null || returnUrl == undefined) {
                            window.location.href = '/Student/';
                        }
                        else {
                            window.location.href = returnUrl;
                        }
                    }, function () {
                        var multiOpenAccordionElements: any = $('#multiOpenAccordion');
                        multiOpenAccordionElements.multiOpenAccordion({
                            active: 'none'
                        });
                        multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
                        var multiSelect: any = $("select[multiple]");
                        multiSelect.bsmSelect({
                            addItemTarget: 'top',
                        });
                    });
            }
        });
        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });
        CrudUtilities.WireupCrudModal(null,
            CrudUtilities.MainWrapSelector, '#serviceOfferingOptions tr .scheduleOffering', '/Service/CreateScheduledOffering/', true, 'Add Service Offering to Students',
            'Add Service Offering', '#form-addedit',
            function () {
                $("#selectedStudents").appendTo('#form-addedit');
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
                var datepickerUI: any = $('.hasDatePicker');
                datepickerUI.datepicker().css('z-index', '5000');
            }, function () {
                var returnUrl = $('#ReturnUrl').val();
                if (returnUrl == null || returnUrl == undefined) {
                    window.location.href = '/Student/';
                }
                else {
                    window.location.href = returnUrl;
                }
            }, function () {
                var multiOpenAccordionElements: any = $('#multiOpenAccordion');
                multiOpenAccordionElements.multiOpenAccordion({
                    active: 'none'
                });
                multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
                var multiSelect: any = $("select[multiple]");
                multiSelect.bsmSelect({
                    addItemTarget: 'top',
                });
            });
    }

    private UpdateFavorites() {
        var instance = this;
        $.get('/ServiceOffering/Favorites', function (data) {
            $('.favorites').html(data);
            $('#favorite-assignment #Favorites').on("change", function () {
                var id = $(this).val();
                if (id) {
                    CrudUtilities.DisplayCrudModal(null,
                        '/Service/CreateScheduledOffering/' + id, '/Service/CreateScheduledOffering/', 'Add Service Offering to Students',
                        'Add Service Offering', '#form-addedit', function () {
                            $("#selectedStudents").appendTo('#form-addedit');
                            var multiSelect: any = $("select[multiple]");
                            multiSelect.bsmSelect({
                                addItemTarget: 'top',
                            });
                            var datepickerUI: any = $('.hasDatePicker');
                            datepickerUI.datepicker().css('z-index', '5000');
                        }, function () {
                            var returnUrl = $('#ReturnUrl').val();
                            if (returnUrl == null || returnUrl == undefined) {
                                window.location.href = '/Student/';
                            }
                            else {
                                window.location.href = returnUrl;
                            }
                        }, function () {
                            var multiOpenAccordionElements: any = $('#multiOpenAccordion');
                            multiOpenAccordionElements.multiOpenAccordion({
                                active: 'none'
                            });
                            multiOpenAccordionElements.multiOpenAccordion("option", "active", 'none');
                            var multiSelect: any = $("select[multiple]");
                            multiSelect.bsmSelect({
                                addItemTarget: 'top',
                            });
                        });
                }
            });
        });
    }
};

$(document).ready(function () {
    new ScheduleOfferingPage().Initialize();
});
