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
    public class ServiceOfferingClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }
        private EducationSecurityPrincipal CurrentUser { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            CurrentUser = new EducationSecurityPrincipal(new User { Id = 39, UserKey = "39" });
            MockRequest = MockHttpContextFactory.CreateRequest();
        }

        [TestMethod]
        public void GivenServiceTypeFiltersExistInRequest_WhenIConstruct_ThenServiceTypesPopulated()
        {
            MockRequest.Expect(m => m["ServiceTypeFilters"]).Return("Apples|Oranges|Grapes");
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            CollectionAssert.AreEqual(new string[] { "Apples", "Oranges", "Grapes" }, target.ServiceTypes.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeProviderOrProgramInRequest_WhenIConstruct_ThenServiceTypeProviderOrProgramNamePopulated()
        {
            MockRequest.Expect(m => m["ServiceTypeProviderOrProgram"]).Return("Test");
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            Assert.AreEqual("Test", target.ServiceTypeProviderOrProgramName);
        }

        [TestMethod]
        public void GivenServiceCategoryFiltersExistInRequest_WhenIConstruct_ThenServiceCategoriesPopulated()
        {
            MockRequest.Expect(m => m["ServiceCategoryFilters"]).Return("Tuna|Salmon|Bass");
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            CollectionAssert.AreEqual(new string[] { "Tuna", "Salmon", "Bass" }, target.ServiceCategories.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeFiltersExistInRequest_AndServiceOfferingMatchesFilter_WhenIExecuteFilterPredicate_ThenServiceOfferingIsSelected()
        {
            ServiceOffering argument = new ServiceOffering { ServiceType = new ServiceType { Name = "Apples" }, IsActive = true };
            MockRequest.Expect(m => m["ServiceTypeFilters"]).Return("Apples|Oranges|Grapes");
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(argument));
        }

        [TestMethod]
        public void GivenServiceTypeFiltersExistInRequest_AndServiceOfferingDoesNotMatchFilter_WhenIExecuteFilterPredicate_ThenServiceOfferingIsNotSelected()
        {
            ServiceOffering argument = new ServiceOffering { ServiceType = new ServiceType { Name = "Purple" } };
            MockRequest.Expect(m => m["ServiceTypeFilters"]).Return("Apples|Oranges|Grapes");
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(argument));
        }

        [TestMethod]
        public void GivenIsActiveIsFalse_WhenIExecuteFilterPredicate_ThenReturnFalse()
        {
            ServiceOffering argument = new ServiceOffering { IsActive = false };
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(argument));
        }

        [TestMethod]
        public void GivenServiceOffering_WhenInvokeDataSelector_ThenDataContainsExpectedProperties()
        {
            bool expectedIsFavorite = true;
            bool expectedIsProviate = true;
            string expectedServiceTypeName = "jw29fij2";
            string expectedProviderName = "slkdjfsdkljfs";
            string expectedProgramName = "wjovjwiojw";
            int expectedServiceOfferingId = 382;
            ServiceOffering offering = new ServiceOffering
            {
                UsersLinkingAsFavorite = new List<User> { CurrentUser.Identity.User },
                ServiceType = new ServiceType { Name = expectedServiceTypeName, IsPrivate = expectedIsProviate },
                Provider = new Provider { Name = expectedProviderName },
                Program = new Program { Name = expectedProgramName },
                Id = expectedServiceOfferingId
            };
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            dynamic actual = target.DataSelector.Compile().Invoke(offering);

            Assert.AreEqual(expectedIsFavorite, actual.IsFavorite);
            Assert.AreEqual(expectedIsProviate, actual.IsPrivate);
            Assert.AreEqual(expectedServiceTypeName, actual.ServiceType);
            Assert.AreEqual(expectedProviderName, actual.Provider);
            Assert.AreEqual(expectedProgramName, actual.Program);
            Assert.AreEqual(expectedServiceOfferingId, actual.Id);
        }

        [TestMethod]
        public void GivenServiceOffering_AndUserIsProvider_AndUserNotAssignedProvider_WhenInvokeDataSelector_ThenDataCanInteractFalse()
        {
            ServiceOffering offering = new ServiceOffering
            {
                Provider = new Provider(),
                ServiceType = new ServiceType(),
                Program = new Program()
            };
            CurrentUser.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.Provider } });
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            dynamic actual = target.DataSelector.Compile().Invoke(offering);

            Assert.AreEqual(false, actual.CanInteract);
        }

        [TestMethod]
        public void GivenServiceOffering_AndUserIsSiteCoordinator_WhenInvokeDataSelector_ThenDataCanInteractTrue()
        {
            ServiceOffering offering = new ServiceOffering
            {
                Provider = new Provider(),
                ServiceType = new ServiceType(),
                Program = new Program()
            };
            CurrentUser.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.SiteCoordinator } });
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            dynamic actual = target.DataSelector.Compile().Invoke(offering);

            Assert.AreEqual(true, actual.CanInteract);
        }

        [TestMethod]
        public void GivenServiceOffering_AndUserIsDataAdmin_WhenInvokeDataSelector_ThenDataCanInteract()
        {
            ServiceOffering offering = new ServiceOffering
            {
                Provider = new Provider(),
                ServiceType = new ServiceType(),
                Program = new Program()
            };
            CurrentUser.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            dynamic actual = target.DataSelector.Compile().Invoke(offering);

            Assert.AreEqual(true, actual.CanInteract);
        }

        [TestMethod]
        public void GivenServiceOffering_AndUserIsProvider_AndUserIsAssignedProvider_WhenInvokeDataSelector_ThenDataCanInteractTrue()
        {
            Provider matchingProvider = new Provider();
            ServiceOffering offering = new ServiceOffering
            {
                Provider = matchingProvider,
                ServiceType = new ServiceType(),
                Program = new Program()
            };
            CurrentUser.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.Provider }, Providers = new[] { matchingProvider } });
            ServiceOfferingClientDataTable target = new ServiceOfferingClientDataTable(MockRequest, CurrentUser);

            dynamic actual = target.DataSelector.Compile().Invoke(offering);

            Assert.AreEqual(true, actual.CanInteract);
        }
    }
}
