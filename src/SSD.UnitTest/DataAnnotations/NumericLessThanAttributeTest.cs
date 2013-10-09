using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.DataAnnotations
{
    [TestClass]
    public class NumericLessThanAttributeTest
    {
        [TestMethod]
        public void GivenNullOtherProperty_WhenConstruct_ThenPropertyMatches()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new NumericLessThanAttribute(null));
        }

        [TestMethod]
        public void GivenOtherProperty_WhenConstruct_ThenPropertyMatches()
        {
            string expected = "blah";

            var target = new NumericLessThanAttribute(expected);

            Assert.AreEqual(expected, target.OtherProperty);
        }

        [TestMethod]
        public void GivenPropertyIsGreaterThan_AndValidationContext_WhenValidate_ThenThrowException()
        {
            TestEntity toValidate = new TestEntity { Value = "10" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value");

            ValidationException actual = target.ExpectException<ValidationException>(() => target.Validate("20", validationContext));
            Assert.AreEqual("Test Entity Value must be less than Value.", actual.Message);
        }

        [TestMethod]
        public void GivenPropertyIsGreaterThan_AndAllowEquality_AndValidationContext_WhenValidate_ThenThrowExceptionWithEqualityMessage()
        {
            TestEntity toValidate = new TestEntity { Value = "10" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value") { AllowEquality = true };

            ValidationException actual = target.ExpectException<ValidationException>(() => target.Validate("20", validationContext));
            Assert.AreEqual("Test Entity Value must be less than or equal to Value.", actual.Message);
        }

        [TestMethod]
        public void GivenInvalidNumber_AndValidationContext_WhenValidate_ThenThrowException()
        {
            TestEntity toValidate = new TestEntity { Value = "10" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value");

            ValidationException actual = target.ExpectException<ValidationException>(() => target.Validate("blah", validationContext));
            Assert.AreEqual("Test Entity Value is not a numeric value.", actual.Message);
        }

        [TestMethod]
        public void GivenInvalidValue_AndValidationContext_WhenValidate_ThenThrowException()
        {
            TestEntity toValidate = new TestEntity { Value = "blah" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value");

            ValidationException actual = target.ExpectException<ValidationException>(() => target.Validate("20", validationContext));
            Assert.AreEqual("Value is not a numeric value.", actual.Message);
        }

        [TestMethod]
        public void GivenInvalidProperty_AndValidationContext_WhenValidate_ThenThrowException()
        {
            TestEntity toValidate = new TestEntity { Value = "10" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("blah");

            ValidationException actual = target.ExpectException<ValidationException>(() => target.Validate("10", validationContext));
            Assert.AreEqual("Could not find a property named blah.", actual.Message);
        }

        [TestMethod]
        public void GivenValidProperty_AndValidationContext_WhenValidate_ThenSucceed()
        {
            TestEntity toValidate = new TestEntity { Value = "20" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value");

            target.Validate("10", validationContext);
        }

        [TestMethod]
        public void GivenNumbersEqual_AndAllowEquality_AndValidationContext_WhenValidate_ThenSucceed()
        {
            TestEntity toValidate = new TestEntity { Value = "20" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value") { AllowEquality = true };

            target.Validate("20", validationContext);
        }

        [TestMethod]
        public void GivenNumbersEqual_AndDontAllowEquality_AndValidationContext_WhenValidate_ThenThrowException()
        {
            TestEntity toValidate = new TestEntity { Value = "20" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new NumericLessThanAttribute("Value") { AllowEquality = false };

            ValidationException actual = target.ExpectException<ValidationException>(() => target.Validate("20", validationContext));
            Assert.AreEqual("AllowEquality has been set to false so values cannot be equal.", actual.Message);
        }

        private class TestEntity
        {
            public string Value { get; set; }
        }
    }
}
