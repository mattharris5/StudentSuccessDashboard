/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="ViewInterfaces.d.ts" />
var DataTable = (function () {
    function DataTable(dataTableElement) {
        this.DataTableElement = dataTableElement;
        this._DelayTimer = undefined;
    }
    DataTable.prototype.Refresh = function () {
        var instance = this;
        if (this._DelayTimer != undefined) {
            clearTimeout(this._DelayTimer);
        }
        this._DelayTimer = setTimeout(function () {
            instance.DataTableElement.fnFilter();
        }, 1000);
    };

    DataTable.prototype.Initialize = function (filter) {
        this._Filter = filter;
        var instance = this;
        this.DataTableElement.dataTable({
            "bServerSide": true,
            "sAjaxSource": instance.GetServerDataUrl(),
            "bProcessing": true,
            "bPagination": true,
            "sDom": instance.GetDataTableDomData(),
            "oTableTools": DataTable.CreateToolsOptions(instance.GetPrintExportColumnIndices()),
            "aoColumns": instance.CreateColumns(),
            "fnServerParams": function (aoData) {
                instance.SetServerParameters(aoData);
            },
            "fnServerData": instance.LoadServerData,
            "fnDrawCallback": function (oSettings) {
                instance.DrawCallback(oSettings);
            },
            "oLanguage": instance.GetTableLanguage()
        }).fnSetFilteringDelay();
    };

    DataTable.prototype.SetServerParameters = function (aoData) {
        this._Filter.PushToArray(aoData);
    };

    DataTable.prototype.GetTableLanguage = function () {
        return {
            "oAria": {
                "sSortAscending": ": activate to sort column ascending",
                "sSortDescending": ": activate to sort column descending"
            },
            "oPaginate": {
                "sFirst": "First",
                "sLast": "Last",
                "sNext": "Next",
                "sPrevious": "Previous"
            },
            "sEmptyTable": "No data available in table",
            "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
            "sInfoEmpty": "Showing 0 to 0 of 0 entries",
            "sInfoFiltered": "(filtered from _MAX_ total entries)",
            "sInfoPostFix": "",
            "sInfoThousands": ",",
            "sLengthMenu": "Show _MENU_ entries",
            "sLoadingRecords": "Loading...",
            "sProcessing": "Processing...",
            "sSearch": "Search:",
            "sUrl": "",
            "sZeroRecords": "No matching records found"
        };
    };

    DataTable.prototype.GetServerDataUrl = function () {
        throw new Error("GetServerDataUrl is not implemented");
    };

    DataTable.prototype.GetPrintExportColumnIndices = function () {
        throw new Error("GetPrintExportColumnIndices is not implemented");
    };

    DataTable.prototype.CreateColumns = function () {
        throw new Error("CreateColumns is not implemented");
    };

    DataTable.prototype.DrawCallback = function (oSettings) {
        throw new Error("DrawCallback is not implemented");
    };

    DataTable.prototype.GetDataTableDomData = function () {
        if (this.GetPrintExportColumnIndices()) {
            return 'T<"clear">lfrtip';
        }
        return "lfrtip";
    };

    DataTable.prototype.LoadServerData = function (sUrl, aoData, fnCallback, oSettings) {
        oSettings.jqXHR = $.ajax({
            "url": sUrl,
            "data": aoData,
            "success": function (json) {
                if (json.sError) {
                    oSettings.oApi._fnLog(oSettings, 0, json.sError);
                }
                $(oSettings.oInstance).trigger('xhr', [oSettings, json]);
                fnCallback(json);
            },
            "dataType": "json",
            "cache": false,
            "type": oSettings.sServerMethod,
            "error": function (xhr, textStatus, errorThrown) {
                var dataString = xhr.responseText;
                if (dataString && dataString.search('<input id="LoginToken" name="LoginToken" type="hidden" value="" />') >= 0) {
                    alert('You are no longer authorized to access data.  Please log back in.');
                    $(document.body).html(dataString);
                } else {
                    if (textStatus == "parsererror") {
                        oSettings.oApi._fnLog(oSettings, 0, "DataTables warning: JSON data from " + "server could not be parsed. This is caused by a JSON formatting error.");
                    }
                }
            }
        });
    };

    DataTable.CreateToolsOptions = function (exportColumnIndices) {
        if (exportColumnIndices) {
            var buttons = [
                {
                    "sExtends": "copy",
                    "sButtonText": "Copy",
                    "mColumns": exportColumnIndices,
                    "fnInit": DataTable.InitializeExportButton
                },
                {
                    "sExtends": "csv",
                    "sButtonText": "CSV",
                    "mColumns": exportColumnIndices,
                    "fnInit": DataTable.InitializeExportButton
                },
                {
                    "sExtends": "xls",
                    "sButtonText": "Tab Delimited",
                    "mColumns": exportColumnIndices,
                    "fnInit": DataTable.InitializeExportButton
                },
                {
                    "sExtends": "pdf",
                    "sButtonText": "PDF",
                    "mColumns": exportColumnIndices,
                    "fnInit": DataTable.InitializeExportButton
                },
                {
                    "sExtends": "print",
                    "sButtonText": "Export/Print",
                    "fnInit": function (nButton, oConfig) {
                        nButton.className += " btn btn-mini";
                    },
                    "fnComplete": DataTable.EnterPrintPreviewHandler
                }
            ];
            return {
                "sSwfPath": "../../../../Content/DataTables-1.9.4-Overrides/copy_csv_xls_pdf.swf",
                "aButtons": buttons
            };
        }
        return null;
    };

    DataTable.ExitPrintPreviewHandler = function (e) {
        if (e.which === 27) {
            DataTable.IsInPrintExportMode = false;
            $('.dataTables_filter').hide();
            $('.col2').css('padding-left', '294px');
            $('.dataTable').css('width', '');
            $('#main-wrap').addClass('wide');
            $('.DTTT_button_copy').addClass('HideDTTTClass');
            $('.DTTT_button_csv').addClass('HideDTTTClass');
            $('.DTTT_button_xls').addClass('HideDTTTClass');
            $('.DTTT_button_pdf').addClass('HideDTTTClass');
            $('.DTTT_button_print').removeClass('HideDTTTClass');
            $('.DTTT_button_print').css('display', '');
            DataTable.PrintPreviewReference.fnPrint(false, DataTable.PrintPreviewOConfigReference);
            $(window).unbind('keyup', DataTable.ExitPrintPreviewHandler);
        }
    };

    DataTable.EnterPrintPreviewHandler = function (nButton, oConfig, oFlash, sFlash) {
        DataTable.IsInPrintExportMode = true;
        DataTable.PrintPreviewReference = this;
        DataTable.PrintPreviewOConfigReference = oConfig;
        $('#main-wrap').removeClass('wide');
        $('#main-wrap').css('max-width', '95%');
        $('.col2').css('padding-left', '0');
        $('.dataTable').css('width', '100%');
        $('.DTTT_button_copy').removeClass('HideDTTTClass');
        $('.DTTT_button_csv').removeClass('HideDTTTClass');
        $('.DTTT_button_xls').removeClass('HideDTTTClass');
        $('.DTTT_button_pdf').removeClass('HideDTTTClass');
        $('.DTTT_button_print').addClass('HideDTTTClass');
        $('.DTTT_button_print').css('display', 'none');
        $(window).keyup(DataTable.ExitPrintPreviewHandler);
    };

    DataTable.InitializeExportButton = function (nButton, oConfig) {
        nButton.className += " btn btn-mini HideDTTTClass";
    };
    return DataTable;
})();
//# sourceMappingURL=DataTable.js.map
