/// <reference path="ViewInterfaces.d.ts" />
var Selector = (function () {
    function Selector() {
        this.SelectedRows = new Array();
    }
    Selector.prototype.Remove = function (value) {
        this.SelectedRows = $.grep(this.SelectedRows, function (item) {
            return item != value;
        }, false);
    };

    Selector.prototype.Add = function (value) {
        this.SelectedRows.push(value);
    };

    Selector.prototype.Reset = function () {
        this.SelectedRows = new Array();
    };

    Selector.prototype.SetSelections = function (allAvailableRows, selectAll) {
        if (selectAll) {
            this.SelectedRows = allAvailableRows;
        } else {
            this.SelectedRows.forEach(function (elem) {
                if ($.inArray($(elem).data('value'), allAvailableRows) == -1) {
                    this.dtSelectedRows = $.grep(this.dtSelectedRows, function (item) {
                        return item != $(elem).data('value');
                    }, false);
                }
            });
        }
    };

    Selector.prototype.AddHiddenSelectedElements = function () {
        throw new Error("AddHiddenSelectedElements is not implemented");
    };

    Selector.prototype.CheckSelectedElements = function (checkBoxes) {
        var instance = this;
        checkBoxes.each(function () {
            if ($.inArray($(this).data('value'), instance.SelectedRows) > -1) {
                $(this).attr('checked', 'CHECKED');
            } else {
                $(this).removeAttr('checked');
            }
        });
    };

    Selector.prototype.UpdateSelections = function (selectAll, filterCriteria) {
        throw new Error("UpdateSelections is not implemented");
    };

    Selector.prototype.UpdateSelectionsWithUrl = function (selectAll, filterCriteria, url) {
        var instance = this;
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
    };
    return Selector;
})();
//# sourceMappingURL=Selector.js.map
