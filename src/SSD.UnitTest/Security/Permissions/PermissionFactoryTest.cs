using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Collections.Generic;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class PermissionFactoryTest
    {
        [TestMethod]
        public void GivenNullActivityName_WhenCreate_ThenThrowException()
        {
            PermissionFactory target = new PermissionFactory();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.Create(null, new Dictionary<string, object>()));
        }

        [TestMethod]
        public void GivenEmptyActivityName_WhenCreate_ThenThrowException()
        {
            PermissionFactory target = new PermissionFactory();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.Create(string.Empty, new Dictionary<string, object>()));
        }

        [TestMethod]
        public void GivenActivityNameOnlyWhitespace_WhenCreate_ThenThrowException()
        {
            PermissionFactory target = new PermissionFactory();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.Create("\r\n \t", new Dictionary<string, object>()));
        }

        [TestMethod]
        public void GivenViewStudentDetail_WhenCreate_ThenReturnViewStudentDetailPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("ViewStudentDetail", new Student());

            Assert.IsInstanceOfType(actual, typeof(ViewStudentDetailPermission));
        }

        [TestMethod]
        public void GivenCreateStudentNameString_WhenCreate_ThenReturnViewStudentDetailPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("CreateStudentNameString", new Student());

            Assert.IsInstanceOfType(actual, typeof(ViewStudentDetailPermission));
        }

        [TestMethod]
        public void GivenSetServiceTypePrivacy_WhenCreate_ThenReturnSetServiceTypePrivacyPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("SetServiceTypePrivacy");

            Assert.IsInstanceOfType(actual, typeof(SetServiceTypePrivacyPermission));
        }

        [TestMethod]
        public void GivenEditRequest_WhenCreate_ThenReturnManageServiceRequestPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("EditRequest", new ServiceRequest());

            Assert.IsInstanceOfType(actual, typeof(ManageServiceRequestPermission));
        }

        [TestMethod]
        public void GivenDeleteRequest_WhenCreate_ThenReturnManageServiceRequestPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("DeleteRequest", new ServiceRequest());

            Assert.IsInstanceOfType(actual, typeof(ManageServiceRequestPermission));
        }

        [TestMethod]
        public void WhenCreateServiceRequestString_WhenCreate_ThenReturnManageServiceRequestPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("CreateServiceRequestString", new ServiceRequest());

            Assert.IsInstanceOfType(actual, typeof(ManageServiceRequestPermission));
        }

        [TestMethod]
        public void GivenEditScheduledOffering_WhenCreate_ThenReturnManageAssignedOfferingPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("EditScheduledOffering", new StudentAssignedOffering());

            Assert.IsInstanceOfType(actual, typeof(ManageAssignedOfferingPermission));
        }

        [TestMethod]
        public void GivenDeleteScheduledOffering_WhenCreate_ThenReturnManageAssignedOfferingPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("DeleteScheduledOffering", new StudentAssignedOffering());

            Assert.IsInstanceOfType(actual, typeof(ManageAssignedOfferingPermission));
        }

        [TestMethod]
        public void GivenEditProvider_WhenCreate_ThenReturnManageProviderPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("EditProvider", 1);

            Assert.IsInstanceOfType(actual, typeof(ManageProviderPermission));
        }

        [TestMethod]
        public void GivenSetFavoriteServiceOffering_WhenCreate_ThenReturnManageProviderPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("SetFavoriteServiceOffering", 1);

            Assert.IsInstanceOfType(actual, typeof(ManageProviderPermission));
        }

        [TestMethod]
        public void GivenCreateServiceRequest_WhenCreate_ThenReturnCreateServiceRequestPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("CreateServiceRequest", new List<Student>());

            Assert.IsInstanceOfType(actual, typeof(CreateServiceRequestPermission));
        }

        [TestMethod]
        public void GivenProcessDataFile_WhenCreate_ThenReturnManageCustomFieldPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("ProcessDataFile", new Student());

            Assert.IsInstanceOfType(actual, typeof(ManageCustomFieldPermission));
        }

        [TestMethod]
        public void GivenCreateServiceAttendance_WhenCreate_ThenReturnManageServiceAttendancePermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("CreateServiceAttendance", new StudentAssignedOffering());

            Assert.IsInstanceOfType(actual, typeof(ManageServiceAttendancePermission));
        }

        [TestMethod]
        public void GivenEditServiceAttendance_WhenCreate_ThenReturnManageServiceAttendancePermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("EditServiceAttendance", new StudentAssignedOffering());

            Assert.IsInstanceOfType(actual, typeof(ManageServiceAttendancePermission));
        }

        [TestMethod]
        public void GivenDeleteServiceAttendance_WhenCreate_ThenReturnManageServiceAttendancePermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("DeleteServiceAttendance", new StudentAssignedOffering());

            Assert.IsInstanceOfType(actual, typeof(ManageServiceAttendancePermission));
        }

        [TestMethod]
        public void GivenScheduleOffering_WhenCreate_ThenReturnScheduleOfferingPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("ScheduleOffering", new List<Student> { new Student() }, new ServiceOffering());

            Assert.IsInstanceOfType(actual, typeof(ScheduleOfferingPermission));
        }

        [TestMethod]
        public void GivenImportOfferingData_WhenCreate_ThenReturnImportOfferingDataPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("ImportOfferingData", new ServiceOffering());

            Assert.IsInstanceOfType(actual, typeof(ImportOfferingDataPermission));
        }

        [TestMethod]
        public void GivenGenerateStudentProfileExport_WhenCreate_ThenReturnViewStudentDetailPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("StudentProfileExportMapData", new Student());

            Assert.IsInstanceOfType(actual, typeof(ViewStudentDetailPermission));
        }

        [TestMethod]
        public void GivenUploadCustomFieldData_WhenCreate_ThenReturnCustomFieldDataPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("UploadCustomFieldData", new PublicField());

            Assert.IsInstanceOfType(actual, typeof(CustomFieldDataPermission));
        }

        [TestMethod]
        public void GivenViewStudentCustomFieldData_WhenCreate_ThenReturnCustomFieldDataPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("ViewStudentCustomFieldData", new PublicField());

            Assert.IsInstanceOfType(actual, typeof(CustomFieldDataPermission));
        }

        [TestMethod]
        public void GivenStudentProfileExportCustomFieldData_WhenCreate_ThenReturnCustomFieldDataPermission()
        {
            PermissionFactory target = new PermissionFactory();

            IPermission actual = target.Create("StudentProfileExportCustomFieldData", new PublicField());

            Assert.IsInstanceOfType(actual, typeof(CustomFieldDataPermission));
        }

        [TestMethod]
        public void GivenUnrecognizedActivityName_WhenCreate_ThenThrowException()
        {
            PermissionFactory target = new PermissionFactory();

            target.ExpectException<InvalidOperationException>(() => target.Create("gibberish!!"));
        }

        [TestMethod]
        public void GivenNullIPermissionFactory_WhenSetCurrent_ThenArgumentNullException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => PermissionFactory.SetCurrent(null));
        }

        [TestMethod]
        public void GivenValidIPermissionFactory_WhenSetCurrent_ThenCurrentIPermissionFactoryIsSetToPassedInIPermissionFactory()
        {
            IPermissionFactory expected = MockRepository.GenerateMock<IPermissionFactory>();

            PermissionFactory.SetCurrent(expected);

            Assert.AreEqual(expected, PermissionFactory.Current);
        }
    }
}
