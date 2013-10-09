using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSD.ViewModels
{
    [TestClass]
    public class StudentProfileExportModelTest
    {
        [TestMethod]
        public void GivenModel_WhenConstruct_ThenSelectedCustomFieldsIsNotNull()
        {
            Assert.IsNotNull(new StudentProfileExportModel().SelectedCustomFieldIds);
        }

        [TestMethod]
        public void GivenModel_WhenConstruct_ThenSelectedServiceTypesIsNotNull()
        {
            Assert.IsNotNull(new StudentProfileExportModel().SelectedServiceTypeIds);
        }

        [TestMethod]
        public void GivenModel_WhenConstruct_ThenSelectedSchoolsIsNotNull()
        {
            Assert.IsNotNull(new StudentProfileExportModel().SelectedSchoolIds);
        }

        [TestMethod]
        public void GivenModel_WhenConstruct_ThenSelectedGradesIsNotNull()
        {
            Assert.IsNotNull(new StudentProfileExportModel().SelectedGrades);
        }
    }
}
