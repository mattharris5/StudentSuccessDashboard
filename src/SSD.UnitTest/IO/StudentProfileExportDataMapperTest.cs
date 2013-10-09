using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security;
using SSD.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.IO
{
    [TestClass]
    public class StudentProfileExportDataMapperTest
    {
        StudentProfileExportDataMapper Target { get; set; }
        TestData TestData { get; set; }
        EducationSecurityPrincipal User { get; set; }
        IUserAuditor MockUserAuditor { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new StudentProfileExportDataMapper();
            TestData = new TestData();
            User = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            PermissionFactory.SetCurrent(MockRepository.GenerateMock<IPermissionFactory>());
            MockUserAuditor = MockRepository.GenerateMock<IUserAuditor>();
            User.Identity.User.PrivateHealthDataViewEvents = new List<PrivateHealthDataViewEvent>();
        }

        [TestMethod]
        public void GivenModel_WhenMapColumnHeadings_ThenExpectedStringsReturned()
        {
            List<string> expected = new List<String> { "School Description", "Grade", "Student Name", "Student ID", TestData.CustomFields[0].Name, TestData.ServiceTypes[0].Name };
            var model = new StudentProfileExportFieldDescriptor { SelectedCustomFields = new List<CustomField> { TestData.CustomFields[0] }, SelectedServiceTypes = new List<ServiceType> { TestData.ServiceTypes[0] } };

            var actual = Target.MapColumnHeadings(model);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithBirthDateIncluded_WhenMapColumnHeadings_ThenExpectedStringsReturned()
        {
            List<string> expected = new List<String> { "School Description", "Grade", "Student Name", "Student ID", "Birth Date", TestData.CustomFields[0].Name, TestData.ServiceTypes[0].Name };
            var model = new StudentProfileExportFieldDescriptor { SelectedCustomFields = new List<CustomField> { TestData.CustomFields[0] }, SelectedServiceTypes = new List<ServiceType> { TestData.ServiceTypes[0] } };
            model.BirthDateIncluded = true;

            var actual = Target.MapColumnHeadings(model);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithParentNameIncluded_WhenMapColumnHeadings_ThenExpectedStringsReturned()
        {
            List<string> expected = new List<String> { "School Description", "Grade", "Student Name", "Student ID", "Parent Name", TestData.CustomFields[0].Name, TestData.ServiceTypes[0].Name };
            var model = new StudentProfileExportFieldDescriptor { SelectedCustomFields = new List<CustomField> { TestData.CustomFields[0] }, SelectedServiceTypes = new List<ServiceType> { TestData.ServiceTypes[0] } };
            model.ParentNameIncluded = true;

            var actual = Target.MapColumnHeadings(model);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenNullStudent_WhenMapData_ThenExceptionThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.MapData(new StudentProfileExportFieldDescriptor(), null, User, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullUser_WhenMapData_ThenExceptionThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.MapData(new StudentProfileExportFieldDescriptor(), new Student(), null, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullAuditor_WhenMapData_ThenExceptionThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.MapData(new StudentProfileExportFieldDescriptor(), new Student(), User, null));
        }

        [TestMethod]
        public void GivenNoSelectedCustomFields_AndNoSelectedServiceTypes_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.StudentSISId };
            var model = new StudentProfileExportFieldDescriptor();
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithBirthDateIncluded_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.StudentSISId, student.DateOfBirth };
            var model = new StudentProfileExportFieldDescriptor { BirthDateIncluded = true };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithParentNameIncluded_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.StudentSISId, student.Parents };
            var model = new StudentProfileExportFieldDescriptor { ParentNameIncluded = true };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithSelectedCustomFields_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.StudentSISId, student.CustomFieldValues.First().Value, student.CustomFieldValues.Last().Value };
            var model = new StudentProfileExportFieldDescriptor { SelectedCustomFields = new List<CustomField> { TestData.CustomFields[0], TestData.CustomFields[2] } };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithSelectedCustomFields_AndStudentDoesntHaveAllTheCustomFields_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.StudentSISId, "1200", "1201", "1202", "", "", "", "" };
            var model = new StudentProfileExportFieldDescriptor { SelectedCustomFields = TestData.CustomFields.OfType<PublicField>() };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithSelectedServiceTypes_AndSomeStudentAssignedOfferingsInactive_WhenMapData_ThenOnlyActiveStudentAssignedOfferingsReturned()
        {
            Student student = TestData.Students[0];
            student.StudentAssignedOfferings.Clear();
            student.StudentAssignedOfferings.Add(new StudentAssignedOffering { StudentId = 1, ServiceOfferingId = 1, ServiceOffering = TestData.ServiceOfferings[0], IsActive = false });
            var model = new StudentProfileExportFieldDescriptor { SelectedServiceTypes = new List<ServiceType> { TestData.ServiceTypes[1] } };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);
            var services = actual.ElementAt(4) as List<string>;

            Assert.AreEqual(0, services.Count());
        }

        [TestMethod]
        public void GivenModelWithSelectedServiceTypes_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            var offering = student.StudentAssignedOfferings.First().ServiceOffering;
            List<object> expected = new List<object> { offering.Name };
            var model = new StudentProfileExportFieldDescriptor { SelectedServiceTypes = new List<ServiceType> { offering.ServiceType } };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);
            var services = actual.ElementAt(4) as List<string>;

            CollectionAssert.AreEqual(expected, services);
        }

        [TestMethod]
        public void GivenSelectedServiceHasProgramName_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            var offering = student.StudentAssignedOfferings.First().ServiceOffering;
            List<object> expected = new List<object> { offering.Name };
            var model = new StudentProfileExportFieldDescriptor { SelectedServiceTypes = new List<ServiceType> { offering.ServiceType } };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);
            var services = actual.ElementAt(4) as List<string>;

            CollectionAssert.AreEqual(expected, services);
        }

        [TestMethod]
        public void GivenModelWithSelectedCustomFields_AndSelectedServiceTypes_WhenMapData_ThenExpectedObjectsReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.StudentSISId, student.CustomFieldValues.First().Value, student.CustomFieldValues.Last().Value };
            var offering = student.StudentAssignedOfferings.First().ServiceOffering;
            List<object> expectedServices = new List<object> { offering.Name };
            var model = new StudentProfileExportFieldDescriptor { SelectedCustomFields = new List<CustomField> { TestData.CustomFields[0], TestData.CustomFields[2] }, SelectedServiceTypes = new List<ServiceType> { offering.ServiceType } };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());

            var actual = Target.MapData(model, student, User, MockUserAuditor);
            var actualServices = actual.ElementAt(6) as List<string>;

            CollectionAssert.AreEqual(expected.ToList(), actual.Take(6).ToList());
            CollectionAssert.AreEqual(expectedServices, actualServices);
        }

        [TestMethod]
        public void GivenPermissionDoesNotGrantAccess_WhenMapData_ThenOnlyDirectoryLevelInformationReturned()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName };
            var model = new StudentProfileExportFieldDescriptor { SelectedServiceTypes = new List<ServiceType> { TestData.ServiceTypes[1] } };
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(permission);

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenModelWithBirthDateIncluded_AndParentNameIncluded_AndSelectedCustomFields_AndSelectedServiceTypes_AndPermissionGrantsCustomFieldOnly_WhenMapData_ThenResultContainsOnlyDirectoryLevelData()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, null, null, student.CustomFieldValues.First().Value, student.CustomFieldValues.Last().Value };
            var model = new StudentProfileExportFieldDescriptor
            {
                BirthDateIncluded = true,
                ParentNameIncluded = true,
                SelectedCustomFields = new List<CustomField> { TestData.CustomFields[0], TestData.CustomFields[2] },
                SelectedServiceTypes = new List<ServiceType> { TestData.ServiceTypes[1] }
            };
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            permission.Expect(p => p.CustomFieldOnly).Return(true);
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(permission);

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenSelectedCustomFields_WhenMapData_ThenUserHasNewPrivateHealthDataViewEvents()
        {
            Student student = TestData.Students[0];
            List<object> expected = new List<object> { student.School.Name, student.Grade, student.FullName, student.Id, null, null, student.CustomFieldValues.First().Value, student.CustomFieldValues.Last().Value };
            var model = new StudentProfileExportFieldDescriptor
            {
                SelectedCustomFields = TestData.CustomFields
            };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportMapData", student)).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            MockUserAuditor.Expect(m => m.CreatePrivateHealthInfoViewEvent(User.Identity.User, student.CustomFieldValues.ToList())).Return(new PrivateHealthDataViewEvent());

            var actual = Target.MapData(model, student, User, MockUserAuditor);

            Assert.AreEqual(1, User.Identity.User.PrivateHealthDataViewEvents.Count());
        }
    }
}
