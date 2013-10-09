
class StringExtensions {

    static endsWith(targetString :string, suffix :string): boolean {
        /// <summary>Determines whether the end of this instance matches the specified string.</summary>
        /// <param name="suffix" type="String">A string to compare to.</param>
        /// <returns type="booleanean">true if suffix matches the end of this instance; otherwise, false.</returns>
        return (targetString.substr(targetString.length - suffix.length) === suffix);
    }

    static startsWith(targetString :string, prefix :string): boolean {
        /// <summary >Determines whether the beginning of this instance matches the specified string.</summary>
        /// <param name="prefix" type="String">The String to compare.</param>
        /// <returns type="booleanean">true if prefix matches the beginning of this string; otherwise, false.</returns>
        return (targetString.substr(0, prefix.length) === prefix);
    }

    static trim(targetString :string): string {
        /// <summary >Removes all leading and trailing white-space characters from the current String object.</summary>
        /// <returns type="String">The string that remains after all white-space characters are removed from the start and end of the current String object.</returns>
        return targetString.replace(/^\s+|\s+$/g, '');
    }

    static trimEnd(targetString :string): string {
        /// <summary >Removes all trailing white spaces from the current String object.</summary>
        /// <returns type="String">The string that remains after all white-space characters are removed from the end of the current String object.</returns>
        return targetString.replace(/\s+$/, '');
    }

    static trimStart(targetString :string): string {
        /// <summary >Removes all leading white spaces from the current String object.</summary>
        /// <returns type="String">The string that remains after all white-space characters are removed from the start of the current String object.</returns>
        return targetString.replace(/^\s+/, '');
    }

    static format(formatString: string, ...args: string[]): string {
        var regex :RegExp = new RegExp("{-?[0-9]+}", "g")
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
    }

    static parseJsonDateTime(jsonDateTime: string): Date {
        return new Date(parseInt(jsonDateTime.substr(6)));
    }
}