/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="UserDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />

QUnit.module("", {
    setup: function () {
    },
    teardown: function () {
        DataTable.IsInPrintExportMode = false;
    }
});

test("Given user is active, When RenderStatus, Then render green icon", function () {
    var expected: string = '<i class="icon-sign-blank green" title="Active"></i>';
    var target: UserDataTable = new UserDataTable(new UserSelector());

    var actual: string = target.RenderStatus("Active", null, null);

    strictEqual(actual, expected);
});

test("Given user is inactive, When RenderStatus, Then render red icon", function () {
    var expected: string = '<i class="icon-sign-blank red" title="Inactive"></i>';
    var target: UserDataTable = new UserDataTable(new UserSelector());

    var actual: string = target.RenderStatus("Inactive", null, null);

    strictEqual(actual, expected);
});

test("Given user is awaiting activation, When RenderStatus, Then render orange icon", function () {
    var expected: string = '<i class="icon-sign-blank orange" title="Awaiting Assignment"></i>';
    var target: UserDataTable = new UserDataTable(new UserSelector());

    var actual: string = target.RenderStatus("Awaiting Assignment", null, null);

    strictEqual(actual, expected);
});

test("Given table is in print export mode, When RenderStatus, Then return raw boolean string", function () {
    var expected: string = "Active";
    var target: UserDataTable = new UserDataTable(new UserSelector());
    DataTable.IsInPrintExportMode = true;

    var actual: string = target.RenderStatus("Active", null, null);

    strictEqual(actual, expected);
});

test("When RenderEmail, Then render email as hyperlink", function () {
    var expected: string = '<a class="btn btn-mini" href="mailto:bob@bob.com"><i class="icon-envelope"></i></a>';
    var target: UserDataTable = new UserDataTable(new UserSelector());

    var actual: string = target.RenderEmail("bob@bob.com", null, null);

    strictEqual(actual, expected);
});

test("Given table is in print export mode, When RenderEmail, Then return raw email", function () {
    var expected: string = "bob@bob.com";
    var target: UserDataTable = new UserDataTable(new UserSelector());
    DataTable.IsInPrintExportMode = true;

    var actual: string = target.RenderEmail("bob@bob.com", null, null);

    strictEqual(actual, expected);
});

test("When RenderAssociations, Then render association count as button link", function () {
    var expected: string = "<a class='btn btn-mini user-associations' data-toggle='modal' data-value='3' role='button' href='#'>12</a>";
    var full: any = {
        Id: 3
    };
    var target: UserDataTable = new UserDataTable(new UserSelector());

    var actual: string = target.RenderAssociations(12, null, full);

    strictEqual(actual, expected);
});

test("Given no associations, When RenderAssociations, Then render empty string", function () {
    var expected: string = "";
    var full: any = {
        Id: 3
    };
    var target: UserDataTable = new UserDataTable(new UserSelector());

    var actual: string = target.RenderAssociations(0, null, full);

    strictEqual(actual, expected);
});

test("Given table is in print export mode, When RenderAssociations, Then return raw association count", function () {
    var expected = 12;
    var full: any = {
        Id: 3
    };
    var target: UserDataTable = new UserDataTable(new UserSelector());
    DataTable.IsInPrintExportMode = true;

    var actual: string = target.RenderAssociations(12, null, full);

    strictEqual(actual, expected);
});