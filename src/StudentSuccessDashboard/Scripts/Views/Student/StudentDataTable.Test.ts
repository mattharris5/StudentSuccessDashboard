/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="StudentDataTable.ts" />
/// <chutzpah_reference path="../../QUnit/qunit.js" />
/// <chutzpah_reference path="../../jQuery/jquery-1.10.1.js" />

test("Given service request data has no subject, When RenderServiceRequest, Then rendered service request contains no subject", function () {
    var target: StudentDataTable = new StudentDataTable(new StudentSelector());
    var args = { aData: new Array("Y|1|TestName|2||", "", "", "", "", "", "", "") };
    var actual: string = target.RenderServiceRequest(args);
    var expected: string = '<ul class="flags"><li><div class="btn-group">' +
                        '<a class="btn btn-mini btn-success dropdown-toggle" data-toggle="dropdown">TestName<span class="caret"></span></a>' +
                        '<ul class="dropdown-menu pull-right"><li><a href="#" class="editServiceRequest" data-value="1"><i class="icon-edit"></i> Edit This Service Request</a></li>' +
                        '<li><a href="#" class="removeServiceRequest" data-value="1"><i class="icon-remove"></i> Remove This Service Request</a></li></ul></div></li></ul>';
    strictEqual(actual, expected);
});

test("Given service request data has subject, When RenderServiceRequest, Then rendered service request contains subject", function () {
    var target: StudentDataTable = new StudentDataTable(new StudentSelector());
    var args = { aData: new Array("Y|1|TestName|2|TestSubject|", "", "", "", "", "", "", "") };
    var actual: string = target.RenderServiceRequest(args);
    var expected: string = '<ul class="flags"><li><div class="btn-group">' +
                        '<a class="btn btn-mini btn-success dropdown-toggle" data-toggle="dropdown">TestName/TestSubject<span class="caret"></span></a>' +
                        '<ul class="dropdown-menu pull-right"><li><a href="#" class="editServiceRequest" data-value="1"><i class="icon-edit"></i> Edit This Service Request</a></li>' +
                        '<li><a href="#" class="removeServiceRequest" data-value="1"><i class="icon-remove"></i> Remove This Service Request</a></li></ul></div></li></ul>';
    strictEqual(actual, expected);
});
