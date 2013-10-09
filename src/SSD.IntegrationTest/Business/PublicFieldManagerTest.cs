using Castle.Windsor;
using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using SSD.IO;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSD.Business
{
    [TestClass]
    public class PublicFieldManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private EducationSecurityPrincipal User { get; set; }
        private PublicFieldManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            User = new EducationSecurityPrincipal(EducationContext.Users.Include("UserRoles.Role").Include(u => u.PrivateHealthDataViewEvents).Single(u => u.Id == 1));
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new PublicFieldManager(repositoryContainer, MockRepository.GenerateMock<IBlobClient>(), new DataTableBinder(), new UserAuditor());
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            if (EducationContext != null)
            {
                EducationContext.Dispose();
            }
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenSelectedCategoriesPopulated()
        {
            CustomFieldModel actual = Target.GenerateEditViewModel(1, User);

            Assert.AreNotEqual(0, actual.SelectedCategories.Count());
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenAuditDataPopulated()
        {
            CustomField data = EducationContext.CustomFields.Single(f => f.Name == "Tardies");

            CustomFieldModel actual = Target.GenerateEditViewModel(data.Id, User);

            Assert.IsNotNull(actual.Audit.CreatedBy);
            Assert.AreNotEqual(DateTime.MinValue, actual.Audit.CreateTime);
            Assert.IsNotNull(actual.Audit.LastModifiedBy);
            Assert.IsTrue(actual.Audit.LastModifyTime.HasValue);
        }

        [TestMethod]
        public void GivenDataExtractModelWithSelectedCustomFields_WhenRetrieveStudentsList_ThenCustomFieldsIncluded()
        {
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedCustomFieldIds = EducationContext.CustomFields.Select(f => f.Id) };

            var result = Target.RetrieveStudentsList(model);

            Assert.IsTrue(result.Any(r => r.CustomFieldValues.Count > 0));
        }

        [TestMethod]
        public void GivenDataExtractModelWithSelectedServiceTypes_WhenRetreiveStudentsList_ThenStudentAssignedOfferingsIncluded()
        {
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedServiceTypeIds = EducationContext.ServiceTypes.Select(t => t.Id) };

            var result = Target.RetrieveStudentsList(model);

            Assert.IsTrue(result.First().StudentAssignedOfferings.Count() > 0);
        }

        [TestMethod]
        public void GivenDataExtractModelWithBothSelected_WhenRetrieveStudentsList_ThenCustomFieldsIncluded_AndStudentAssignedOfferingsIncluded()
        {
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedCustomFieldIds = EducationContext.CustomFields.Select(f => f.Id), SelectedServiceTypeIds = EducationContext.ServiceTypes.Select(t => t.Id) };

            var result = Target.RetrieveStudentsList(model);

            Assert.IsTrue(result.Any(r => r.CustomFieldValues.Count > 0));
            Assert.IsTrue(result.Any(r => r.StudentAssignedOfferings.Count > 0));
        }

        [TestMethod]
        public void GivenModel_WhenGenerateStudentProfileExport_ThenStreamReturnedContainsExcelFile()
        {
            StudentProfileExportModel model = new StudentProfileExportModel
            {
                SelectedCustomFieldIds = new List<int> { EducationContext.CustomFields.First().Id },
                SelectedServiceTypeIds = new List<int> { EducationContext.ServiceTypes.First().Id },
                SelectedSchoolIds = new List<int> { EducationContext.Schools.First().Id },
                SelectedGrades = new List<int> { 10, 12 }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as MemoryStream;

            Assert.IsNotNull(result);
            File.WriteAllBytes(outputPath, result.ToArray());
            using (var workbook = new XLWorkbook(outputPath))
            {
                Assert.IsNotNull(workbook);
            }
        }

        [TestMethod]
        public void GivenModel_AndCustomFieldSelected_WhenGenerateStudentProfileExport_ThenFileContainsCustomFieldValue()
        {
            StudentProfileExportModel model = new StudentProfileExportModel
            {
                SelectedCustomFieldIds = new List<int> { EducationContext.CustomFields.Where(c => c.Name == "Tardies").Single().Id },
                SelectedSchoolIds = new List<int> { EducationContext.Schools.First().Id },
                SelectedGrades = new List<int> { 10, 12 }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as MemoryStream;

            File.WriteAllBytes(outputPath, result.ToArray());
            using (var workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual("1200", worksheet.Cell("E3").Value);
            }
        }

        [TestMethod]
        public void GivenModel_AndServiceTypeSelected_WhenGenerateStudentProfileExport_ThenFileContainsServiceTypeValue()
        {
            StudentProfileExportModel model = new StudentProfileExportModel
            {
                SelectedServiceTypeIds = new List<int> { 2 },
                SelectedSchoolIds = new List<int> { EducationContext.Schools.First().Id },
                SelectedGrades = new List<int> { 10, 12 }
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as MemoryStream;

            File.WriteAllBytes(outputPath, result.ToArray());
            using (var workbook = new XLWorkbook(outputPath))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                Assert.AreEqual("Big Brothers, Big Sisters Provide College Access/One on One Activities", worksheet.Cell("E3").Value);
            }
        }

        [TestMethod]
        public void GivenModel_AndAllFieldsSelected_AndAllSchoolsSelected_WhenGenerateStudentProfileExport_ThenFileContainsServiceTypeValue()
        {
            StudentProfileExportModel model = new StudentProfileExportModel
            {
                SelectedServiceTypeIds = EducationContext.ServiceTypes.Select(t => t.Id).ToList(),
                SelectedSchoolIds = EducationContext.Schools.Select(s => s.Id).ToList(),
                SelectedCustomFieldIds = EducationContext.CustomFields.Select(f => f.Id).ToList(),
                BirthDateIncluded = true,
                ParentNameIncluded = true
            };
            string outputPath = Path.ChangeExtension(Path.Combine("TestData", MethodBase.GetCurrentMethod().Name), ".xlsx");

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as MemoryStream;

            File.WriteAllBytes(outputPath, result.ToArray());
            using (var workbook = new XLWorkbook(outputPath))
            {
                Assert.IsNotNull(workbook);
            }
        }
    }
}
