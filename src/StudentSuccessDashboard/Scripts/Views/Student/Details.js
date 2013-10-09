/// <reference path="../ManagePage.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentDetailsPage = (function (_super) {
    __extends(StudentDetailsPage, _super);
    function StudentDetailsPage() {
        _super.call(this);
    }
    StudentDetailsPage.prototype.Initialize = function () {
        var clickOverElements = $('[rel="clickover"]');
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
                }
            });
        });
    };
    return StudentDetailsPage;
})(ManagePage);

$(document).ready(function () {
    new StudentDetailsPage().Initialize();
});
//# sourceMappingURL=Details.js.map
