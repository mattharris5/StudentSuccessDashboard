/// <reference path="../Selector.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="UserFilter.ts" />

class UserSelector extends Selector {

    constructor() {
        super();
    }

    AddHiddenSelectedElements(): void {
        for (var i = 0; i < this.SelectedRows.length; i++) {
            var newInput = $("<input type='hidden' value='" + this.SelectedRows[i] + "' />")
                 .attr("id", "Ids")
                 .attr("name", "Ids");
            $('li#SelectedUserIds').after(newInput);
        }
    }

    UpdateSelections(selectAll: boolean, filterCriteria: any): void {
        this.UpdateSelectionsWithUrl(selectAll, filterCriteria, "/User/AllFilteredUserIds");
    }
}