/// <reference path="../Selector.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var StudentSelector = (function (_super) {
    __extends(StudentSelector, _super);
    function StudentSelector() {
        _super.call(this);
        this.SelectedRows = new Array();
    }
    StudentSelector.prototype.AddHiddenSelectedElements = function () {
        for (var i = 0; i < this.SelectedRows.length; i++) {
            var newInput = $("<input type='hidden' value='" + this.SelectedRows[i] + "' />").attr("id", "StudentIds").attr("name", "StudentIds");
            $('input#Id').after(newInput);
        }
    };

    StudentSelector.prototype.UpdateSelections = function (selectAll, filterCriteria) {
        this.UpdateSelectionsWithUrl(selectAll, filterCriteria, "/Student/AllFilteredStudentIds");
    };
    return StudentSelector;
})(Selector);
//# sourceMappingURL=StudentSelector.js.map
