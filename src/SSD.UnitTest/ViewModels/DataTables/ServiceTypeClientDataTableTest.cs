using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class ServiceTypeClientDataTableTest
    {
        private HttpContextBase MockContext { get; set; }
        private ServiceTypeClientDataTable Target { get; set; }
        private TestData TestData { get; set; }
        private EducationSecurityPrincipal User { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockHttpContextFactory.Create();
            TestData = new TestData();
            User = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
        }

        [TestMethod]
        public void GivenServiceTypeNameSearchCriteria_WhenIConstruct_ThenServiceTypeNameSet()
        {
            string expected = "Basic Needs";
            MockRequestParameter("ServiceTypeName", expected);

            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.AreEqual(expected, Target.ServiceTypeName);
        }

        [TestMethod]
        public void GivenServiceTypeNameSearchCriteria_AndServiceTypeContainsSearchCriteria_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            string searchCriteria = "Basic";
            ServiceType type = new ServiceType { Name = "Student " + searchCriteria + " Needs", IsActive = true };
            MockRequestParameter("ServiceTypeName", searchCriteria);
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenNoServiceTypeNameSearchCriteria_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            ServiceType type = new ServiceType { IsActive = true };
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenEmptyServiceTypeNameSearchCriteria_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            ServiceType type = new ServiceType { IsActive = true };
            MockRequestParameter("ServiceTypeName", string.Empty);
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceTypeNameSearchCriteria_AndServiceTypeDoesNotContainSearchCriteria_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            string searchCriteria = "Basic";
            ServiceType type = new ServiceType { Name = "Student Needs" };
            MockRequestParameter("ServiceTypeName", searchCriteria);
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceTypeNameSearchCriteria_AndServiceTypeContainsSearchCriteria_AndCaseMismatched_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            string searchCriteria = "BaSiC";
            ServiceType type = new ServiceType { Name = "Student " + searchCriteria.ToUpper() + " Needs", IsActive = true };
            MockRequestParameter("ServiceTypeName", searchCriteria);
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceTypeInactive_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            ServiceType type = new ServiceType();
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceType_AndNoSortInformation_WhenInvokeSortSelector_ThenReturnName()
        {
            string expected = "Basic Needs";
            ServiceType type = new ServiceType { Name = expected };
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.AreEqual(expected, Target.SortSelector.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceType_AndSortOnFirstColumn_WhenInvokeSortSelector_ThenReturnIsPrivate()
        {
            bool expected = true;
            ServiceType type = new ServiceType { IsPrivate = expected };
            PrepareDataTableRequestParameters("0");
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.AreEqual(expected.ToString(), Target.SortSelector.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceType_AndSortOnSecondColumn_WhenInvokeSortSelector_ThenReturnName()
        {
            string expected = "Basic Needs";
            ServiceType type = new ServiceType { Name = expected };
            PrepareDataTableRequestParameters("1");
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.AreEqual(expected, Target.SortSelector.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceType_AndSortOnFourthColumn_WhenInvokeSortSelector_ThenReturnDescription()
        {
            string expected = "This is the string to be sorted on";
            ServiceType type = new ServiceType { Description = expected };
            PrepareDataTableRequestParameters("3");
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            Assert.AreEqual(expected, Target.SortSelector.Compile().Invoke(type));
        }

        [TestMethod]
        public void GivenServiceType_WhenInvokeDataSelector_ThenReturnObjectWithServiceTypeState()
        {
            string expectedName = "Basic Needs";
            bool expectedIsPrivate = true;
            string expectedDescription = "This is a test service type.";
            ServiceType type = new ServiceType
            {
                Name = expectedName,
                IsPrivate = expectedIsPrivate,
                Description = expectedDescription
            };
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = Target.DataSelector.Compile().Invoke(type);

            Assert.AreEqual(expectedName, actual.Name);
            Assert.AreEqual(expectedIsPrivate, actual.IsPrivate);
            Assert.AreEqual(expectedDescription, actual.Description);
        }

        [TestMethod]
        public void GivenServiceType_AndUserIsAdministrator_WhenInvokeDataSelector_ThenIsEditableTrue()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });
            ServiceType type = new ServiceType();
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = Target.DataSelector.Compile().Invoke(type);

            Assert.IsTrue(actual.IsEditable);
        }

        [TestMethod]
        public void GivenServiceType_AndUserIsNotAdministrator_WhenInvokeDataSelector_ThenIsEditableFalse()
        {
            ServiceType type = new ServiceType();
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = Target.DataSelector.Compile().Invoke(type);

            Assert.IsFalse(actual.IsEditable);
        }

        [TestMethod]
        public void GivenServiceType_AndServiceTypeHasServiceOfferingsWithPrograms_WhenInvokeDataSelector_ThenReturnObjectHasProgramNames()
        {
            string[] expected = new[] { "Apple Picking", "Youth Group", "After School Tutoring" };
            ServiceType type = new ServiceType
            {
                ServiceOfferings = expected.Select(name => new ServiceOffering { Program = new Program { Name = name, IsActive = true }, IsActive = true }).ToList()
            };
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = Target.DataSelector.Compile().Invoke(type);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Programs).ToList());
        }

        [TestMethod]
        public void GivenServiceType_AndServiceTypeHasServiceOfferingsWithPrograms_AndServiceTypeHasInactiveServiceOfferings_WhenInvokeDataSelector_ThenReturnObjectHasActiveProgramNames()
        {
            string[] expected = new[] { "Apple Picking", "After School Tutoring" };
            ServiceType type = new ServiceType
            {
                ServiceOfferings = expected.Select(name => new ServiceOffering { Program = new Program { Name = name, IsActive = true }, IsActive = true }).ToList()
            };
            type.ServiceOfferings.Add(new ServiceOffering { Program = new Program { Name = "Youth Group", IsActive = false }, IsActive = false });
            Target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = Target.DataSelector.Compile().Invoke(type);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Programs).ToList());
        }

        [TestMethod]
        public void GivenAServiceType_WhenExecuteDataSelector_ThenIsPrivateMatches()
        {
            ServiceType serviceType = TestData.ServiceTypes[0];
            ServiceTypeClientDataTable target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = target.DataSelector.Compile().Invoke(serviceType);

            Assert.AreEqual(serviceType.IsPrivate, actual.IsPrivate);
        }

        [TestMethod]
        public void GivenAServiceType_WhenExecuteDataSelector_ThenNamesMatch()
        {
            ServiceType serviceType = TestData.ServiceTypes[0];
            ServiceTypeClientDataTable target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = target.DataSelector.Compile().Invoke(serviceType);

            Assert.AreEqual(serviceType.Name, actual.Name);
        }

        [TestMethod]
        public void GivenAServiceType_WhenExecuteDataSelector_ThenDescriptionsMatch()
        {
            ServiceType serviceType = TestData.ServiceTypes[0];
            ServiceTypeClientDataTable target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = target.DataSelector.Compile().Invoke(serviceType);

            Assert.AreEqual(serviceType.Description, actual.Description);
        }

        [TestMethod]
        public void GivenAServiceType_WhenExecuteDataSelector_ThenIdsMatch()
        {
            ServiceType serviceType = TestData.ServiceTypes[0];
            ServiceTypeClientDataTable target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = target.DataSelector.Compile().Invoke(serviceType);

            Assert.AreEqual(serviceType.Id, actual.Id);
        }

        [TestMethod]
        public void GivenAServiceTypeWithNoAccessRights_WhenExecuteDataSelector_ThenIsEditableIsFalse()
        {
            ServiceType serviceType = TestData.ServiceTypes[0];
            ServiceTypeClientDataTable target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = target.DataSelector.Compile().Invoke(serviceType);

            Assert.IsFalse(actual.IsEditable);
        }

        [TestMethod]
        public void GivenAServiceTypeWithAccessRights_WhenExecuteDataSelector_ThenIsEditableIsTrue()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });
            ServiceType serviceType = TestData.ServiceTypes[0];
            ServiceTypeClientDataTable target = new ServiceTypeClientDataTable(MockContext.Request, User);

            dynamic actual = target.DataSelector.Compile().Invoke(serviceType);

            Assert.IsTrue(actual.IsEditable);
        }

        private void MockRequestParameter(string paramName, string paramValue)
        {
            if (paramValue != null)
            {
                MockContext.Request.Expect(m => m[paramName]).Return(paramValue);
            }
        }

        private void PrepareDataTableRequestParameters(string sortColumn)
        {
            MockContext.Request.Expect(m => m["iSortCol_0"]).Return(sortColumn);
            MockContext.Request.Expect(m => m["sSortDir_0"]).Return("asc");
        }
    }
}
