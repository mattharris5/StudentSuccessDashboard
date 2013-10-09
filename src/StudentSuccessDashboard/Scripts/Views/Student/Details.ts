/// <reference path="../ManagePage.ts" />

class StudentDetailsPage extends ManagePage {

    constructor() {
        super();
    }

    Initialize(): void {
        var clickOverElements: any = $('[rel="clickover"]');
        clickOverElements.clickover();
        $('#addServiceOffering').on('click', function (event) {
            var postUrl = '/Service/SaveStudentIds';
            var returnUrl = '/Student/Details/' + $(this).data("value");
            event.preventDefault();
            $.ajax({
                type: 'POST',
                dataType: 'html',
                url: postUrl,
                data: JSON.stringify({ students: $(this).data("value"), returnUrl: returnUrl }),
                contentType: 'application/json',
                success: function (result) {
                    window.location.href = '/service/ScheduleOffering?ReturnUrl=' + returnUrl;
                },
            });
        });
    }
}

$(document).ready(function () {
    new StudentDetailsPage().Initialize();
});
