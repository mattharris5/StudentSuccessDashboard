using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace SSD.DataAnnotations
{
    [TestClass]
    public class FileSizeAttributeTest
    {
        [TestMethod]
        public void WhenConstruct_ThenMinSizeAndMaxSizeDefaultsSet()
        {
            var target = new FileSizeAttribute();

            Assert.AreEqual(1, target.MinSize);
            Assert.AreEqual(int.MaxValue, target.MaxSize);
        }

        [TestMethod]
        public void GivenMaxSize_WhenConstruct_ThenPropertyMatches()
        {
            int expected = 100;

            var target = new FileSizeAttribute(0, expected);

            Assert.AreEqual(expected, target.MaxSize);
        }

        [TestMethod]
        public void GivenMinSize_WhenConstruct_ThenPropertyMatches()
        {
            int expected = 100;

            var target = new FileSizeAttribute(expected, int.MaxValue);

            Assert.AreEqual(expected, target.MinSize);
        }

        [TestMethod]
        public void GivenSizeExceedsMax_WhenIsValid_ThenFalseReturned()
        {
            HttpPostedFileBase test = MockRepository.GenerateMock<HttpPostedFileBase>();
            test.Expect(t => t.ContentLength).Return(200);
            var target = new FileSizeAttribute(0, 100);

            Assert.IsFalse(target.IsValid(test));
        }

        [TestMethod]
        public void GivenSizeFallsShortOfMin_WhenIsValid_ThenFalseReturned()
        {
            HttpPostedFileBase test = MockRepository.GenerateMock<HttpPostedFileBase>();
            test.Expect(t => t.ContentLength).Return(50);
            var target = new FileSizeAttribute(100, 200);

            Assert.IsFalse(target.IsValid(test));
        }

        [TestMethod]
        public void GivenValidSize_WhenIsValid_ThenTrueReturned()
        {
            HttpPostedFileBase test = MockRepository.GenerateMock<HttpPostedFileBase>();
            test.Expect(t => t.ContentLength).Return(50);
            var target = new FileSizeAttribute(0, 100);

            Assert.IsTrue(target.IsValid(test));
        }

        [TestMethod]
        public void GivenSizeExceedsMax_WhenValidate_ThenThrowException()
        {
            HttpPostedFileBase test = MockRepository.GenerateMock<HttpPostedFileBase>();
            test.Expect(t => t.ContentLength).Return(200);
            var target = new FileSizeAttribute(0, 100);

            target.ExpectException<ValidationException>(() => target.Validate(test, "blah"));
        }

        [TestMethod]
        public void GivenSizeFallsShortOfMin_WhenValidate_ThenThrowException()
        {
            HttpPostedFileBase test = MockRepository.GenerateMock<HttpPostedFileBase>();
            test.Expect(t => t.ContentLength).Return(20);
            var target = new FileSizeAttribute(100, 200);

            target.ExpectException<ValidationException>(() => target.Validate(test, "blah"));
        }

        [TestMethod]
        public void GivenValidSize_WhenValidate_ThenSucceed()
        {
            HttpPostedFileBase test = MockRepository.GenerateMock<HttpPostedFileBase>();
            test.Expect(t => t.ContentLength).Return(50);
            var target = new FileSizeAttribute(0, 100);

            target.Validate(test, "blah");
        }

        [TestMethod]
        public void GivenMinSize_AndMaxSize_WhenFormatErrorMessage_ThenCorrectErrorMessageReturned()
        {
            var target = new FileSizeAttribute(20, 100);
            string actual = target.FormatErrorMessage("FileProperty");

            Assert.AreEqual("The content length of FileProperty file size must be at least 20 and less than 100 bytes.", actual);
        }
    }
}
