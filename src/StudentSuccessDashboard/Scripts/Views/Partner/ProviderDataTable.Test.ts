/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="ProviderDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />

test("When CreateColumns, Then correct number of columns created", function () {
    var expectedLength: number = 5;
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: any[] = target.CreateColumns();

    equal(actual.length, expectedLength);
});

test("When CreateColumns, Then columns created in correct order", function () {
    var expectedLength: number = 5;
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: any[] = target.CreateColumns();

    equal(actual.length, expectedLength);
    equal(actual[0].mData, "Name");
    equal(actual[1].mData, "Contact");
    equal(actual[2].mData, "Schools");
    equal(actual[3].mData, "Programs")
    equal(actual[4].mData, "Id");
});

test("Given Provider with full address, When RenderProvider, Then name and address displayed", function () {
    var full: any = {
        Address: {
            Street: "123 Main St.",
            City: "Cbus",
            State: "OH",
            Zip: "44444"
        }
    };
    var expected: string = "Youth Center<br />123 Main St.<br />Cbus, OH 44444";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderProvider("Youth Center", null, full);

    equal(actual, expected);
});

test("Given Provider with no address, When RenderProvider, Then name displayed", function () {
    var full: any = {
        Address: {}
    };
    var expected: string = "Youth Center";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderProvider(expected, null, full);

    equal(actual, expected);
});

test("Given Provider with empty address, When RenderProvider, Then name displayed", function () {
    var full: any = {
        Address: {
            Street: "",
            City: "",
            State: "",
            Zip: ""
        }
    };
    var expected: string = "Youth Center";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderProvider(expected, null, full);

    equal(actual, expected);
});

test("Given Provider has a link, When RenderProvider, Then link will open in a new tab/window", function () {
    var full: any = {
        Website: "www.made-up-web-site.com",
        Address: { }
    };
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderProvider(null, null, full);

    equal("_blank", $(actual).attr('target'));
});

test("Given Provider has contact info, When RenderContact, Then contact info displayed", function () {
    var data: any = {
        Name: "Bob",
        Phone: "555-444-1234",
        Email: "bob@bob.bob"
    }
    var expected: string = "Contact Name: Bob<br />Phone: 555-444-1234<br />Email: <a href='mailto:bob@bob.bob'>bob@bob.bob</a>";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has contact name only, When RenderContact, Then contact name displayed", function () {
    var data: any = {
        Name: "Bob",
        Phone: "",
        Email: ""
    }
    var expected: string = "Contact Name: Bob";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has contact phone only, When RenderContact, Then contact phone displayed", function () {
    var data: any = {
        Name: "",
        Phone: "555-444-1234",
        Email: ""
    }
    var expected: string = "Phone: 555-444-1234";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has contact email only, When RenderContact, Then contact email displayed", function () {
    var data: any = {
        Name: "",
        Phone: "",
        Email: "bob@bob.bob"
    }
    var expected: string = "Email: <a href='mailto:bob@bob.bob'>bob@bob.bob</a>";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderContact(data, null, null);

    equal(actual, expected);
});

test("Given Provider has no contact info, When RenderContact, Then empty string displayed", function () {
    var data: any = {
        Name: "",
        Phone: "",
        Email: ""
    }
    var expected: string = "";
    var target: ProviderDataTable = new ProviderDataTable();

    var actual: string = target.RenderContact(data, null, null);

    equal(actual, expected);
});