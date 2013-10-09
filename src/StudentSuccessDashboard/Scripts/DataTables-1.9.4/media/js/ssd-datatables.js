
/* Note 'unshift' does not work in IE6. A simply array concatenation would. This is used
     * to give the custom type top priority
     */
jQuery.fn.dataTableExt.aTypes.unshift(
    function (sData) {
        var sValidChars = "0123456789-,";
        var Char;
        var bDecimal = false;

        /* Check the numeric part */
        for (i = 0 ; i < sData.length ; i++) {
            Char = sData.charAt(i);
            if (sValidChars.indexOf(Char) == -1) {
                return null;
            }

            /* Only allowed one decimal place... */
            if (Char == ",") {
                if (bDecimal) {
                    return null;
                }
                bDecimal = true;
            }
        }

        return 'numeric-comma';
    }
);

jQuery.fn.dataTableExt.oSort['numeric-comma-asc'] = function (a, b) {
    var x = (a == "-") ? 0 : a.replace(/,/, ".");
    var y = (b == "-") ? 0 : b.replace(/,/, ".");
    x = parseFloat(x);
    y = parseFloat(y);
    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['numeric-comma-desc'] = function (a, b) {
    var x = (a == "-") ? 0 : a.replace(/,/, ".");
    var y = (b == "-") ? 0 : b.replace(/,/, ".");
    x = parseFloat(x);
    y = parseFloat(y);
    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};

//setting filter delay so that the browser is more responsive
jQuery.fn.dataTableExt.oApi.fnSetFilteringDelay = function (oSettings, iDelay) {
    var _that = this;

    if (iDelay === undefined) {
        iDelay = 250;
    }

    this.each(function (i) {
        $.fn.dataTableExt.iApiIndex = i;
        var
            $this = this,
            oTimerId = null,
            sPreviousSearch = null,
            anControl = $('input', _that.fnSettings().aanFeatures.f);

        anControl.unbind('keyup').bind('keyup', function () {
            var $$this = $this;

            if (sPreviousSearch === null || sPreviousSearch != anControl.val()) {
                window.clearTimeout(oTimerId);
                sPreviousSearch = anControl.val();
                oTimerId = window.setTimeout(function () {
                    $.fn.dataTableExt.iApiIndex = i;
                    _that.fnFilter(anControl.val());
                }, iDelay);
            }
        });

        return this;
    });
    return this;
};