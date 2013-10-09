using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace SSD.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FileTypesAttribute : ValidationAttribute
    {
        public FileTypesAttribute(string types)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types");
            }
            Types = types.Split(',').ToList();
        }

        public IList<string> Types { get; private set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            var fileExt = Path.GetExtension((value as HttpPostedFileBase).FileName).Substring(1);
            return Types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, "The file uploaded is of an invalid format. Please use the recommended ({0}) format.", String.Join(", ", Types));
        }
    }
}
