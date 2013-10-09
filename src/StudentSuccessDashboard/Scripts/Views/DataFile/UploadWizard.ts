/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="../../typings/bootstrap/bootstrap.d.ts" />

class UploadWizardPage extends ManagePage {

    constructor() {
        super();
    }

    Initialize(): void {
        this.SetupControls();
    }

    SetupControls(): void {
        var setupTooltips: any = $("[rel=tooltip]");
        setupTooltips.tooltip();
    }
}