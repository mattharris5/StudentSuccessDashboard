/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../typings/sinon/sinon-1.5.d.ts" />
/// <reference path="StudentApprovalDataTable.ts" />
/// <reference path="Manage.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../QUnit/sinon-1.5.2.js" />
/// <chutzpah_reference path="../../QUnit/sinon-qunit-1.0.0.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
/// <chutzpah_reference path="../../jQuery/jquery-ui-1.10.3.js" />
/// <chutzpah_reference path="../../jQuery/jquery.bsmselect.js" />
/// <chutzpah_reference path="../../jQuery/jquery.multiOpenAccordion.js" />
/// <chutzpah_reference path="../../DataTables-1.9.4/media/js/jquery.dataTables.js" />
/// <chutzpah_reference path="../../DataTables-1.9.4/media/js/ssd-datatables.js" />
/// <chutzpah_reference path="../ssd.js" />
test("When Initialize, Then multi open accordions prepared", function () {
    var spy = sinon.spy(jQuery.prototype, "multiOpenAccordion");

    new ManageStudentApprovalPage(new StudentApprovalDataTable()).Initialize();

    ok(spy.calledTwice);
    deepEqual(spy.firstCall.args[0], { active: 'none' });
    deepEqual(spy.secondCall.args, ["option", "active", 'none']);
    spy.thisValues.forEach(function (thisValue) {
        equal(thisValue.selector, "#multiOpenAccordion");
    });
});

test("When Initialize, Then multi select options prepared", function () {
    var spy = sinon.spy(jQuery.prototype, "bsmSelect");

    new ManageStudentApprovalPage(new StudentApprovalDataTable()).Initialize();

    ok(spy.calledOnce);
    deepEqual(spy.firstCall.args[0], { addItemTarget: 'top' });
    equal(spy.firstCall.thisValue.selector, "select[multiple]");
});

test("When Initialize, Then child data table initialized", function () {
    var dataTable = new StudentApprovalDataTable();
    var spy = sinon.spy(dataTable, "Initialize");

    new ManageStudentApprovalPage(dataTable).Initialize();

    ok(spy.calledOnce);
});

test("Given null StudentApprovalDataTable, When Constructed, Then raise error", function () {
    QUnit.throws(function () {
        new ManageStudentApprovalPage(null);
    });
});
//# sourceMappingURL=Manage.Test.js.map
