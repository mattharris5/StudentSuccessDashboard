using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web;

namespace SSD.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FileSizeAttribute : ValidationAttribute
    {
        public FileSizeAttribute()
            : this(1, int.MaxValue)
        { }

        public FileSizeAttribute(int minSize, int maxSize)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public int MinSize { get; set; }
        public int MaxSize { get; set; }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return true;
            }
            return file.ContentLength >= MinSize && file.ContentLength < MaxSize;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, "The content length of {0} file size must be at least {1} and less than {2} bytes.", name, MinSize, MaxSize);
        }
    }
}
