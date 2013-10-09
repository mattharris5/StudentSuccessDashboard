/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="UserFilter.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
/// <chutzpah_reference path="../../jQuery/jquery-ui-1.10.3.js" />
/// <chutzpah_reference path="../../jQuery/jquery.multiOpenAccordion.js" />
/// <chutzpah_reference path="../ssd.js" />
test("Given status criteria, When ToJson, Then statuses are pipe-delimited.", function () {
    $('body').append('<select id="status" multiple="multiple"><option value="Inactive" selected="selected"></option><option value="Awaiting Assignment"></option><option value="Active" selected="selected"></option></select>');
    var target = new UserFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.Status, 'Inactive|Active');
});

test("Given roles, When ToJson, Then roles are pipe-delimited.", function () {
    $('body').append('<select id="roles" multiple="multiple"><option value="Data Admin" selected="selected"></option><option value="Provider"></option><option value="Site Coordinator" selected="selected"></option></select>');
    var target = new UserFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.Roles, 'Data Admin|Site Coordinator');
});

test("Given school names, When ToJson, Then names are pipe-delimited.", function () {
    $('body').append('<select id="schools" multiple="multiple"><option value="Addams" selected="selected"></option><option value="Anthony"></option><option value="Washington" selected="selected"></option></select>');
    var target = new UserFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.Schools, 'Addams|Washington');
});
//# sourceMappingURL=UserFilter.Test.js.map
