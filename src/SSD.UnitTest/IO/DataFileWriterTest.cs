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
    public class DataFileWriterTest
    {
        private DataFileWriter Target { get; set; }
        
        [TestInitialize]
        public void InitializeTest()
        {
            Target = new DataFileWriter();
        }

        [TestMethod]
        public void GivenBlobContainerIsNull_WhenIWrite_ThenAnArgumentNullExceptionIsThrown()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Write(null, "blah"));
        }
    }
}
