using System.Web.Mvc;

namespace SSD.DataAnnotations
{
    public class ModelClientValidationNumericGreaterThanRule : ModelClientValidationRule
    {
        public ModelClientValidationNumericGreaterThanRule(string errorMessage, object other, bool allowEquality)
        {
            ErrorMessage = errorMessage;
            ValidationType = "numericgreaterthan";
            ValidationParameters["other"] = other;
            ValidationParameters["allowequality"] = allowEquality;
        }
    }
}
