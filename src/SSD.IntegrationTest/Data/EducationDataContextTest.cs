using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;

namespace SSD.Data
{
    [TestClass]
    public class EducationDataContextTest
    {
        private TransactionScope _TestTransaction;
        private EducationDataContext Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            _TestTransaction = new TransactionScope();
            Target = new EducationDataContext();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (_TestTransaction != null)
            {
                _TestTransaction.Dispose();
            }
            if (Target != null)
            {
                Target.Dispose();
            }
        }

        [TestMethod]
        public void GivenDuplicateProviderAlreadyExists_WhenSaveChanges_ThenExceptionIsThrown()
        {
            Target.Providers.Add(new Provider { Name = "Bob", Address = new Address() });
            Target.SaveChanges();
            Target.Providers.Add(new Provider { Name = "Bob" });
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenDatabaseIsDeleted_WhenInitialize_ThenServiceTypeCategoryIsPopulated()
        {
            ResetDatabase();
            var expected = new List<string> 
            { 
                "Basic Needs" , "Consumer Services", "Criminal Justice and Legal Services" ,"Education", "Environmental Quality",
                "Health Care" ,"Income Support and Employment","Individual and Family Life" , "Mental Health Care and Counseling" ,"Organizational/Community Services",
                "Support Groups" ,"Target Populations" , "Test Category,"
            };

            List<string> categories = Target.Categories.Select(c => c.Name).ToList();

            CollectionAssert.AreEquivalent(expected, categories);
        }

        [TestMethod]
        public void GivenANewUserIsAdded_WhenSaveChanges_ThenDatabaseContainsUser()
        {
            string userKey = Guid.NewGuid().ToString();
            Target.Users.Add(new User { UserKey = userKey, FirstName = "Mr.", LastName = "Blah", DisplayName = "blah", EmailAddress = "blah@blah.com" });

            Target.SaveChanges();

            using (EducationDataContext verifyContext = new EducationDataContext())
            {
                Assert.IsNotNull(verifyContext.Users.SingleOrDefault(u => u.UserKey == userKey));
            }
        }

        [TestMethod]
        public void GivenANewServiceType_WhenSaveChanges_ThenDatabaseContainsServiceType()
        {
            Target.ServiceTypes.Add(new ServiceType
            {
                Name = "Tester Service",
                Description = "Tester Service Description"
            });

            Target.SaveChanges();

            using (EducationDataContext verifyContext = new EducationDataContext())
            {
                Assert.IsNotNull(verifyContext.ServiceTypes.SingleOrDefault(s => s.Name.Equals("Tester Service")));
            }
        }

        [TestMethod]
        public void GivenANewServiceOffering_WhenSaveChanges_ThenDatabaseContainsServiceOfferings()
        {
            Target.ServiceOfferings.Add(new ServiceOffering
            {
                Provider = new Provider
                {
                    Name = "999999"
                },
                ServiceType = new ServiceType
                {
                    Name = "999999"
                },
                Program = new Program 
                {
                    Name = "99999"
                }
            });

            Target.SaveChanges();

            using (EducationDataContext verifyContext = new EducationDataContext())
            {
                Assert.IsNotNull(verifyContext.ServiceOfferings.SingleOrDefault(s => s.Provider.Name.Equals("999999") && s.ServiceType.Name.Equals("999999")));
            }
        }

        [TestMethod]
        public void GivenEmailAddressAlreadyExistsForAUser_WhenSaveChanges_ThenFail()
        {
            Target.Users.Add(new User { FirstName = "sldkfjs", LastName = "skljfslkjw", DisplayName = "298ejf298", UserKey = "vwoiuwh", EmailAddress = "bob@bob.bob" });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenUserKeyAlreadyExistsForAUser_WhenSaveChanges_ThenFail()
        {
            Target.Users.Add(new User { FirstName = "sldkfjs", LastName = "skljfslkjw", DisplayName = "298ejf298", UserKey = "bob", EmailAddress = "vwoiuwh@owm.com" });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenCategoryNameAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.Categories.Add(new Category { Name = "Education" });

            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenPriorityNameAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.Priorities.Add(new Priority { Name = "Low" });

            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenRoleNameAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.Roles.Add(new Role { Name = SecurityRoles.DataAdmin });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenServiceTypeNameAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.ServiceTypes.Add(new ServiceType { Name = "Mentoring" });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenSubjectNameAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.Subjects.Add(new Subject { Name = "Science" });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenPropertyEntityAndNameAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Property duplicate = new Property { EntityName = "SSD.Domain.Category", Name = "Name" };
            Target.Properties.Add(duplicate);
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void WhenInitialize_ThenExpectedPropertiesAreProtected()
        {
            var expected = new List<Property>
            {
                new Property { EntityName = "SSD.Domain.Student", Name = "StudentKey" },
                new Property { EntityName = "SSD.Domain.Student", Name = "ServiceRequests" },
                new Property { EntityName = "SSD.Domain.Student", Name = "StudentAssignedOfferings" },
                new Property { EntityName = "SSD.Domain.ServiceOffering", Name = "StudentAssignedOfferings" }
            };
            ResetDatabase();
            Target.Dispose();
            Target = new EducationDataContext();

            var actual = Target.Properties.Where(p => p.IsProtected).ToList();

            Assert.IsTrue(expected.All(p => actual.Exists(a => a.Name == p.Name && a.EntityName == p.EntityName)));
            Assert.IsFalse(actual.Any(a => !expected.Exists(p => p.Name == a.Name && p.EntityName == a.EntityName)));
        }

        [TestMethod]
        public void GivenUserRoleAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.UserRoles.Add(new UserRole { UserId = 1, RoleId = 1 });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenCustomFieldAlreadyExists_WhenSaveChanges_ThenFail()
        {
            Target.CustomFields.Add(new PublicField { Name = "ACT", CreatingUserId = Target.Users.First().Id, CustomFieldTypeId = Target.CustomFieldTypes.First().Id, Categories = Target.CustomFieldCategories.ToList() });
            
            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenCustomFieldHasNoCategories_WhenSaveChanges_ThenFail()
        {
            Target.CustomFields.Add(new PublicField { Name = "New Field Name", CreatingUserId = Target.Users.First().Id, CustomFieldTypeId = Target.CustomFieldTypes.First().Id });

            Target.ExpectException<DbEntityValidationException>(() => Target.SaveChanges());
        }

        [TestMethod]
        public void GivenANewServiceType_WhenSaveChanges_ThenNewServiceTypeIsSaved()
        {
            ServiceType serviceType = new ServiceType
            {
                Name = "Bob",
                Description = "Bob's Description",
            };
            Target.ServiceTypes.Add(serviceType);

            Target.SaveChanges();

            var secondaryClient = new EducationDataContext();
            var actual = secondaryClient.ServiceTypes.Where(s => s.Name == "Bob").SingleOrDefault();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenServiceOfferingIsUpdated_WhenSaveChanges_ThenOperationSucceeds()
        {
            ServiceOffering toUpdate = Target.ServiceOfferings.First();
            Target.SetModified(toUpdate);

            Target.SaveChanges();
        }

        [TestMethod]
        public void WhenInitializeDatabase_ThenFulfillmentStatusesPopulated()
        {
            List<string> expected = new List<string> { Statuses.Open, Statuses.Fulfilled, Statuses.Rejected };
            ResetDatabase();
            Target.Dispose();
            Target = new EducationDataContext();
            
            var actual = Target.FulfillmentStatuses.Select(f => f.Name).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void GivenDuplicateFulfillmentStatusAdded_WhenSaveChanges_ThenThrowException()
        {
            Target.FulfillmentStatuses.Add(new FulfillmentStatus { Name = Statuses.Fulfilled });

            Target.ExpectException<DbUpdateException>(() => Target.SaveChanges());
        }

        private void ResetDatabase()
        {
            _TestTransaction.Dispose();
            _TestTransaction = null;
            if (Target.Database.Exists())
            {
                Target.Database.ExecuteSqlCommand("USE [master]; ALTER DATABASE [SSD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                Target.Database.Delete();
                Target.Database.Initialize(true);
            }
        }
    }
}
