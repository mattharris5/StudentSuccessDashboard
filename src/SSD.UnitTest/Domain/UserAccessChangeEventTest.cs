using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace SSD.Domain
{
    [TestClass]
    public class UserAccessChangeEventTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            UserAccessChangeEvent actual = new UserAccessChangeEvent();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }

        [TestMethod]
        public void GivenNullAccessData_WhenGetAccessXml_ThenGetEmptyXElement()
        {
            UserAccessChangeEvent target = new UserAccessChangeEvent { AccessData = null };

            Assert.IsTrue(target.AccessXml.IsEmpty);
        }

        [TestMethod]
        public void GivenEmptyAccessData_WhenGetAccessXml_ThenGetEmptyXElement()
        {
            UserAccessChangeEvent target = new UserAccessChangeEvent { AccessData = string.Empty };

            Assert.IsTrue(target.AccessXml.IsEmpty);
        }

        [TestMethod]
        public void GivenWhitespaceAccessData_WhenGetAccessXml_ThenGetEmptyXElement()
        {
            UserAccessChangeEvent target = new UserAccessChangeEvent { AccessData = " \r\n\t" };

            Assert.IsTrue(target.AccessXml.IsEmpty);
        }

        [TestMethod]
        public void GivenXmlAccessData_WhenGetAccessXml_ThenGetEmptyXElement()
        {
            UserAccessChangeEvent target = new UserAccessChangeEvent { AccessData = "<data>bob</data>" };

            Assert.AreEqual("data", target.AccessXml.Name);
            Assert.AreEqual("bob", target.AccessXml.Value);
        }

        [TestMethod]
        public void GivenNullAccessXml_WhenGetAccessData_ThenGetNull()
        {
            UserAccessChangeEvent target = new UserAccessChangeEvent { AccessXml = null };

            Assert.IsNull(target.AccessData);
        }

        [TestMethod]
        public void GivenAccessXml_WhenGetAccessData_ThenGetXmlAsString()
        {
            XElement xml = new XElement("data", "bob");
            UserAccessChangeEvent target = new UserAccessChangeEvent { AccessXml = xml };

            Assert.AreEqual(xml.ToString(), target.AccessData);
        }
    }
}
