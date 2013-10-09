using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class StudentServiceOfferingScheduleModelTest
    {
        [TestMethod]
        public void GivenNullStudentAssignedOffering_WhenCopyTo_ThenThrowException()
        {
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.CopyTo(null));
        }

        [TestMethod]
        public void GivenNullStudentAssignedOffering_WhenCopyFrom_ThenThrowException()
        {
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();

            TestExtensions.ExpectException<ArgumentNullException>(() => target.CopyFrom(null));
        }

        [TestMethod]
        public void GivenValidStudentAssignedOffering_WhenCopyTo_ThenModelHasViewModelData()
        {
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel { Id = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), Notes = "blah" };
            StudentAssignedOffering actual = new StudentAssignedOffering();

            target.CopyTo(actual);

            Assert.AreEqual(target.StartDate, actual.StartDate);
            Assert.AreEqual(target.EndDate, actual.EndDate);
            Assert.AreEqual(target.Notes, actual.Notes);
        }

        [TestMethod]
        public void GivenValidStudentAssignedOffering_WhenCopyFrom_ThenViewModelHasModelData()
        {
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();
            StudentAssignedOffering offering = new StudentAssignedOffering
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Notes = "blah",
                ServiceOffering = new ServiceOffering
                {
                    Provider = new Provider(),
                    ServiceType = new ServiceType(),
                    Program = new Program()
                },
                CreatingUser = new User()
            };

            target.CopyFrom(offering);

            Assert.AreEqual(target.Id, offering.Id);
            Assert.AreEqual(target.StartDate, offering.StartDate);
            Assert.AreEqual(target.EndDate, offering.EndDate);
            Assert.AreEqual(target.Notes, offering.Notes);
        }

        [TestMethod]
        public void GivenValidStudentAssignedOffering_WhenCopyFrom_ThenViewModelHasName()
        {
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();
            StudentAssignedOffering offering = new StudentAssignedOffering
            {
                ServiceOffering = new ServiceOffering
                {
                    Provider = new Provider { Name = "provider name" },
                    ServiceType = new ServiceType { Name = "service type name" },
                    Program = new Program { Name = "program name" }
                },
                CreatingUser = new User()
            };

            target.CopyFrom(offering);

            Assert.AreEqual(target.Name, offering.ServiceOffering.Name);
        }

        [TestMethod]
        public void GivenModelHasAuditData_WhenCopyFrom_ThenModelStateSet()
        {
            StudentAssignedOffering expectedState = new StudentAssignedOffering
            {
                ServiceOffering = new ServiceOffering
                {
                    Provider = new Provider { Name = "provider name" },
                    ServiceType = new ServiceType { Name = "service type name" },
                    Program = new Program { Name = "program name" }
                },
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" },
                LastModifyTime = new DateTime(2010, 5, 13),
                LastModifyingUser = new User { DisplayName = "jimGeorge" }
            };
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();

            target.CopyFrom(expectedState);

            AuditModel actualState = target.Audit;
            Assert.AreEqual(expectedState.CreateTime, actualState.CreateTime);
            Assert.AreEqual(expectedState.CreatingUser.DisplayName, actualState.CreatedBy);
            Assert.AreEqual(expectedState.LastModifyTime, actualState.LastModifyTime);
            Assert.AreEqual(expectedState.LastModifyingUser.DisplayName, actualState.LastModifiedBy);
        }

        [TestMethod]
        public void GivenModelNotModified_WhenCopyFrom_ThenModelStatelastModifyValuesNull()
        {
            StudentAssignedOffering expectedState = new StudentAssignedOffering
            {
                ServiceOffering = new ServiceOffering
                {
                    Provider = new Provider { Name = "provider name" },
                    ServiceType = new ServiceType { Name = "service type name" },
                    Program = new Program { Name = "program name" }
                },
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }

        [TestMethod]
        public void GivenModelNotModified_AndViewModelAuditDataAlreadySet_WhenCopyFrom_ThenModelStatelastModifyValuesNull()
        {
            StudentAssignedOffering expectedState = new StudentAssignedOffering
            {
                ServiceOffering = new ServiceOffering
                {
                    Provider = new Provider { Name = "provider name" },
                    ServiceType = new ServiceType { Name = "service type name" },
                    Program = new Program { Name = "program name" }
                },
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            StudentServiceOfferingScheduleModel target = new StudentServiceOfferingScheduleModel();
            target.Audit = new AuditModel { LastModifiedBy = "bob", LastModifyTime = DateTime.Now };

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }
    }
}
