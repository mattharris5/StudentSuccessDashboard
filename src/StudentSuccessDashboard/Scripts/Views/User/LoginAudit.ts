/// <reference path="../ssd.ts" />
/// <reference path="../ManagePage.ts" />
/// <reference path="LoginAuditDataTable.ts" />

class LoginAuditPage extends ManagePage {
    private _LoginAuditDataTable: LoginAuditDataTable;
    static accessDataArray: Array<string> = new Array<string>();

    constructor() {
        super();
        this._LoginAuditDataTable = new LoginAuditDataTable();
    }

    Initialize(): void {
        this._LoginAuditDataTable.Initialize(null);
    }
}

$(document).ready(function () {
    new LoginAuditPage().Initialize();
});