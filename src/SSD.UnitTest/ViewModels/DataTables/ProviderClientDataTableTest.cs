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
    public class ProviderClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }
        private TestData TestData { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockHttpContextFactory.CreateRequest();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenNameCriteria_AndProviderNameMatches_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider provider = new Provider
            {
                Name = "provider",
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("pro");
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(provider));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProviderNameDoesntMatch_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider provider = new Provider
            {
                Name = "provider",
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("xyz");
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(provider));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProviderAssociatedWithProgramWithMatchingName_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider provider = new Provider
            {
                Name = "not the search criteria",
                IsActive = true,
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering
                    {
                        Program = new Program { Name = "contains bLah criteria" }
                    }
                }
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(provider));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProviderNotAssociatedWithProgramWithMatchingName_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider provider = new Provider
            {
                Name = "not the search criteria",
                IsActive = true,
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering
                    {
                        Program = new Program { Name = "Nope" }
                    }
                }
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(provider));
        }

        [TestMethod]
        public void GivenProviderIsInactive_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider provider = new Provider
            {
                IsActive = false
            };
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(provider));

        }

        [TestMethod]
        public void GivenProvider_WhenInvokeDataSelector_ThenDataHasNameAndIdAndWebsite()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider expectedState = new Provider
            {
                Name = "provider's name",
                Website = "www.this-is-made-up.com",
                Id = 482
            };
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.Name, actual.Name);
            Assert.AreEqual(expectedState.Website, actual.Website);
            Assert.AreEqual(expectedState.Id, actual.Id);
        }

        [TestMethod]
        public void GivenProvider_WhenInvokeDataSelector_ThenDataHasAddress()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider expectedState = new Provider
            {
                Address = new Address { City = "Cbus", State = "OH", Street = "123 Main St.", Zip = "44444" }
            };
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.Address.City, actual.Address.City);
            Assert.AreEqual(expectedState.Address.State, actual.Address.State);
            Assert.AreEqual(expectedState.Address.Street, actual.Address.Street);
            Assert.AreEqual(expectedState.Address.Zip, actual.Address.Zip);
        }

        [TestMethod]
        public void GivenProvider_WhenInvokeDataSelector_ThenDataHasContact()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider expectedState = new Provider
            {
                Contact = new Contact { Email = "bob@bob.bob", Name = "Bob", Phone = "555-444-1234" }
            };
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.Contact.Email, actual.Contact.Email);
            Assert.AreEqual(expectedState.Contact.Name, actual.Contact.Name);
            Assert.AreEqual(expectedState.Contact.Phone, actual.Contact.Phone);
        }

        [TestMethod]
        public void GivenProvider_AndProviderHasAssociatedServiceOfferingsWithAProgramThatHasSchools_WhenInvokeDataSelector_ThenDataHasAssociatedSchoolNames()
        {
            string[] expected = new[] { "School1", "School2", "School3" };
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider expectedState = new Provider
            {
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering 
                    { 
                        IsActive = true, 
                        Program = new Program 
                        { 
                            Schools = expected.Select(e => new School { Name = e } ).ToList(),
                            IsActive = true 
                        } 
                    }
                }
            };
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(expectedState);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Schools).ToList());
        }

        [TestMethod]
        public void GivenProvider_AndProviderHasAssociatedServiceOfferingsWithAProgram_WhenInvokeDataSelector_ThenDataHasAssociatedProgramNames()
        {
            string[] expected = new[] { "program 1", "blah", "apples!" };
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider expectedState = new Provider
            {
                ServiceOfferings = expected.Select(e => new ServiceOffering { IsActive = true, Program = new Program { Name = e, IsActive = true } }).ToList()
            };
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(expectedState);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Programs).ToList());
        }

        [TestMethod]
        public void GivenProvider_AndUserIsNotDataAdmin_AndUserNotAssociatedToProvider_WhenInvokeDataSelector_ThenDataAccessModeIsView()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Provider provider = new Provider();
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(provider);

            Assert.AreEqual("View", actual.AccessMode);
        }

        [TestMethod]
        public void GivenProvider_AndUserIsDataAdmin_WhenInvokeDataSelector_ThenDataAccessModeIsAll()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever", UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } } } });
            Provider provider = new Provider();
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(provider);

            Assert.AreEqual("All", actual.AccessMode);
        }

        [TestMethod]
        public void GivenProvider_AndUserIsNotDataAdmin_AndUserIsAssociatedToProvider_WhenInvokeDataSelector_ThenDataAccessModeIsEdit()
        {
            Provider provider = new Provider { Id = 3548 };
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever", UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = SecurityRoles.Provider }, Providers = new List<Provider> { provider } } } });
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(provider);

            Assert.AreEqual("Edit", actual.AccessMode);
        }

        [TestMethod]
        public void GivenProviderHasInactiveProgram_AndProviderHasActiveProgram_AndUserIsDataAdmin_WhenInvokeDataSelector_ThenInactiveProgramsDontShow()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User { UserKey = "whatever", UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } } } });
            Provider provider = TestData.Providers[0];
            var expectedPrograms = TestData.Programs.Where(p => p.ServiceOfferings.Where(s => s.IsActive).Select(s => s.Provider).Contains(provider) && p.IsActive);
            ProviderClientDataTable target = new ProviderClientDataTable(MockRequest, user);

            dynamic actual = target.DataSelector.Compile().Invoke(provider);

            CollectionAssert.AreEqual(expectedPrograms.Select(p => p.Name).ToList(), ((IEnumerable<string>)actual.Programs).ToList());
        }
    }
}
