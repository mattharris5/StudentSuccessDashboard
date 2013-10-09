using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.DataAnnotations
{
    [TestClass]
    public class DoesNotEqualAttributeTest
    {
        private const string TestInvalidValue = "this isn't valid";
        private DoesNotEqualAttribute Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new DoesNotEqualAttribute(TestInvalidValue);
        }

        [TestMethod]
        public void GivenInvalidValue_WhenConstruct_ThenPropertyMatches()
        {
            string expected = "property should match this value";

            DoesNotEqualAttribute target = new DoesNotEqualAttribute(expected);

            Assert.AreEqual(expected, target.InvalidValue);
        }

        [TestMethod]
        public void GivenValueMatchesInvalidValue_WhenCheckIsValide_ThenFalse()
        {
            Assert.IsFalse(Target.IsValid(TestInvalidValue));
        }

        [TestMethod]
        public void GivenValueDoesNotMatchInvalidValue_WhenCheckIsValide_ThenTrue()
        {
            Assert.IsTrue(Target.IsValid("not invalid"));
        }

        [TestMethod]
        public void GivenValueMatchesInvalidValue_AndValidationContext_WhenValidate_ThenThrowValidationException_AndDisplayNameInValidationResult_AndMemberNameInValidationResult()
        {
            TestEntity toValidate = new TestEntity { Value = TestInvalidValue };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };

            ValidationException actual = Target.ExpectException<ValidationException>(() => Target.Validate(toValidate.Value, validationContext));

            CollectionAssert.Contains(actual.ValidationResult.MemberNames.ToList(), validationContext.MemberName);
            Assert.IsTrue(actual.Message.Contains(validationContext.DisplayName));
        }

        [TestMethod]
        public void GivenValueDoesNotMatchInvalidValue_AndValidationContext_WhenValidate_ThenSucceed()
        {
            TestEntity toValidate = new TestEntity { Value = "not invalid" };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };

            Target.Validate(toValidate.Value, validationContext);
        }

        private class TestEntity
        {
            public string Value { get; set; }
        }
    }
}
