using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;

namespace SSD.ViewModels
{
    [TestClass]
    public class ServiceRequestModelTest
    {
        private ServiceRequestModel Target { get; set; }
        private TestData TestData { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceRequestModel();
            TestData = new TestData();
        }

        [TestMethod]
        public void WhenContruct_ThenDefaultsSet()
        {
            Target = new ServiceRequestModel();

            Assert.AreEqual(1, Target.SelectedPriorityId);
            Assert.AreEqual(1, Target.SelectedSubjectId);
            Assert.AreEqual(1, Target.SelectedStatusId);
        }

        [TestMethod]
        public void GivenZero_WhenSetSelectedPriorityId_ThenThrowException()
        {
            Target.ExpectException<ArgumentException>(() => Target.SelectedPriorityId = 0);
        }

        [TestMethod]
        public void GivenZero_WhenSetSelectedSubjectId_ThenThrowException()
        {
            Target.ExpectException<ArgumentException>(() => Target.SelectedSubjectId = 0);
        }

        [TestMethod]
        public void GivenZero_WhenSetSelectedStatusId_ThenThrowException()
        {
            Target.ExpectException<ArgumentException>(() => Target.SelectedStatusId = 0);
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyTo_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyTo(null));
        }

        [TestMethod]
        public void GivenNullModel_WhenCopyFrom_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CopyFrom(null));
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyTo_ThenFieldsAreIdentical()
        {
            ServiceRequest expected = TestData.ServiceRequests[0];
            ServiceRequest actual = new ServiceRequest();

            Target.CopyFrom(expected);
            Target.CopyTo(actual);

            Assert.AreEqual(expected.PriorityId, actual.PriorityId);
            Assert.AreEqual(expected.SubjectId, actual.SubjectId);
            Assert.AreEqual(expected.ServiceTypeId, actual.ServiceTypeId);
            Assert.AreEqual(expected.Notes, actual.Notes);
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyFrom_ThenFieldsAreIdentical()
        {
            ServiceRequest expected = TestData.ServiceRequests[0];
            expected.FulfillmentDetails = TestData.ServiceRequestFulfillments;

            Target.CopyFrom(expected);

            Assert.AreEqual(expected.Id, Target.Id);
            Assert.AreEqual(expected.PriorityId, Target.SelectedPriorityId);
            Assert.AreEqual(expected.ServiceTypeId, Target.SelectedServiceTypeId);
            Assert.AreEqual(expected.SubjectId, Target.SelectedSubjectId);
            Assert.AreEqual(expected.Notes, Target.Notes);
            Assert.AreEqual(TestData.ServiceRequestFulfillments[3].FulfilledById, Target.SelectedAssignedOfferingId);
            Assert.AreEqual(TestData.ServiceRequestFulfillments[3].FulfillmentStatusId, Target.SelectedStatusId);
        }

        [TestMethod]
        public void GivenModelHasAuditData_WhenCopyFrom_ThenModelStateSet()
        {
            ServiceRequest expectedState = new ServiceRequest
            {
                PriorityId = 1,
                SubjectId = 1,
                FulfillmentDetails = new List<ServiceRequestFulfillment> { new ServiceRequestFulfillment { FulfillmentStatusId = 1 } },
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" },
                LastModifyTime = new DateTime(2010, 5, 13),
                LastModifyingUser = new User { DisplayName = "jimGeorge" }
            };
            ServiceRequestModel target = new ServiceRequestModel();

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
            ServiceRequest expectedState = new ServiceRequest
            {
                PriorityId = 1,
                SubjectId = 1,
                FulfillmentDetails = new List<ServiceRequestFulfillment> { new ServiceRequestFulfillment { FulfillmentStatusId = 1 } },
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            ServiceRequestModel target = new ServiceRequestModel();

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }

        [TestMethod]
        public void GivenModelNotModified_AndViewModelAuditDataAlreadySet_WhenCopyFrom_ThenModelStatelastModifyValuesNull()
        {
            ServiceRequest expectedState = new ServiceRequest
            {
                PriorityId = 1,
                SubjectId = 1,
                FulfillmentDetails = new List<ServiceRequestFulfillment> { new ServiceRequestFulfillment { FulfillmentStatusId = 1 } },
                CreateTime = new DateTime(2005, 4, 30),
                CreatingUser = new User { DisplayName = "fredBob" }
            };
            ServiceRequestModel target = new ServiceRequestModel();
            target.Audit = new AuditModel { LastModifiedBy = "bob", LastModifyTime = DateTime.Now };

            target.CopyFrom(expectedState);

            Assert.IsNull(target.Audit.LastModifiedBy);
            Assert.IsFalse(target.Audit.LastModifyTime.HasValue);
        }
    }
}
