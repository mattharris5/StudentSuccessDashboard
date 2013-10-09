using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSD.Domain
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void WhenIConstruct_ThenSchoolsIsNotNull()
        {
            Program target = new Program();
            Assert.IsNotNull(target.Schools);
        }

        [TestMethod]
        public void WhenIConstruct_ThenServiceOfferingsIsNotNull()
        {
            Program target = new Program();
            Assert.IsNotNull(target.ServiceOfferings);
        }

        [TestMethod]
        public void WhenIConstruct_ThenContactIsNotNull()
        {
            Program target = new Program();
            Assert.IsNotNull(target.ContactInfo);
        }
    }
}
