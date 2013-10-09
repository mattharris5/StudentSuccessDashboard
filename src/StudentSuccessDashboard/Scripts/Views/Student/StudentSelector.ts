/// <reference path="../Selector.ts" />

class StudentSelector extends Selector {

    constructor() {
        super();
        this.SelectedRows = new Array();
    }

    AddHiddenSelectedElements(): void {
        for (var i = 0; i < this.SelectedRows.length; i++) {
            var newInput = $("<input type='hidden' value='" + this.SelectedRows[i] + "' />")
                 .attr("id", "StudentIds")
                 .attr("name", "StudentIds");
            $('input#Id').after(newInput);
        }
    }

    UpdateSelections(selectAll: boolean, filterCriteria: any): void {
        this.UpdateSelectionsWithUrl(selectAll, filterCriteria, "/Student/AllFilteredStudentIds");
    }
}