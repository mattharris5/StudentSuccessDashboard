using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class ProgramClientDataTableTest : BaseClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockHttpContextFactory.CreateRequest();
        }

        [TestMethod]
        public void GivenNoCriteria_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Program program = new Program { IsActive = true };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNoCriteria_AndIsActiveIsFalse_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Program program = new Program { IsActive = false };

            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramHasDifferentName_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Program program = new Program { Name = "different", IsActive = true };
            MockRequest.Expect(m => m["PartnerName"]).Return("name");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNoCriteria_AndProgramHasName_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Program program = new Program { Name = "something", IsActive = true };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramMatchesName_AndIsActiveIsFalse_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Program program = new Program { Name = "name", IsActive = false };
            MockRequest.Expect(m => m["PartnerName"]).Return("name");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteriaIsEmptyString_AndProgramHasName_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Program program = new Program { Name = "something", IsActive = true };
            MockRequest.Expect(m => m["PartnerName"]).Return(string.Empty);
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramNameContainsCriteria_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Program program = new Program { Name = "different", IsActive = true };
            MockRequest.Expect(m => m["PartnerName"]).Return("fere");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramNameContainsCriteria_AndCaseDoesNotMatch_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Program program = new Program { Name = "diffErent", IsActive = true };
            MockRequest.Expect(m => m["PartnerName"]).Return("feRe");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramAssociatedWithProviderWithMatchingName_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Program program = new Program
            {
                Name = "not the search criteria",
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering 
                    {
                        Provider = new Provider { Name = "contains bLah criteria", IsActive = true },
                        ServiceType = new ServiceType(),
                        IsActive = true
                    }
                },
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramAssociatedWithProviderWithMatchingName_AndProviderIsNotActive_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Program program = new Program
            {
                Name = "not the search criteria",
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering 
                    {
                        Provider = new Provider { Name = "contains bLah criteria", IsActive = false },
                        ServiceType = new ServiceType()
                    }
                },
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramAssociatedWithServiceTypeWithMatchingName_AndServiceTypeIsActive_WhenExecuteFilterPredicate_ThenReturnTrue() 
        {
            Program program = new Program
            {
                Name = "not the search criteria",
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering 
                    {
                        Provider = new Provider(),
                        ServiceType = new ServiceType { Name = "contains bLah criteria", IsActive = true },
                        IsActive = true
                    }
                },
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramAssociatedWithServiceTypeWithMatchingName_AndServiceTypeIsNotActive_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Program program = new Program
            {
                Name = "not the search criteria",
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering 
                    {
                        Provider = new Provider(),
                        ServiceType = new ServiceType { Name = "contains bLah criteria" }
                    }
                },
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenNameCriteria_AndProgramServiceOfferingIsInactive_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Program program = new Program
            {
                Name = "not the search criteria",
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering 
                    {
                        Provider = new Provider(),
                        ServiceType = new ServiceType { Name = "contains bLah criteria", IsActive = true },
                        IsActive = false
                    }
                },
                IsActive = true
            };
            MockRequest.Expect(m => m["PartnerName"]).Return("blah");
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenProgram_WhenExecuteSortSelector_ThenReturnProgramName()
        {
            string expected = "something";
            Program program = new Program { Name = expected };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            Assert.AreEqual(expected, target.SortSelector.Compile().Invoke(program));
        }

        [TestMethod]
        public void GivenProgram_WhenExecuteDataSelector_ThenNamePropertyMatches()
        {
            string expected = "something";
            Program program = new Program { Name = expected };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            Assert.AreEqual(expected, actual.Name);
        }

        [TestMethod]
        public void GivenProgram_WhenExecuteDataSelector_ThenContactDataPropertyMatches()
        {
            Contact expectedState = new Contact { Name = "Bob", Phone = "123-456-7890", Email = "bob@bob.bob" };
            Program program = new Program { ContactInfo = expectedState };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            Assert.AreEqual(expectedState.Name, actual.Contact.Name);
            Assert.AreEqual(expectedState.Phone, actual.Contact.Phone);
            Assert.AreEqual(expectedState.Email, actual.Contact.Email);
        }

        [TestMethod]
        public void GivenProgramWithSchools_WhenExecuteDataSelector_ThenSchoolListMatches()
        {
            string[] expected = new[] { "School1", "School2", "School3" };
            Program program = new Program
            {
                Schools = expected.Select(name => new School { Name = name }).ToList()
            };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Schools).ToList());
        }

        [TestMethod]
        public void GivenProgramWithProviders_WhenExecuteDataSelector_ThenProviderListMatches()
        {
            string[] expected = new[] { "Apple", "Orange", "Banana" };
            Program program = new Program
            {
                ServiceOfferings = expected.Select(name => new ServiceOffering { IsActive = true, Provider = new Provider { Name = name } }).ToList()
            };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Providers).ToList());
        }

        [TestMethod]
        public void GivenProgramWithProviders_AndProvidersCanBeDuplicated_WhenExecuteDataSelector_ThenProviderListMatches()
        {
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);
            string[] expected = new[] { "Apple", "Orange", "Banana" };
            Program program = new Program
            {
                ServiceOfferings = expected.Select(name => new ServiceOffering { IsActive = true, Provider = new Provider { Name = name } }).ToList()
            };
            program.ServiceOfferings.Add(new ServiceOffering { IsActive = true, Provider = new Provider { Name = expected.First() } });

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Providers).ToList());
        }

        [TestMethod]
        public void GivenProgramWithProviders_WhenExecuteDataSelector_ThenServiceTypeListMatches()
        {
            string[] expected = new[] { "Apple", "Orange", "Banana" };
            Program program = new Program
            {
                ServiceOfferings = expected.Select(name => new ServiceOffering { IsActive = true, ServiceType = new ServiceType { Name = name } }).ToList()
            };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.ServiceTypes).ToList());
        }

        [TestMethod]
        public void GivenProgramWithServiceTypes_AndServiceTypesCanBeDuplicated_WhenExecuteDataSelector_ThenServiceTypeListMatches()
        {
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);
            string[] expected = new[] { "Apple", "Orange", "Banana" };
            Program program = new Program
            {
                ServiceOfferings = expected.Select(name => new ServiceOffering { IsActive = true, ServiceType = new ServiceType { Name = name } }).ToList()
            };
            program.ServiceOfferings.Add(new ServiceOffering { IsActive = true, ServiceType = new ServiceType { Name = expected.First() } });

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.ServiceTypes).ToList());
        }

        [TestMethod]
        public void GivenProgram_WhenExecuteDataSelector_ThenIdMatches()
        {
            int expected = 2842;
            Program program = new Program { Id = expected };
            ProgramClientDataTable target = new ProgramClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(program);

            Assert.AreEqual(expected, actual.Id);
        }
    }
}
