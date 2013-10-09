using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class WidgetManagerTest : BaseManagerTest
    {
        private WidgetManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new WidgetManager(Repositories.MockRepositoryContainer);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new WidgetManager(null));
        }

        [TestMethod]
        public void WhenICallGenerateServiceRequestsBySchoolModel_ThenServiceRequestsBySchoolModelIsReturned()
        {
            var actual = Target.GenerateServiceRequestsBySchoolModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenICallGenerateServiceRequestsBySchoolModel_ThenModelHasAListOfServiceRequestsBySchool()
        {
            var actual = Target.GenerateServiceRequestsBySchoolModel();

            Assert.IsTrue(actual.Count() > 0);
        }

        [TestMethod]
        public void GivenThereAreSchools_WhenGenerateServiceRequestsBySchoolModel_ThenTheNumberOfRecordsMatchesTheNumberOfSchools()
        {
            var actual = Target.GenerateServiceRequestsBySchoolModel();

            Assert.AreEqual(Data.Schools.Count(), actual.Count());
        }

        [TestMethod]
        public void GivenSchoolHasServiceRequests_WhenGenerateServiceRequestsBySchoolModel_ThenTotalsMatchOpenAndFulfilled()
        {
            var actual = Target.GenerateServiceRequestsBySchoolModel();

            Assert.AreEqual(actual.ElementAt(0).Total, actual.ElementAt(0).Open + actual.ElementAt(0).Fulfilled);
            Assert.AreEqual(actual.ElementAt(1).Total, actual.ElementAt(1).Open + actual.ElementAt(1).Fulfilled);
        }

        [TestMethod]
        public void GivenSomeServiceRequestsAreRejected_WhenGenerateServiceRequestsBySchoolModel_ThenThoseArentReflectedInTheTotal()
        {
            int totalServiceRequests = Data.ServiceRequests.Where(s => s.Student.SchoolId == 1).Count();

            var actual = Target.GenerateServiceRequestsBySchoolModel();

            Assert.AreNotEqual(totalServiceRequests, actual.ElementAt(1).Total);
        }

        [TestMethod]
        public void WhenGenerateServiceRequestsBySchoolModel_ThenViewModelsSortedBySchoolName()
        {
            var expectedServiceRequestsBySchool = Data.Schools.Select(t => t.Name).OrderBy(n => n).ToList();

            var actual = Target.GenerateServiceRequestsBySchoolModel();

            CollectionAssert.AreEqual(expectedServiceRequestsBySchool, actual.Select(m => m.SchoolName).ToList());
        }

        [TestMethod]
        public void WhenGenerateServiceTypeMetricModels_ThenReturnViewModels()
        {
            var actual = Target.GenerateServiceTypeMetricModels();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenSomeInactiveServiceTypes_WhenGenerateServiceTypeMetricModels_ThenOnlyActiveServiceTypesCounted()
        {
            int expected = Data.ServiceTypes.Where(s => s.IsActive).Count();

            var actual = Target.GenerateServiceTypeMetricModels().Count();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhenGenerateServiceTypeMetricModels_ThenViewModelsHaveServiceTypeNames()
        {
            var expectedServiceTypeNames = Data.ServiceTypes.Where(s => s.IsActive).Select(t => t.Name).ToList();

            var actual = Target.GenerateServiceTypeMetricModels();

            CollectionAssert.AreEquivalent(expectedServiceTypeNames, actual.Select(m => m.ServiceTypeName).ToList());
        }

        [TestMethod]
        public void WhenGenerateServiceTypeMetricModels_ThenViewModelsSortedByServiceTypeName()
        {
            var expectedServiceTypeNames = Data.ServiceTypes.Where(s => s.IsActive).Select(t => t.Name).OrderBy(n => n).ToList();

            var actual = Target.GenerateServiceTypeMetricModels();

            CollectionAssert.AreEqual(expectedServiceTypeNames, actual.Select(m => m.ServiceTypeName).ToList());
        }

        [TestMethod]
        public void WhenGenerateServiceTypeMetricModels_ThenViewModelsHaveProviderCounts()
        {
            int expectedProviderCount = 2;
            RebuildTestDataWithAllActiveStudentAssignments();

            var actual = Target.GenerateServiceTypeMetricModels();

            Assert.AreEqual(expectedProviderCount, actual.Single().ProviderCount);
        }

        [TestMethod]
        public void GivenAssignedOfferingOnlyInactive_WhenGenerateServiceTypeMetricModels_ThenViewModelProviderCountSkipsInactive()
        {
            int expectedProviderCount = 2;
            RebuildTestDataWithOneInactiveStudentAssignments();

            var actual = Target.GenerateServiceTypeMetricModels();

            Assert.AreEqual(expectedProviderCount, actual.Single().ProviderCount);
        }

        [TestMethod]
        public void WhenGenerateServiceTypeMetricModels_ThenViewModelsHavePercentOfStudentsServed()
        {
            int expectedPercentServed = 50;
            RebuildTestDataWithAllActiveStudentAssignments();

            var actual = Target.GenerateServiceTypeMetricModels();

            Assert.AreEqual(expectedPercentServed, actual.Single().PercentOfStudentsBeingServed);
        }

        [TestMethod]
        public void GivenAssignedOfferingOnlyInactive_WhenGenerateServiceTypeMetricModels_ThenViewModelPercentOfStudentsServedSkipsInactive()
        {
            int expectedPercentServed = 50;
            RebuildTestDataWithOneInactiveStudentAssignments();

            var actual = Target.GenerateServiceTypeMetricModels();

            Assert.AreEqual(expectedPercentServed, actual.Single().PercentOfStudentsBeingServed);
        }

        private void RebuildTestDataWithAllActiveStudentAssignments()
        {
            Data.Students.Clear();
            Data.Providers.Clear();
            Data.ServiceTypes.Clear();
            Data.ServiceOfferings.Clear();
            Data.StudentAssignedOfferings.Clear();
            Data.Students.Clear();
            Data.Providers.Add(new Provider());
            Data.Providers.Add(new Provider());
            Data.Providers.Add(new Provider());
            Data.ServiceTypes.Add(new ServiceType { IsActive = true });
            Data.ServiceOfferings.Add(new ServiceOffering { Provider = Data.Providers[0], ServiceType = Data.ServiceTypes[0] });
            Data.ServiceOfferings.Add(new ServiceOffering { Provider = Data.Providers[1], ServiceType = Data.ServiceTypes[0] });
            Data.ServiceOfferings.Add(new ServiceOffering { Provider = Data.Providers[2], ServiceType = Data.ServiceTypes[0] });
            Data.ServiceTypes.Single().ServiceOfferings = Data.ServiceOfferings;
            Data.Students.Add(new Student());
            Data.Students.Add(new Student());
            Data.Students.Add(new Student());
            Data.Students.Add(new Student());
            Data.StudentAssignedOfferings.Add(new StudentAssignedOffering { ServiceOffering = Data.ServiceOfferings[0], Student = Data.Students[0], IsActive = true });
            Data.StudentAssignedOfferings.Add(new StudentAssignedOffering { ServiceOffering = Data.ServiceOfferings[2], Student = Data.Students[1], IsActive = true });
            Data.ServiceOfferings[0].StudentAssignedOfferings.Add(Data.StudentAssignedOfferings[0]);
            Data.ServiceOfferings[2].StudentAssignedOfferings.Add(Data.StudentAssignedOfferings[1]);
        }

        private void RebuildTestDataWithOneInactiveStudentAssignments()
        {
            Data.Students.Clear();
            Data.Providers.Clear();
            Data.ServiceTypes.Clear();
            Data.ServiceOfferings.Clear();
            Data.StudentAssignedOfferings.Clear();
            Data.Students.Clear();
            Data.Providers.Add(new Provider());
            Data.Providers.Add(new Provider());
            Data.Providers.Add(new Provider());
            Data.ServiceTypes.Add(new ServiceType { IsActive = true });
            Data.ServiceOfferings.Add(new ServiceOffering { Provider = Data.Providers[0], ServiceType = Data.ServiceTypes[0] });
            Data.ServiceOfferings.Add(new ServiceOffering { Provider = Data.Providers[1], ServiceType = Data.ServiceTypes[0] });
            Data.ServiceOfferings.Add(new ServiceOffering { Provider = Data.Providers[2], ServiceType = Data.ServiceTypes[0] });
            Data.ServiceTypes.Single().ServiceOfferings = Data.ServiceOfferings;
            Data.Students.Add(new Student());
            Data.Students.Add(new Student());
            Data.Students.Add(new Student());
            Data.Students.Add(new Student());
            Data.StudentAssignedOfferings.Add(new StudentAssignedOffering { ServiceOffering = Data.ServiceOfferings[0], Student = Data.Students[0], IsActive = true });
            Data.StudentAssignedOfferings.Add(new StudentAssignedOffering { ServiceOffering = Data.ServiceOfferings[1], Student = Data.Students[1] });
            Data.StudentAssignedOfferings.Add(new StudentAssignedOffering { ServiceOffering = Data.ServiceOfferings[2], Student = Data.Students[2], IsActive = true });
            Data.ServiceOfferings[0].StudentAssignedOfferings.Add(Data.StudentAssignedOfferings[0]);
            Data.ServiceOfferings[1].StudentAssignedOfferings.Add(Data.StudentAssignedOfferings[1]);
            Data.ServiceOfferings[2].StudentAssignedOfferings.Add(Data.StudentAssignedOfferings[2]);
        }
    }
}
