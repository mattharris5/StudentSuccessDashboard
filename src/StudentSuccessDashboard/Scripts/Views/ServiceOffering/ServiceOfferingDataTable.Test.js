/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="ServiceOfferingDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
QUnit.module("", {
    setup: function () {
    },
    teardown: function () {
        DataTable.IsInPrintExportMode = false;
    }
});

test("Given service offering not editable, When RenderFavorites, Then no checkbox markup returned", function () {
    var target = new ServiceOfferingDataTable();
    var full = {
        CanInteract: false
    };

    var actual = target.RenderFavorites(true, null, full);

    ok(actual.indexOf('checkbox') === -1, 'Result: ' + actual);
});

test("Given service offering editable, When RenderFavorites, Then checkbox markup returned", function () {
    var target = new ServiceOfferingDataTable();
    var full = {
        CanInteract: true
    };

    var actual = target.RenderFavorites(true, null, full);

    ok(actual.indexOf('checkbox') !== -1, 'Result: ' + actual);
});

test("Given service offering editable, When RenderFavorites, Then render checkbox to update favorite flag", function () {
    var expected = "<input name='IsFavorite' class='favorite-checkbox' type='checkbox' data-value='4' />";
    var full = {
        CanInteract: true,
        Id: 4
    };
    var target = new ServiceOfferingDataTable();

    var actual = target.RenderFavorites(false, null, full);

    strictEqual(actual, expected);
});

test("Given service offering editable, And service offering is favorite, When RenderFavorites, Then checkbox to update favorite flag is checked", function () {
    var expected = "<input name='IsFavorite' class='favorite-checkbox' type='checkbox' checked='CHECKED' data-value='4' />";
    var full = {
        CanInteract: true,
        Id: 4
    };
    var target = new ServiceOfferingDataTable();

    var actual = target.RenderFavorites(true, null, full);

    strictEqual(actual, expected);
});

test("Given service offering editable, And table is in print export mode, When RenderFavorites, Then return raw boolean string", function () {
    var expected = false;
    var full = {
        CanInteract: true
    };
    var target = new ServiceOfferingDataTable();
    DataTable.IsInPrintExportMode = true;

    var actual = target.RenderFavorites(false, null, full);

    equal(actual, expected);
});

test("Given service offering editable, When RenderActions, Then buttons markup returned", function () {
    var target = new ServiceOfferingDataTable();
    var full = {
        Program: "",
        CanInteract: true
    };

    var actual = target.RenderActions(4, null, full);

    ok(actual.indexOf('buttons') !== -1, 'Result: ' + actual);
});
//# sourceMappingURL=ServiceOfferingDataTable.Test.js.map
