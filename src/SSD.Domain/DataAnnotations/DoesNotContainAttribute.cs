using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class DoesNotContainAttribute : RegularExpressionAttribute
    {
        public DoesNotContainAttribute(string term)
            : base("^((?!" + term + ").)*$")
        {
            Term = term;
        }

        public string Term { get; private set; }
    }
}
