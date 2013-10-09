/// <reference path="StringExtensions.ts" />
/// <reference path="../typings/jquery/jquery.d.ts" />
var CrudUtilities = (function () {
    function CrudUtilities() {
    }
    CrudUtilities.DisplayCrudModal = function (dt, getUrl, postUrl, modalTitle, buttonText, formSelector, modalLoadedCallback, crudSucceededCallback, crudErrorCallback) {
        $.get(getUrl, function (data) {
            var dataString = data;
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
            } else {
                $(document.body).html(data);
            }
        });
    };

    CrudUtilities.WireupCrudModal = function (dt, elementSelector, onClickSelector, url, appendDataValue, modalTitle, buttonText, formSelector, modalLoadedCallback, crudSucceededCallback, crudErrorCallback) {
        $(elementSelector).on('click', onClickSelector, function () {
            var queryString = $(this).data('query-string');
            var extendedUrl = url;
            if (appendDataValue) {
                extendedUrl = url + $(this).data('value');
            }
            if (queryString !== undefined) {
                extendedUrl = extendedUrl + queryString;
            }
            CrudUtilities.DisplayCrudModal(dt, extendedUrl, url, modalTitle, buttonText, formSelector, modalLoadedCallback, crudSucceededCallback, crudErrorCallback);
        });
    };

    CrudUtilities.RefreshList = function (dt, url, formSelector, successCallback, crudErrorCallback) {
        var formData = $(formSelector).serialize();
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
            error: function (result) {
                var dataString = result.responseText;
                if (dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') === -1) {
                    CrudUtilities.ModalBodyElement.html(result.responseText);
                    CrudUtilities.SubmitButton.removeAttr("disabled");

                    if (crudErrorCallback !== null && crudErrorCallback !== undefined) {
                        crudErrorCallback();
                    }
                } else {
                    $(document.body).html(dataString);
                }
            }
        });
    };

    CrudUtilities.DisplayErrorAlert = function (title, message) {
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
    };
    CrudUtilities.MainWrapSelector = '#main-wrap';
    CrudUtilities.MainWrapElement = $(CrudUtilities.MainWrapSelector);
    CrudUtilities.ModalSelector = '#modal';
    CrudUtilities.ModalElement = $(CrudUtilities.ModalSelector);
    CrudUtilities.ModalBodySelector = '#modal .modal-body';
    CrudUtilities.ModalBodyElement = $(CrudUtilities.ModalBodySelector);
    CrudUtilities.ModalTitleElementSelector = '#modal-title';
    CrudUtilities.ModalTitleElement = $(CrudUtilities.ModalTitleElementSelector);
    CrudUtilities.SubmitButtonSelector = '#btnSubmitModal';
    CrudUtilities.SubmitButton = $(CrudUtilities.SubmitButtonSelector);
    CrudUtilities.CancelButtonSelector = "#btnCancelModal";
    CrudUtilities.CancelButton = $(CrudUtilities.CancelButtonSelector);
    return CrudUtilities;
})();

var ControlUtilities = (function () {
    function ControlUtilities() {
    }
    ControlUtilities.CreateAutocompleteEditMonitorOptions = function (source, editCallback) {
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
    };

    ControlUtilities.UpdateFilter = function (valuesSelector, filterSelector) {
        var values = [];
        var vals = $(valuesSelector).map(function () {
            return $(this).data("value");
        }).get().join('|');
        $(filterSelector).text(vals);
    };
    return ControlUtilities;
})();

function AddHyperlinkScheme(url) {
    if (url.toUpperCase().indexOf("HTTP://") === 0 || url.toUpperCase().indexOf("HTTPS://") === 0) {
        return url;
    }
    return "http://" + url;
}

function ExtractSelectListValues(selectList) {
    var items = "";
    $.each(selectList, function () {
        items = (items.length > 0) ? items + '|' + $(this).val() : $(this).val();
    });
    return items;
}
//# sourceMappingURL=ssd.js.map
