using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class DoesNotEqualAttribute : ValidationAttribute
    {
        public DoesNotEqualAttribute(string invalidValue)
            : base(CreateDefaultErrorMessageFormat(invalidValue))
        {
            InvalidValue = invalidValue;
        }

        public string InvalidValue { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (object.Equals(InvalidValue, value))
            {
                if (validationContext == null)
                {
                    return new ValidationResult(FormatErrorMessage("Value"));
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new string[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }

        private static string CreateDefaultErrorMessageFormat(string invalidValue)
        {
            return "{0} cannot equal '" + invalidValue + "'";
        }
    }
}
