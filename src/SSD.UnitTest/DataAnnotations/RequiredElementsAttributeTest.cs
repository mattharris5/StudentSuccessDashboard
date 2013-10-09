using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace SSD.DataAnnotations
{
    [TestClass]
    public class RequiredElementsAttributeTest
    {
        [TestMethod]
        public void GivenCollectionWithElements_WhenIValidate_ThenTrueReturned()
        {
            IEnumerable<int> test = new List<int> { 1, 2 };

            var target = new RequiredElementsAttribute();

            Assert.IsTrue(target.IsValid(test));
        }

        [TestMethod]
        public void GivenCollectionWithNoElements_WhenIsValid_ThenFalseReturned()
        {
            IEnumerable<int> test = new List<int>();

            var target = new RequiredElementsAttribute();

            Assert.IsFalse(target.IsValid(test));
        }

        [TestMethod]
        public void GivenObjectThatIsNotOfTypeIEnumerable_WhenIsValid_ThenFalseReturned()
        {
            int test = 0;

            var target = new RequiredElementsAttribute();

            Assert.IsFalse(target.IsValid(test));
        }

        [TestMethod]
        public void WhenICallFormatErrorMessage_ThenErrorMessageReturned()
        {
            var target = new RequiredElementsAttribute();

            Assert.AreEqual("Collection must contain at least one element", target.FormatErrorMessage(null));
        }

        [TestMethod]
        public void GivenEntityListWithNoElements_AndValidationContext_WhenValidate_ThenThrowException()
        {
            List<TestEntity> toValidate = new List<TestEntity>();
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new RequiredElementsAttribute();

            target.ExpectException<ValidationException>(() => target.Validate(toValidate, validationContext));
        }

        [TestMethod]
        public void GivenEntityListWithElements_AndValidationContext_WhenValidate_ThenSucceed()
        {
            List<TestEntity> toValidate = new List<TestEntity> { new TestEntity { Value = "blah" } };
            ValidationContext validationContext = new ValidationContext(toValidate) { DisplayName = "Test Entity Value", MemberName = "Value" };
            var target = new RequiredElementsAttribute();

            target.Validate(toValidate, validationContext);
        }

        private class TestEntity
        {
            public string Value { get; set; }
        }
    }
}
