/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="StudentFilter.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
/// <chutzpah_reference path="../../jQuery/jquery-ui-1.10.3.js" />
/// <chutzpah_reference path="../ssd.js" />
test("Given grades criteria, When ToJson, Then grades are pipe-delimited.", function () {
    $('body').append('<select id="grades" multiple="multiple"><option value="1" selected="selected"></option><option value="2"></option><option value="3" selected="selected"></option><option value="4" selected="selected"></option></select>');
    var target = new StudentFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        },
        Refresh: function () {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.grades, '1|3|4');
});

test("Given school criteria, When ToJson, Then school names are pipe-delimited.", function () {
    $('body').append('<select id="schools" multiple="multiple"><option value="Apples" selected="selected"></option><option value="Oranges"></option><option value="Bananas, Green" selected="selected"></option><option value="Bananas, Yellow" selected="selected"></option></select>');
    var target = new StudentFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        },
        Refresh: function () {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.schools, 'Apples|Bananas, Green|Bananas, Yellow');
});

test("Given priority criteria, When ToJson, Then priority names are pipe-delimited.", function () {
    $('body').append('<select id="priorities" multiple="multiple"><option value="Low" selected="selected"></option><option value="Medium"></option><option value="High" selected="selected"></option><option value="Super, Duper High" selected="selected"></option></select>');
    var target = new StudentFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        },
        Refresh: function () {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.priorities, 'Low|High|Super, Duper High');
});

test("Given status criteria, When ToJson, Then statuses are pipe-delimited.", function () {
    $('body').append('<select id="requestStatuses" multiple="multiple"><option value="Fulfilled" selected="selected"></option><option value="Open"></option><option value="Rejected" selected="selected"></option></select>');
    var target = new StudentFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        },
        Refresh: function () {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.requestStatuses, 'Fulfilled|Rejected');
});

test("Given service type criteria, When ToJson, Then service types are pipe-delimited.", function () {
    $('body').append('<select id="serviceTypeFilterList" multiple="multiple"><option value="Mentoring" selected="selected"></option><option value="Provider College Access"></option><option value="Test type" selected="selected"></option></select>');
    var target = new StudentFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        },
        Refresh: function () {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.serviceTypes, 'Mentoring|Test type');
});

test("Given subject criteria, When ToJson, Then subjects are pipe-delimited.", function () {
    $('body').append('<select id="subjects" multiple="multiple"><option value="Math" selected="selected"></option><option value="Science"></option><option value="English" selected="selected"></option></select>');
    var target = new StudentFilter();
    target.Initialize({
        CreateColumns: function () {
            return [];
        },
        DataTableElement: null,
        Initialize: function (filter) {
        },
        Refresh: function () {
        }
    });

    var result = target.ToJson();

    QUnit.strictEqual(result.subjects, 'Math|English');
});
//# sourceMappingURL=StudentFilter.Test.js.map
