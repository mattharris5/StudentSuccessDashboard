var StringExtensions = (function () {
    function StringExtensions() {
    }
    StringExtensions.endsWith = function (targetString, suffix) {
        /// <summary>Determines whether the end of this instance matches the specified string.</summary>
        /// <param name="suffix" type="String">A string to compare to.</param>
        /// <returns type="booleanean">true if suffix matches the end of this instance; otherwise, false.</returns>
        return (targetString.substr(targetString.length - suffix.length) === suffix);
    };

    StringExtensions.startsWith = function (targetString, prefix) {
        /// <summary >Determines whether the beginning of this instance matches the specified string.</summary>
        /// <param name="prefix" type="String">The String to compare.</param>
        /// <returns type="booleanean">true if prefix matches the beginning of this string; otherwise, false.</returns>
        return (targetString.substr(0, prefix.length) === prefix);
    };

    StringExtensions.trim = function (targetString) {
        /// <summary >Removes all leading and trailing white-space characters from the current String object.</summary>
        /// <returns type="String">The string that remains after all white-space characters are removed from the start and end of the current String object.</returns>
        return targetString.replace(/^\s+|\s+$/g, '');
    };

    StringExtensions.trimEnd = function (targetString) {
        /// <summary >Removes all trailing white spaces from the current String object.</summary>
        /// <returns type="String">The string that remains after all white-space characters are removed from the end of the current String object.</returns>
        return targetString.replace(/\s+$/, '');
    };

    StringExtensions.trimStart = function (targetString) {
        /// <summary >Removes all leading white spaces from the current String object.</summary>
        /// <returns type="String">The string that remains after all white-space characters are removed from the start of the current String object.</returns>
        return targetString.replace(/^\s+/, '');
    };

    StringExtensions.format = function (formatString) {
        var args = [];
        for (var _i = 0; _i < (arguments.length - 1); _i++) {
            args[_i] = arguments[_i + 1];
        }
        var regex = new RegExp("{-?[0-9]+}", "g");
        var str = formatString;
        return str.replace(regex, function (item) {
            var intVal = parseInt(item.substring(1, item.length - 1));
            var replace;
            if (intVal >= 0 && args[intVal] !== null) {
                replace = args[intVal];
            } else if (intVal === -1) {
                replace = "{";
            } else if (intVal === -2) {
                replace = "}";
            } else {
                replace = "";
            }
            return replace;
        });
    };

    StringExtensions.parseJsonDateTime = function (jsonDateTime) {
        return new Date(parseInt(jsonDateTime.substr(6)));
    };
    return StringExtensions;
})();
//# sourceMappingURL=StringExtensions.js.map
