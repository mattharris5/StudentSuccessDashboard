/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="ServiceTypeDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
QUnit.module("", {
    setup: function () {
    },
    teardown: function () {
        DataTable.IsInPrintExportMode = false;
    }
});

test("Given no add button exists, When CreateColumns, Then action column not created", function () {
    var expectedLength = 4;
    var target = new ServiceTypeDataTable();

    var actual = target.CreateColumns();

    equal(actual.length, expectedLength);
    notEqual(actual[3].mData, "Id");
});

test("Given add button exists, When CreateColumns, Then action column created", function () {
    var expectedLength = 5;
    var target = new ServiceTypeDataTable();
    var dom = "<a id='addServiceType'></a>";
    $('body').html(dom);

    var actual = target.CreateColumns();

    equal(actual.length, expectedLength);
    equal(actual[4].mData, "Id");
});

test("When RenderPrivateFlags, Then render checkbox to update private flag", function () {
    var expected = '<input type="checkbox" class="private-checkbox" data-value="2" />';
    var target = new ServiceTypeDataTable();

    var actual = target.RenderPrivateFlags(false, null, { Id: 2 });

    strictEqual(actual, expected);
});

test("Given service type is private, When RenderPrivateFlags, Then checkbox to update private flag is checked", function () {
    var expected = '<input type="checkbox" class="private-checkbox" data-value="2" checked="CHECKED" />';
    var target = new ServiceTypeDataTable();

    var actual = target.RenderPrivateFlags(true, null, { Id: 2 });

    strictEqual(actual, expected);
});

test("Given table is in print export mode, When RenderPrivateFlags, Then return raw boolean string", function () {
    var target = new ServiceTypeDataTable();
    DataTable.IsInPrintExportMode = true;

    var actual = target.RenderPrivateFlags(false, null, { Id: 2 });

    equal(actual, false);
});

test("Given table is in print export mode, And service type is private, When RenderPrivateFlags, Then return raw boolean string", function () {
    var target = new ServiceTypeDataTable();
    DataTable.IsInPrintExportMode = true;

    var actual = target.RenderPrivateFlags(true, null, { Id: 2 });

    equal(actual, true);
});
//# sourceMappingURL=ServiceTypeDataTable.Test.js.map
