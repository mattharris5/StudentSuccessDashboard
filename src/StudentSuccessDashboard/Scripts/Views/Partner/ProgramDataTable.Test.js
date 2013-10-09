/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="ProgramDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
test("When CreateColumns, Then correct number of columns created", function () {
    var expectedLength = 6;
    var target = new ProgramDataTable();

    var actual = target.CreateColumns();

    equal(actual.length, expectedLength);
});

test("When CreateColumns, Then columns created in correct order", function () {
    var target = new ProgramDataTable();

    var actual = target.CreateColumns();

    equal(actual[0].mData, "Name");
    equal(actual[1].mData, "Contact");
    equal(actual[2].mData, "Schools");
    equal(actual[3].mData, "Providers");
    equal(actual[4].mData, "ServiceTypes");
    equal(actual[5].mData, "Id");
});
//# sourceMappingURL=ProgramDataTable.Test.js.map
