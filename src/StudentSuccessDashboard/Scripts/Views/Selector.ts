/// <reference path="ViewInterfaces.d.ts" />

class Selector implements ISelector {

    SelectedRows: string[];

    constructor() {
        this.SelectedRows = new Array();
    }

    Remove(value: string) {
        this.SelectedRows = $.grep(this.SelectedRows, function (item) {
            return item != value;
        }, false);
    }

    Add(value: string) {
        this.SelectedRows.push(value);
    }

    Reset() {
        this.SelectedRows = new Array();
    }

    SetSelections(allAvailableRows: string[], selectAll: boolean) {
        if (selectAll) {
            this.SelectedRows = allAvailableRows;
        }
        else {
            this.SelectedRows.forEach(function (elem) {
                if ($.inArray($(elem).data('value'), allAvailableRows) == -1) {
                    this.dtSelectedRows = $.grep(this.dtSelectedRows, function (item) {
                        return item != $(elem).data('value');
                    }, false);
                }
            });
        }
    }

    AddHiddenSelectedElements(): void {
        throw new Error("AddHiddenSelectedElements is not implemented");
    }

    CheckSelectedElements(checkBoxes: JQuery) {
        var instance: Selector = this;
        checkBoxes.each(function () {
            if ($.inArray($(this).data('value'), instance.SelectedRows) > -1) {
                $(this).attr('checked', 'CHECKED');
            }
            else {
                $(this).removeAttr('checked');
            }
        });
    }

    UpdateSelections(selectAll: boolean, filterCriteria: any): void {
        throw new Error("UpdateSelections is not implemented");
    }

    UpdateSelectionsWithUrl(selectAll: boolean, filterCriteria: any, url: string) {
        var instance: Selector = this;
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: url,
            data: filterCriteria,
            success: function (result) {
                instance.SetSelections(result, selectAll);
            },
            error: function (result) {
                // TODO: do something....
            }
        });
    }
}