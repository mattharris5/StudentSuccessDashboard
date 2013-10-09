/// <reference path="../ManagePage.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../ssd.ts" />

class UserProfilePage extends ManagePage {
    private _FirstName: any;
    private _LastName: any;
    private _DisplayName: any;
    private _EmailAddress: any;
    
    constructor() {
        super();
        this._FirstName = $('#FirstName');
        this._LastName = $('#LastName');
        this._DisplayName = $('#DisplayName');
        this._EmailAddress = $('#PendingEmail');
    }

    Initialize() {
        this._FirstName.suggest({ text: "Anonymous" });
        this._LastName.suggest({ text: "Anonymous" });
        this._DisplayName.suggest({ text: "Anonymous" });
        this._EmailAddress.suggest({ text: "Anonymous@sample.com" });
        this.SetupModals();
    }

    SetupModals(): void {
        var instance: UserProfilePage = this;

        CrudUtilities.MainWrapElement.on('hidden', CrudUtilities.ModalSelector, function () {
            CrudUtilities.SubmitButton.removeAttr("disabled");
        }).on('show', CrudUtilities.ModalSelector, function () {
            $(StringExtensions.format("{0},{1}", CrudUtilities.SubmitButtonSelector, CrudUtilities.CancelButtonSelector)).show();
        });

        CrudUtilities.WireupCrudModal(null,
            CrudUtilities.MainWrapSelector, '#eulaBtn', '/Agreement/UserEula/',
            true, 'EULA', 'EULA', '#modalBody', function () { 
                CrudUtilities.SubmitButton.hide();
                CrudUtilities.CancelButton.text('Close');
            }, null, null);
    }
}

$(document).ready(function () {
    new UserProfilePage().Initialize();
});
