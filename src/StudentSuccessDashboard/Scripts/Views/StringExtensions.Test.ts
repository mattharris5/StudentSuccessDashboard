/// <reference path="../typings/qunit/qunit.d.ts" />
/// <reference path="StringExtensions.ts" />

test("Given string ends with suffix, When EndsWith, Then return true", function () {
    var expected = true;

    var actual = StringExtensions.endsWith("test", "st");

    equal(actual, expected);
});

test("Given string does not end with suffic, When EndsWith, Then return false", function () {
    var expected = false;

    var actual = StringExtensions.endsWith("test", "bob");

    equal(actual, expected);
});

test("Given string starts with prefix, When StartsWith, Then return true", function () {
    var expected = true;

    var actual = StringExtensions.startsWith("test", "te");

    equal(actual, expected);
});

test("Given string does not start with prefix, When StartsWith, Then return false", function () {
    var expected = false;

    var actual = StringExtensions.startsWith("test", "bob");

    equal(actual, expected);
});

test("Given string with leading and trailing spaces, When Trim, Then leading and trailing spaces removed", function () {
    var expected = "test";

    var actual = StringExtensions.trim("  test  ");
     
    equal(actual, expected);
});

test("Given string with leading spaces, when TrimStart, Then only leading spaces removed", function () {
    var expected = "test";

    var actual = StringExtensions.trimStart("   test");

    equal(actual, expected);
});

test("Given string with no leading spaces, when TrimStart, Then string is unchanged", function () {
    var expected = "test";

    var actual = StringExtensions.trimStart("test");

    equal(actual, expected);
});

test("Given string with trailing spaces, when TrimEnd, Then only trailing spaces removed", function () {
    var expected = "test";

    var actual = StringExtensions.trimEnd("test  ");

    equal(actual, expected);
});

test("Given string with no trailing spaces, when TrimEnd, Then string is unchanged", function () {
    var expected = "test";

    var actual = StringExtensions.trimEnd("test");

    equal(actual, expected);
});

test("Given format string with two arguments, When Format, Then string result contains arguments at correct positions", function () {
    var expected = "cats really smell";
    var format :string = "{0} really {1}";

    var actual = StringExtensions.format(format, "cats", "smell");

    equal(actual, expected);
});

test("Given format string with argument, And argument is null, When Format, Then string result ignores null", function () {
    var expected = "good ";
    var format: string = "good {0}";

    var actual = StringExtensions.format(format, null);

    equal(actual, expected);
});