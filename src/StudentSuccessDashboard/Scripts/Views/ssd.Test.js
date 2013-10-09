/// <reference path="ssd.ts" />
/// <chutzpah_reference path="../QUnit/qunit.js" />
/// <chutzpah_reference path="../jQuery/jquery-1.10.1.js" />
test("Given no scheme on url, When AddHyperlinkScheme, Then add http:// to url", function () {
    var expected = "foo.bar.com";
    var actual = AddHyperlinkScheme(expected);

    equal(actual, "http://foo.bar.com");
});

test("Given no scheme on url, And url is www domain, When AddHyperlinkScheme, Then add http:// to url", function () {
    var expected = "www.bar.com";
    var actual = AddHyperlinkScheme(expected);

    equal(actual, "http://www.bar.com");
});

test("Given http:// scheme on url, When AddHyperlinkScheme, Then leave url alone", function () {
    var expected = "http://foo.bar.com";
    var actual = AddHyperlinkScheme(expected);

    equal(actual, "http://foo.bar.com");
});

test("Given http:// scheme on url, And case differs normalized scheme, When AddHyperlinkScheme, Then leave url alone", function () {
    var expected = "htTp://foo.bar.com";
    var actual = AddHyperlinkScheme(expected);

    equal(actual, "htTp://foo.bar.com");
});

test("Given https:// scheme on url, When AddHyperlinkScheme, Then leave url alone", function () {
    var expected = "https://foo.bar.com";
    var actual = AddHyperlinkScheme(expected);

    equal(actual, "https://foo.bar.com");
});

test("Given https:// scheme on url, And case differs normalized scheme, When AddHyperlinkScheme, Then leave url alone", function () {
    var expected = "hTtps://foo.bar.com";
    var actual = AddHyperlinkScheme(expected);

    equal(actual, "hTtps://foo.bar.com");
});

test("Given two items in a select list, When ExtractSelectListValues, Then selected item values joined as pipe-delimited string", function () {
    var expected = "1|2";
    var dom = "<select><option value='1'>cat</option><option value='2'>dog</option></select>";
    $('body').html(dom);
    var testObject = $('option');

    var actual = ExtractSelectListValues(testObject);

    equal(actual, expected);
});
//# sourceMappingURL=ssd.Test.js.map
