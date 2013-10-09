/// <reference path="StringExtensions.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />

class CrudUtilities {
    public static MainWrapSelector: string = '#main-wrap';
    public static MainWrapElement: JQuery = $(CrudUtilities.MainWrapSelector);
    public static ModalSelector: string = '#modal';
    public static ModalElement: any = $(CrudUtilities.ModalSelector);
    public static ModalBodySelector: string = '#modal .modal-body';
    public static ModalBodyElement: JQuery = $(CrudUtilities.ModalBodySelector);
    public static ModalTitleElementSelector: string = '#modal-title';
    public static ModalTitleElement: JQuery = $(CrudUtilities.ModalTitleElementSelector);
    public static SubmitButtonSelector: string = '#btnSubmitModal';
    public static SubmitButton: JQuery = $(CrudUtilities.SubmitButtonSelector);
    public static CancelButtonSelector: string = "#btnCancelModal";
    public static CancelButton: JQuery = $(CrudUtilities.CancelButtonSelector);

    constructor() {
    
    }

    static DisplayCrudModal(dt: any, getUrl: string, postUrl: string, modalTitle: string, buttonText: string, formSelector: string, modalLoadedCallback: () => any, crudSucceededCallback: () => any, crudErrorCallback: () => any): void {
        $.get(getUrl, function (data) {
            var dataString: string = data;
            if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                CrudUtilities.ModalElement.modal({ show: true });
                CrudUtilities.ModalBodyElement.html(data);
                CrudUtilities.ModalTitleElement.html(modalTitle);
                CrudUtilities.SubmitButton.text(buttonText);
                CrudUtilities.SubmitButton.removeClass('btn-danger').addClass('btn-primary');
                CrudUtilities.SubmitButton.on('click', function (event) {
                    event.preventDefault();
                    CrudUtilities.RefreshList(dt, postUrl, formSelector, crudSucceededCallback, crudErrorCallback);
                });
                CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                    CrudUtilities.SubmitButton.removeAttr("disabled");
                    CrudUtilities.ModalElement.off('hidden');
                    CrudUtilities.SubmitButton.off('click');
                }).on('show', CrudUtilities.ModalSelector, function () {
                    $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
                });
                if (modalLoadedCallback != null && modalLoadedCallback != undefined) {
                    modalLoadedCallback();
                }
            }
            else {
                $(document.body).html(data);
            }
        });
    }

    static WireupCrudModal(dt: any, elementSelector: string, onClickSelector: string, url: string, appendDataValue: boolean, modalTitle, buttonText: string, formSelector: string, modalLoadedCallback: () => any, crudSucceededCallback: () => any, crudErrorCallback: () => any): void {
        $(elementSelector).on('click', onClickSelector, function () {
            var queryString: string = $(this).data('query-string');
            var extendedUrl: string = url;
            if (appendDataValue) {
                extendedUrl = url + $(this).data('value');
            }
            if (queryString !== undefined) {
                extendedUrl = extendedUrl + queryString;
            }
            CrudUtilities.DisplayCrudModal(dt, extendedUrl, url, modalTitle, buttonText, formSelector, modalLoadedCallback, crudSucceededCallback, crudErrorCallback);
        });
    }

    static RefreshList(dt: any, url: string, formSelector: string, successCallback: () => any, crudErrorCallback: () => any): void {
        var formData: string = $(formSelector).serialize();
        CrudUtilities.SubmitButton.prop('disabled', true);
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,
            data: formData,
            success: function (result) {
                CrudUtilities.ModalElement.modal('hide');
                CrudUtilities.SubmitButton.off('click');
                if (dt) {
                    dt.fnFilter();
                }
                if (successCallback !== null && successCallback !== undefined) {
                    successCallback();
                }
            },
            error: function (result: any) {
                var dataString: string = result.responseText;
                if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                    CrudUtilities.ModalBodyElement.html(result.responseText);
                    CrudUtilities.SubmitButton.removeAttr("disabled");

                    if (crudErrorCallback !== null && crudErrorCallback !== undefined) {
                        crudErrorCallback();
                    }
                }
                else {
                    $(document.body).html(dataString);
                }
            }
        });
    }

    static DisplayErrorAlert(title: string, message: string): void {
        CrudUtilities.ModalElement.modal({ show: true });
        CrudUtilities.ModalBodyElement.html(message);
        CrudUtilities.ModalTitleElement.html(title);
        CrudUtilities.SubmitButton.text('Ok');
        CrudUtilities.SubmitButton.hide();
        CrudUtilities.ModalBodyElement.on('click', function (event) {
            event.preventDefault();
            CrudUtilities.ModalElement.modal('hide');
            CrudUtilities.MainWrapElement.off('click', CrudUtilities.SubmitButtonSelector);
        }, CrudUtilities.SubmitButton);
    }
}

class ControlUtilities {
    static CreateAutocompleteEditMonitorOptions(source: string, editCallback: () => any): any {
        return {
            source: source,
            focus: function (event, ui) {
                editCallback();
            },
            select: function (event, ui) {
                editCallback();
            },
            change: function (event, ui) {
                editCallback();
            },
            close: function (event, ui) {
                editCallback();
            }
        };
    }

    static UpdateFilter(valuesSelector: string, filterSelector: string): void {
        var values = [];
        var vals = $(valuesSelector).map(
            function () {
                return $(this).data("value");
            }).get().join('|');
        $(filterSelector).text(vals);
    }
}

function AddHyperlinkScheme(url: string): string {
    if (url.toUpperCase().indexOf("HTTP://") === 0 || url.toUpperCase().indexOf("HTTPS://") === 0) {
        return url;
    }
    return "http://" + url;
}

function ExtractSelectListValues(selectList: JQuery): string {
    var items: string = "";
    $.each(selectList, function () {
        items = (items.length > 0) ? items + '|' + $(this).val() : $(this).val();
    });
    return items;
}