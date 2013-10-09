/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../typings/sinon/sinon-1.5.d.ts" />
/// <reference path="StudentApprovalDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../QUnit/sinon-1.5.2.js" />
/// <chutzpah_reference path="../../QUnit/sinon-qunit-1.0.0.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
/// <chutzpah_reference path="../../DataTables-1.9.4/media/js/jquery.dataTables.js" />
/// <chutzpah_reference path="../../DataTables-1.9.4/media/js/ssd-datatables.js" />
QUnit.module("", {
    setup: function () {
    },
    teardown: function () {
        DataTable.IsInPrintExportMode = false;
    }
});

test("When Initialize, Then data table prepared", function () {
    var spy = sinon.spy(jQuery.prototype, "dataTable");

    new StudentApprovalDataTable().Initialize(new StudentApprovalFilter());

    ok(spy.calledOnce);
    equal(spy.firstCall.args[0].sAjaxSource, "/StudentApproval/DataTableAjaxHandler");
    equal(spy.firstCall.thisValue.selector, "#studentApprovals");
});

test("When RenderOptOut, Then render checkbox to update opt-out flag", function () {
    var expected = "<input class='optOut-checkbox' type='checkbox' data-value='1' />";
    var full = {
        Id: 1
    };
    var target = new StudentApprovalDataTable();

    var actual = target.RenderOptOut(false, null, full);

    strictEqual(actual, expected);
});

test("Given student has parental opt-out, When RenderOptOut, Then checkbox to update opt-out flag is checked", function () {
    var expected = "<input class='optOut-checkbox' type='checkbox' data-value='1' checked='CHECKED' />";
    var full = {
        Id: 1
    };
    var target = new StudentApprovalDataTable();

    var actual = target.RenderOptOut(true, null, full);

    strictEqual(actual, expected);
});

test("Given table is in print export mode, When RenderOptOut, Then return raw boolean string", function () {
    var expected = false;
    var target = new StudentApprovalDataTable();
    DataTable.IsInPrintExportMode = true;

    var actual = target.RenderOptOut(false, null, null);

    equal(actual, expected);
});
//# sourceMappingURL=StudentApprovalDataTable.Test.js.map
