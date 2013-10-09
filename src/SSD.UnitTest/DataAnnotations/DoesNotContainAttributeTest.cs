using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.DataAnnotations
{
    [TestClass]
    public class DoesNotContainAttributeTest
    {
        [TestMethod]
        public void GivenTerm_WhenConstruct_ThenPropertyMatches()
        {
            string expected = "blah";

            var target = new DoesNotContainAttribute(expected);

            Assert.AreEqual(expected, target.Term);
        }

        [TestMethod]
        public void GivenStringWithInvalidElements_WhenIsValid_ThenFalseReturned()
        {
            string test = "mcblah";
            var target = new DoesNotContainAttribute("blah");

            Assert.IsFalse(target.IsValid(test));
        }

        [TestMethod]
        public void GivenStringWithValidElements_WhenIsValid_ThenTrueReturned()
        {
            string test = "test";
            var target = new DoesNotContainAttribute("blah");

            Assert.IsTrue(target.IsValid(test));
        }

        [TestMethod]
        public void GivenInvalidString_WhenFormatErrorMessage_ThenCorrectErrorMessageReturned()
        {
            var target = new DoesNotContainAttribute("blah");
            string actual = target.FormatErrorMessage("mcblah");

            Assert.AreEqual("The field mcblah must match the regular expression '^((?!blah).)*$'.", actual);
        }

        [TestMethod]
        public void GivenValueContainsTerm_AndValidationContext_WhenValidate_ThenThrowException()
        {
            TestEntity toValidate = new TestEntity { Value = "mcblah" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new DoesNotContainAttribute("blah");

            target.ExpectException<ValidationException>(() => target.Validate(toValidate.Value, validationContext));
        }

        [TestMethod]
        public void GivenValueDoesntContainTerm_AndValidationContext_WhenValidate_ThenSucceed()
        {
            TestEntity toValidate = new TestEntity { Value = "test" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new DoesNotContainAttribute("blah");

            target.Validate(toValidate.Value, validationContext);
        }

        private class TestEntity
        {
            public string Value { get; set; }
        }
    }
}
