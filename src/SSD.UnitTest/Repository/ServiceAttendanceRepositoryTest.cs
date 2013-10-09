using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class ServiceAttendanceRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<ServiceAttendance> MockDbSet { get; set; }
        private ServiceAttendanceRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<ServiceAttendance>>();
            MockContext.Expect(m => m.ServiceAttendances).Return(MockDbSet);
            Target = new ServiceAttendanceRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceAttendanceRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsServiceAttendance_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAServiceAttendance_WhenAdd_ThenAddToContext()
        {
            var expected = new ServiceAttendance { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAServiceAttendance_ThenUpdate_ThenContextSetsModified()
        {
            var expected = new ServiceAttendance { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAServiceAttendance_WhenRemove_ThenRemoveFromContext()
        {
            var expected = new ServiceAttendance { Id = 1 };

            Target.Remove(expected);

            MockDbSet.AssertWasCalled(m => m.Remove(expected));
        }
    }
}
