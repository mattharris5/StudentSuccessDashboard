/// <reference path="../ssd.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="AccessAuditDataTable.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var AccessAuditPage = (function (_super) {
    __extends(AccessAuditPage, _super);
    function AccessAuditPage() {
        _super.call(this);
        this._AccessAuditDataTable = new AccessAuditDataTable();
        this.SetupModals();
    }
    AccessAuditPage.prototype.Initialize = function () {
        this._AccessAuditDataTable.Initialize(null);
    };

    AccessAuditPage.prototype.SetupModals = function () {
        var instance = this;

        $(CrudUtilities.MainWrapSelector).on('click', '#accessAudit tr .associations', function () {
            var parser = new DOMParser();
            var index = $(this).data('value');
            var xmlDoc = parser.parseFromString(AccessAuditPage.accessDataArray[index], "text/xml");
            var html = "";
            var x = xmlDoc.getElementsByTagName("providers");
            if (x[0].childNodes[0] != undefined) {
                html += "<h4>Providers</h4><ul>";
                for (var i = 0; i < x[0].childNodes.length; i++) {
                    html += "<li>";
                    html += x[0].childNodes[i].attributes["name"].value;
                    html += "</li>";
                }
                html += "</ul>";
            }

            x = xmlDoc.getElementsByTagName("schools");
            if (x[0].childNodes[0] != undefined) {
                html += "<h4>Schools</h4><ul>";
                for (var i = 0; i < x[0].childNodes.length; i++) {
                    html += "<li>";
                    html += x[0].childNodes[i].attributes["name"].value;
                    html += "</li>";
                }
                html += "</ul>";
            }

            CrudUtilities.ModalElement.modal({ show: true });
            CrudUtilities.ModalBodyElement.html(html);
            CrudUtilities.ModalTitleElement.html('Associations');
            CrudUtilities.CancelButton.text('Close');
            CrudUtilities.SubmitButton.hide();
            CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
                CrudUtilities.SubmitButton.removeAttr("disabled");
                CrudUtilities.ModalElement.off('hidden');
                CrudUtilities.SubmitButton.off('click');
            }).on('show', CrudUtilities.ModalSelector, function () {
                $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
            });
        });
    };
    AccessAuditPage.accessDataArray = new Array();
    return AccessAuditPage;
})(ManagePage);

$(document).ready(function () {
    new AccessAuditPage().Initialize();
});
//# sourceMappingURL=AccessAudit.js.map
