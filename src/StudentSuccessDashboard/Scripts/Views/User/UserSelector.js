/// <reference path="../Selector.ts" />
/// <reference path="../ssd.ts" />
/// <reference path="UserFilter.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var UserSelector = (function (_super) {
    __extends(UserSelector, _super);
    function UserSelector() {
        _super.call(this);
    }
    UserSelector.prototype.AddHiddenSelectedElements = function () {
        for (var i = 0; i < this.SelectedRows.length; i++) {
            var newInput = $("<input type='hidden' value='" + this.SelectedRows[i] + "' />").attr("id", "Ids").attr("name", "Ids");
            $('li#SelectedUserIds').after(newInput);
        }
    };

    UserSelector.prototype.UpdateSelections = function (selectAll, filterCriteria) {
        this.UpdateSelectionsWithUrl(selectAll, filterCriteria, "/User/AllFilteredUserIds");
    };
    return UserSelector;
})(Selector);
//# sourceMappingURL=UserSelector.js.map
