using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using SSD.ViewModels;
using System;
using System.Linq;
using Rhino.Mocks;
using System.Collections.Generic;

namespace SSD.Business
{
    [TestClass]
    public class AgreementManagerTest : BaseManagerTest
    {
        private AgreementManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new AgreementManager(Repositories.MockRepositoryContainer);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new AgreementManager(null));
        }

        [TestMethod]
        public void GivenEulas_WhenGenerateEulaAdminModel_ThenExpectedModelReturned()
        {
            EulaModel expected = new EulaModel();
            expected.CopyFrom(Data.Eulas.OrderByDescending(e => e.CreateTime).First());

            var actual = Target.GenerateEulaAdminModel();

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.EulaText, actual.EulaText);
        }

        [TestMethod]
        public void WhenGeneratePromptViewModel_ThenReturnInstance()
        {
            EulaModel actual = Target.GeneratePromptViewModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenMultipleEulaAgreements_WhenGeneratePromptViewModel_ThenReturnLatestAgreement()
        {
            EulaAgreement expected = Data.Eulas.OrderByDescending(e => e.CreateTime).First();

            EulaModel actual = Target.GeneratePromptViewModel();

            Assert.AreEqual(expected.CreateTime, actual.Audit.CreateTime);
            Assert.AreEqual(expected.CreatingUser.DisplayName, actual.Audit.CreatedBy);
            Assert.AreEqual(expected.EulaText, actual.EulaText);
            Assert.AreEqual(expected.Id, actual.Id);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(null, User));
        }

        [TestMethod]
        public void GivenNullUser_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(new EulaModel(), null));
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<EntityAccessUnauthorizedException>(() => Target.Create(new EulaModel(), User));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenChangesSaved()
        {
            EulaModel viewModel = new EulaModel { Id = 1, EulaText = "blah blah blah", Audit = new AuditModel { CreatedBy = User.Identity.User.DisplayName, CreateTime = DateTime.Now } };
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            Target.Create(viewModel, User);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenAddedEulaAgreementHasStateFromViewModel()
        {
            EulaModel expectedState = new EulaModel { Id = 1, EulaText = "blah blah" };
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            Target.Create(expectedState, User);

            Repositories.MockEulaAgreementRepository.AssertWasCalled(m => m.Add(Arg<EulaAgreement>.Matches(a => AssertPropertiesMatch(expectedState, a))));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenAddedEulaAgreementHasCreationAuditData()
        {
            EulaModel viewModel = new EulaModel { Id = 1, EulaText = "blah blah" };
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            Target.Create(viewModel, User);

            Repositories.MockEulaAgreementRepository.AssertWasCalled(m => m.Add(Arg<EulaAgreement>.Matches(a => a.CreateTime.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now) && a.CreatingUser == User.Identity.User)));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenUserRepositoryUpdates()
        {
            EulaModel viewModel = new EulaModel { Id = 1, EulaText = "blah blerg bleh", Audit = new AuditModel { CreatedBy = User.Identity.User.DisplayName, CreateTime = DateTime.Now } };
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            Target.Create(viewModel, User);

            Repositories.MockUserRepository.AssertWasCalled(m => m.Update(User.Identity.User));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenLog_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Log(null, User));
        }

        [TestMethod]
        public void GivenNullUser_WhenLog_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Log(new EulaModel(), null));
        }

        [TestMethod]
        public void GivenViewModel_WhenLog_ThenRepositoryUpdates()
        {
            EulaModel viewModel = new EulaModel { Id = 1, EulaText = "blah blerg bleh", Audit = new AuditModel { CreatedBy = User.Identity.User.DisplayName, CreateTime = DateTime.Now } };

            Target.Log(viewModel, User);

            Repositories.MockUserRepository.AssertWasCalled(m => m.Update(User.Identity.User));
        }

        [TestMethod]
        public void GivenViewModel_WhenLog_ThenContextSaves()
        {
            EulaModel viewModel = new EulaModel { Id = 1, EulaText = "blah blerg bleh", Audit = new AuditModel { CreatedBy = User.Identity.User.DisplayName, CreateTime = DateTime.Now } };

            Target.Log(viewModel, User);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenNoUser_WhenGenerateEulaModelByUser_ThenException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEulaModelByUser(35000));
        }

        [TestMethod]
        public void GivenNoAcceptanceWhenGenerateEulaModelByUserThenException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEulaModelByUser(2));
        }

        [TestMethod]
        public void GivenUser_WhenGenerateEulaModelByUser_ThenEulaModelReturned()
        {
            TestData data = new TestData();
            EulaModel expected = new EulaModel();
            expected.CopyFrom(data.Users.First().EulaAcceptances.First().EulaAgreement);

            var actual = Target.GenerateEulaModelByUser(1);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.EulaText, actual.EulaText);
        }

        private static bool AssertPropertiesMatch(EulaModel expectedState, EulaAgreement actualState)
        {
            Assert.IsNotNull(actualState);
            Assert.AreEqual(expectedState.EulaText, actualState.EulaText);
            return true;
        }
    }
}
