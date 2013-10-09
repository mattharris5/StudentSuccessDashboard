using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.IO
{
    [TestClass]
    public class StudentProfileExportFieldDescriptorTest
    {
        [TestMethod]
        public void WhenConstruct_ThenSelectedCustomFieldsNotNull()
        {
            var target = new StudentProfileExportFieldDescriptor();

            Assert.IsNotNull(target.SelectedCustomFields);
        }

        [TestMethod]
        public void WhenConstruct_ThenSelectedServiceTypesNotNull()
        {
            var target = new StudentProfileExportFieldDescriptor();

            Assert.IsNotNull(target.SelectedServiceTypes);
        }
    }
}
