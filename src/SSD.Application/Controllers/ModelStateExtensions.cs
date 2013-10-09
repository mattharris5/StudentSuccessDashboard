using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Controllers
{
    public static class ModelStateExtensions
    {
        public static void AddModelErrors(this ModelStateDictionary modelState, ValidationException exception)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException("modelState");
            }
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            if (exception.ValidationResult.MemberNames.Any())
            {
                foreach (string property in exception.ValidationResult.MemberNames)
                {
                    modelState.AddModelError(property, exception.ValidationResult.ErrorMessage);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, exception.Message);
            }
        }
    }
}
