using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Web;

namespace SSD.DataAnnotations
{
    [TestClass]
    public class FileTypesAttributeTest
    {
        [TestMethod]
        public void GivenNullTypes_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new FileTypesAttribute(null));
        }

        [TestMethod]
        public void GivenValidType_WhenIValidate_ThenTrueIsReturned()
        {
            var file = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.Expect(f => f.FileName).Return("myfile.test");

            var target = new FileTypesAttribute("test,testing");

            Assert.IsTrue(target.IsValid(file));
        }

        [TestMethod]
        public void GivenInvalidType_WhenIValidate_ThenFalseIsReturned()
        {
            var file = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.Expect(f => f.FileName).Return("myfile.blah");

            var target = new FileTypesAttribute("test,testing");

            Assert.IsFalse(target.IsValid(file));
        }

        [TestMethod]
        public void GivenNull_WhenIValidate_ThenTrueIsReturned()
        {
            var target = new FileTypesAttribute("test,testing");

            Assert.IsTrue(target.IsValid(null));
        }

        [TestMethod]
        public void WhenICallFormateErrorMessage_ThenAFriendlyErrorMessageIsReturned()
        {
            var file = MockRepository.GenerateStub<HttpPostedFileBase>();
            file.Expect(f => f.FileName).Return("myfile.test");

            var target = new FileTypesAttribute("test,testing");

            Assert.AreEqual("The file uploaded is of an invalid format. Please use the recommended (test, testing) format.",
                target.FormatErrorMessage(null));
        }
    }
}
