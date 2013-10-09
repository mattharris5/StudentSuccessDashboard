using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Controllers
{
    [TestClass]
    public class ModelStateExtensionsTest
    {
        [TestMethod]
        public void GivenNullInstance_WhenAddModelErrors_ThenThrowException()
        {
            ModelStateDictionary target = null;
            ValidationException exception = new ValidationException();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.AddModelErrors(exception));
        }

        [TestMethod]
        public void GivenNullException_WhenAddModelErrors_ThenThrowException()
        {
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.AddModelErrors(null));
        }

        [TestMethod]
        public void GivenExceptionWithMultipeMembers_WhenAddModelErrors_ThenModelStateErrorAddedForEachMember()
        {
            string[] expected = new string[] { "Property1", "Property2", "AnotherProperty" };
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException(new ValidationResult(null, expected), null, null);

            target.AddModelErrors(exception);

            CollectionAssert.AreEqual(expected, target.Keys.ToList());
        }

        [TestMethod]
        public void GivenExceptionWithMultipeMembers_WhenAddModelErrors_ThenErrorMessageAddedForEachMember()
        {
            string expected = "this error message should appear for each property";
            string[] properties = new string[] { "Property1", "Property2", "AnotherProperty" };
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException(new ValidationResult(expected, properties), null, null);

            target.AddModelErrors(exception);

            string[] actual = target.Values.Select(v => v.Errors.Single().ErrorMessage).ToArray();
            Assert.AreEqual(properties.Length, actual.Length);
            Assert.AreEqual(expected, actual.Distinct().Single());
        }

        [TestMethod]
        public void GivenExceptionNoMember_WhenAddModelErrors_ThenModelStateErrorAddedWithEmptyKey()
        {
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException(new ValidationResult(null, null), null, null);

            target.AddModelErrors(exception);

            Assert.AreEqual(string.Empty, target.Keys.Single());
        }

        [TestMethod]
        public void GivenExceptionNoMember_WhenAddModelErrors_ThenModelStateErrorAddedWithMessage()
        {
            string expected = "here is the error message";
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException(new ValidationResult(expected, null), null, null);

            target.AddModelErrors(exception);

            Assert.AreEqual(expected, target.Values.Single().Errors.Single().ErrorMessage);
        }

        [TestMethod]
        public void GivenExceptionCreatedWithoutValidationResult_WhenAddModelErrors_ThenModelStateErrorAddedWithEmptyKey()
        {
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException(null);

            target.AddModelErrors(exception);

            Assert.AreEqual(string.Empty, target.Keys.Single());
        }

        [TestMethod]
        public void GivenExceptionCreatedWithoutValidationResult_WhenAddModelErrors_ThenModelStateErrorAddedWithMessage()
        {
            string expected = "even without a validation result this should be the message";
            ModelStateDictionary target = new ModelStateDictionary();
            ValidationException exception = new ValidationException(expected);

            target.AddModelErrors(exception);

            Assert.AreEqual(expected, target.Values.Single().Errors.Single().ErrorMessage);
        }
    }
}
