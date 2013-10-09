/// <reference path="../ManagePage.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../ssd.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var UserProfilePage = (function (_super) {
    __extends(UserProfilePage, _super);
    function UserProfilePage() {
        _super.call(this);
        this._FirstName = $('#FirstName');
        this._LastName = $('#LastName');
        this._DisplayName = $('#DisplayName');
        this._EmailAddress = $('#PendingEmail');
    }
    UserProfilePage.prototype.Initialize = function () {
        this._FirstName.suggest({ text: "Anonymous" });
        this._LastName.suggest({ text: "Anonymous" });
        this._DisplayName.suggest({ text: "Anonymous" });
        this._EmailAddress.suggest({ text: "Anonymous@sample.com" });
        this.SetupModals();
    };

    UserProfilePage.prototype.SetupModals = function () {
        var instance = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(null, CrudUtilities.MainWrapSelector, '#eulaBtn', '/Agreement/UserEula/', true, 'EULA', 'EULA', '#modalBody', function () {
            CrudUtilities.SubmitButton.hide();
            CrudUtilities.CancelButton.text('Close');
        }, null, null);
    };
    return UserProfilePage;
})(ManagePage);

$(document).ready(function () {
    new UserProfilePage().Initialize();
});
//# sourceMappingURL=UserProfile.js.map
