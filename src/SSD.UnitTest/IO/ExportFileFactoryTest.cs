using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD.IO
{
    [TestClass]
    public class ExportFileFactoryTest
    {
        [TestMethod]
        public void GivenNullType_WhenCreate_ThenThrowException()
        {
            ExportFileFactory target = new ExportFileFactory();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.Create(null));
        }

        [TestMethod]
        public void GivenUnrecognizedType_WhenCreate_ThenThrowException()
        {
            ExportFileFactory target = new ExportFileFactory();

            target.ExpectException<InvalidOperationException>(() => target.Create(typeof(string)));
        }

        [TestMethod]
        public void GivenStudentProfileExportFileType_WhenCreate_ThenStudentProfileExportFileReturned()
        {
            ExportFileFactory target = new ExportFileFactory();

            var result = target.Create(typeof(StudentProfileExportFile)) as StudentProfileExportFile;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenNullIExportFileFactory_WhenSetCurrent_ThenArgumentNullException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => ExportFileFactory.SetCurrent(null));
        }

        [TestMethod]
        public void GivenValidIExportFileFactory_WhenSetCurrent_ThenCurrentIExportFileFactoryIsSetToPassedInIExportFileFactory()
        {
            IExportFileFactory expected = MockRepository.GenerateMock<IExportFileFactory>();

            ExportFileFactory.SetCurrent(expected);

            Assert.AreEqual(expected, ExportFileFactory.Current);
        }
    }
}
