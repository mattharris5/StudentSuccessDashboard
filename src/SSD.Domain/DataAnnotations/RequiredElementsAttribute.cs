using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace SSD.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredElementsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            IEnumerable list = value as IEnumerable;
            return (list != null && list.GetEnumerator().MoveNext());
        }

        public override string FormatErrorMessage(string name)
        {
            return ("Collection must contain at least one element");
        }
    }
}
