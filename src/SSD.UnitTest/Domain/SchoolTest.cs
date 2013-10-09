using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.Domain
{

    [TestClass]
    public class SchoolTest
    {
        [TestMethod]
        public void WhenIConstruct_ThenProgramsIsNotNull()
        {
            Assert.IsNotNull(new School().Programs);
        }

        [TestMethod]
        public void WhenIConstruct_ThenUserRolesIsNotNull()
        {
            Assert.IsNotNull(new School().UserRoles);
        }

        [TestMethod]
        public void WhenIConstruct_ThenStudentsIsNotNull()
        {
            Assert.IsNotNull(new School().Students);
        }
    }
}
