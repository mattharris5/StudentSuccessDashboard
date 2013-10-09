using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSD.Domain
{
    [TestClass]
    public class RoleTest
    {
        [TestMethod]
        public void WhenIConstruct_ThenUserListIsNotNull()
        {
            Assert.IsNotNull(new Role().UserRoles);
        }
    }
}
