/// <reference path="../ManagePage.ts" />
/// <reference path="../ssd.ts" />

class ServiceOfferingFileUploadCompletePage extends ManagePage {

    constructor() {
        super();
    }

    Initialize(): void {
        this.SetupControls();
    }

    SetupControls(): void {
        $('#redirectToServiceOffering').on('click', function () {
            window.location.href = '/ServiceOffering';
        });
    }
}

$(document).ready(function () {
    new ServiceOfferingFileUploadCompletePage().Initialize();
});