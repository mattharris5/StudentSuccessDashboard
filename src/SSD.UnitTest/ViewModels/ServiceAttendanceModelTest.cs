using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class ServiceAttendanceModelTest
    {
        private ServiceAttendanceModel Target { get; set; }
        private TestData TestData { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceAttendanceModel();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyTo_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyTo(null));
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyTo_ThenAllFieldsAreCopied()
        {
            var actual = new ServiceAttendance();
            Target.CopyFrom(TestData.ServiceAttendances[0]);

            Target.CopyTo(actual);

            Assert.AreEqual(actual.StudentAssignedOfferingId, Target.StudentAssignedOfferingId);
            Assert.AreEqual(actual.DateAttended, Target.DateAttended);
            Assert.AreEqual(actual.SubjectId, Target.SelectedSubjectId);
            Assert.AreEqual(actual.Duration, Target.Duration);
            Assert.AreEqual(actual.Notes, Target.Notes);
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyFrom(null));
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyFrom_ThenAllFieldsAreCopied()
        {
            var expected = TestData.ServiceAttendances[0];
            
            Target.CopyFrom(TestData.ServiceAttendances[0]);

            Assert.AreEqual(expected.Id, Target.Id);
            Assert.AreEqual(expected.StudentAssignedOfferingId, Target.StudentAssignedOfferingId);
            Assert.AreEqual(expected.DateAttended, Target.DateAttended);
            Assert.AreEqual(expected.SubjectId, Target.SelectedSubjectId);
            Assert.AreEqual(expected.Duration, Target.Duration);
            Assert.AreEqual(expected.Notes, Target.Notes);
        }

        [TestMethod]
        public void GivenModelHasAuditData_WhenCopyFrom_ThenModelStateSet()
        {
            ServiceAttendance expectedState = new ServiceAttendance
            {
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" },
                LastModifyTime = new DateTime(2010, 5, 13),
                LastModifyingUser = new User { DisplayName = "jimGeorge" }
            };
            ServiceAttendanceModel target = new ServiceAttendanceModel();

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
            ServiceAttendance expectedState = new ServiceAttendance
            {
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            ServiceAttendanceModel target = new ServiceAttendanceModel();

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }

        [TestMethod]
        public void GivenModelNotModified_AndViewModelAuditDataAlreadySet_WhenCopyFrom_ThenModelStatelastModifyValuesNull()
        {
            ServiceAttendance expectedState = new ServiceAttendance
            {
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            ServiceAttendanceModel target = new ServiceAttendanceModel();
            target.Audit = new AuditModel { LastModifiedBy = "bob", LastModifyTime = DateTime.Now };

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }
    }
}
