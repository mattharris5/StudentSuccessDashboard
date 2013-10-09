/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="ProviderDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />
test("When CreateColumns, Then correct number of columns created", function () {
    var expectedLength = 5;
    var target = new ProviderDataTable();

    var actual = target.CreateColumns();

    equal(actual.length, expectedLength);
});

test("When CreateColumns, Then columns created in correct order", function () {
    var expectedLength = 5;
    var target = new ProviderDataTable();

    var actual = target.CreateColumns();

    equal(actual.length, expectedLength);
    equal(actual[0].mData, "Name");
    equal(actual[1].mData, "Contact");
    equal(actual[2].mData, "Schools");
    equal(actual[3].mData, "Programs");
    equal(actual[4].mData, "Id");
});

test("Given Provider with full address, When RenderProvider, Then name and address displayed", function () {
    var full = {
        Address: {
            Street: "123 Main St.",
            City: "Cbus",
            State: "OH",
            Zip: "44444"
        }
    };
    var expected = "Youth Center<br />123 Main St.<br />Cbus, OH 44444";
    var target = new ProviderDataTable();

    var actual = target.RenderProvider("Youth Center", null, full);

    equal(actual, expected);
});

test("Given Provider with no address, When RenderProvider, Then name displayed", function () {
    var full = {
        Address: {}
    };
    var expected = "Youth Center";
    var target = new ProviderDataTable();

    var actual = target.RenderProvider(expected, null, full);

    equal(actual, expected);
});

test("Given Provider with empty address, When RenderProvider, Then name displayed", function () {
    var full = {
        Address: {
            Street: "",
            City: "",
            State: "",
            Zip: ""
        }
    };
    var expected = "Youth Center";
    var target = new ProviderDataTable();

    var actual = target.RenderProvider(expected, null, full);

    equal(actual, expected);
});

test("Given Provider has a link, When RenderProvider, Then link will open in a new tab/window", function () {
    var full = {
        Website: "www.made-up-web-site.com",
        Address: {}
    };
    var target = new ProviderDataTable();

    var actual = target.RenderProvider(null, null, full);

    equal("_blank", $(actual).attr('target'));
});

test("Given Provider has contact info, When RenderContact, Then contact info displayed", function () {
    var data = {
        Name: "Bob",
        Phone: "555-444-1234",
        Email: "bob@bob.bob"
    };
    var expected = "Contact Name: Bob<br />Phone: 555-444-1234<br />Email: <a href='mailto:bob@bob.bob'>bob@bob.bob</a>";
    var target = new ProviderDataTable();

    var actual = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has contact name only, When RenderContact, Then contact name displayed", function () {
    var data = {
        Name: "Bob",
        Phone: "",
        Email: ""
    };
    var expected = "Contact Name: Bob";
    var target = new ProviderDataTable();

    var actual = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has contact phone only, When RenderContact, Then contact phone displayed", function () {
    var data = {
        Name: "",
        Phone: "555-444-1234",
        Email: ""
    };
    var expected = "Phone: 555-444-1234";
    var target = new ProviderDataTable();

    var actual = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has contact email only, When RenderContact, Then contact email displayed", function () {
    var data = {
        Name: "",
        Phone: "",
        Email: "bob@bob.bob"
    };
    var expected = "Email: <a href='mailto:bob@bob.bob'>bob@bob.bob</a>";
    var target = new ProviderDataTable();

    var actual = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has no contact info, When RenderContact, Then empty string displayed", function () {
    var data = {
        Name: "",
        Phone: "",
        Email: ""
    };
    var expected = "";
    var target = new ProviderDataTable();

    var actual = target.RenderContact(data, null, null);

    equal(actual, expected);
});
//# sourceMappingURL=ProviderDataTable.Test.js.map
