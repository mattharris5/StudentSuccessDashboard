/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="PartnerFilter.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />

test("Given search criteria exists, When PushToArray, Then correct criteria added to array", function () {
    var aoData: any[] = [];
    $('body').append('<input name="autocompletePartnerName" id="autocompletePartnerName" type="text" value="bob">');
    var target: PartnerFilter = new PartnerFilter();

    target.PushToArray(aoData);

    deepEqual(aoData[0], { name: "PartnerName", value: "bob" });
});