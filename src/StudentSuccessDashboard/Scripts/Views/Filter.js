/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="ViewInterfaces.d.ts" />
var Filter = (function () {
    function Filter() {
        this._MultiOpenAccordion = $('#multiOpenAccordion');
    }
    Filter.prototype.Initialize = function (dataTable) {
        throw new Error("Initialize is not implemented");
    };

    Filter.prototype.PushToArray = function (aoData) {
        throw new Error("PushToArray is not implemented");
    };

    Filter.prototype.ToJson = function () {
        throw new Error("ToJson is not implemented");
    };

    Filter.prototype.Reset = function () {
        this._MultiOpenAccordion.multiOpenAccordion({
            active: 'none'
        });
        this._MultiOpenAccordion.multiOpenAccordion("option", "active", 'none');
    };
    return Filter;
})();
//# sourceMappingURL=Filter.js.map
