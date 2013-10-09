using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class ServiceOfferingScheduleModelTest
    {
        private ServiceOfferingScheduleModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceOfferingScheduleModel();
        }

        [TestMethod]
        public void WhenICopyToDomainEntity_ThenDomainEntityContainsState()
        {
            StudentAssignedOffering destination = new StudentAssignedOffering();
            Target.Id = 12;
            Target.EndDate = new DateTime(2004, 4, 5);
            Target.Notes = "swkldfjwoi";
            Target.ServiceOfferingId = 3483;
            Target.StartDate = new DateTime(2001, 6, 21);
            Target.CopyTo(destination);

            //Assert.AreEqual(Target.Id, destination.Id);
            Assert.AreEqual(Target.EndDate, destination.EndDate);
            Assert.AreEqual(Target.Notes, destination.Notes);
            Assert.AreEqual(Target.ServiceOfferingId, destination.ServiceOfferingId);
            Assert.AreEqual(Target.StartDate, destination.StartDate);
        }

        [TestMethod]
        public void WhenICopyFromADomainEntity_ThenViewModelContainsState()
        {
            StudentAssignedOffering source = new StudentAssignedOffering { Id = 12, EndDate = new DateTime(2004, 4, 5), Notes = "swkldfjwoi", ServiceOfferingId = 3483, StartDate = new DateTime(2001, 6, 21) };
            Target.CopyFrom(source);

            Assert.AreEqual(source.Id, Target.Id);
            Assert.AreEqual(source.EndDate, Target.EndDate);
            Assert.AreEqual(source.Notes, Target.Notes);
            Assert.AreEqual(source.ServiceOfferingId, Target.ServiceOfferingId);
            Assert.AreEqual(source.StartDate, Target.StartDate);
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
    }
}
