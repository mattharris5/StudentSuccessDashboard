using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.ViewModels
{
    [TestClass]
    public class UserModelTest
    {
        private UserModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new UserModel();
        }

        [TestMethod]
        public void GivenUserAlreadyHasEmailAddress_AndViewModelEmailAddressDifferent_WhenICopyToDomainEntity_ThenDomainEntityContainsState_AndEmailAddressUnchanged()
        {
            string expectedEmailAddress = "jim@jim.jim";
            User destination = new User { EmailAddress = expectedEmailAddress };
            InitializeTargetState("jim@fred.bob");

            Target.CopyTo(destination);

            Assert.AreEqual(Target.DisplayName, destination.DisplayName);
            Assert.AreEqual(expectedEmailAddress, destination.EmailAddress); // NOTE: Email address is read-only from view model
            Assert.AreEqual(Target.PendingEmail, destination.PendingEmail);
            Assert.AreEqual(Target.UserKey, destination.UserKey);
        }

        [TestMethod]
        public void GivenUserAlreadyHasPendingEmail_AndUserAlreadyHasConfirmationGuid_AndViewModelPendingEmailIsNull_WhenICopyToDomainEntity_ThenPendingEmailIsNull_AndConfirmationGuidIsEmpty()
        {
            User destination = new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = Guid.NewGuid() };
            InitializeTargetState(null);

            Target.CopyTo(destination);

            Assert.IsNull(destination.PendingEmail);
            Assert.AreEqual(Guid.Empty, destination.ConfirmationGuid);
        }

        [TestMethod]
        public void GivenUserAlreadyHasPendingEmail_AndUserAlreadyHasConfirmationGuid_AndViewModelPendingEmailIsEmpty_WhenICopyToDomainEntity_ThenPendingEmailIsNull_AndConfirmationGuidIsEmpty()
        {
            User destination = new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = Guid.NewGuid() };
            InitializeTargetState(string.Empty);

            Target.CopyTo(destination);

            Assert.IsNull(destination.PendingEmail);
            Assert.AreEqual(Guid.Empty, destination.ConfirmationGuid);
        }

        [TestMethod]
        public void GivenUserAlreadyHasPendingEmail_AndUserAlreadyHasConfirmationGuid_AndViewModelPendingEmailIsWhitespace_WhenICopyToDomainEntity_ThenPendingEmailIsNull_AndConfirmationGuidIsEmpty()
        {
            User destination = new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = Guid.NewGuid() };
            InitializeTargetState("     \r\n \t");

            Target.CopyTo(destination);

            Assert.IsNull(destination.PendingEmail);
            Assert.AreEqual(Guid.Empty, destination.ConfirmationGuid);
        }

        [TestMethod]
        public void GivenUserAlreadyHasPendingEmail_AndUserAlreadyHasConfirmationGuid_AndViewModelPendingEmailDiffers_WhenICopyToDomainEntity_ThenPendingEmailAndConfirmationGuidUpdate()
        {
            Guid notExpectedConfirmationGuid = Guid.NewGuid();
            User destination = new User { PendingEmail = "jim@jim.jim", ConfirmationGuid = notExpectedConfirmationGuid };
            InitializeTargetState("bob@bob.bob");

            Target.CopyTo(destination);

            Assert.AreEqual("bob@bob.bob", destination.PendingEmail);
            Assert.AreNotEqual(notExpectedConfirmationGuid, destination.ConfirmationGuid);
        }

        [TestMethod]
        public void GivenUserAlreadyHasPendingEmail_AndUserAlreadyHasConfirmationGuid_AndViewModelPendingEmailMatches_WhenICopyToDomainEntity_ThenPendingEmailAndConfirmationGuidRemainUnchanged()
        {
            Guid expectedConfirmationGuid = Guid.NewGuid();
            User destination = new User { PendingEmail = "bob@bob.bob", ConfirmationGuid = expectedConfirmationGuid };
            InitializeTargetState("bob@bob.bob");

            Target.CopyTo(destination);

            Assert.AreEqual("bob@bob.bob", destination.PendingEmail);
            Assert.AreEqual(expectedConfirmationGuid, destination.ConfirmationGuid);
        }

        [TestMethod]
        public void WhenICopyFromADomainEntity_ThenViewModelContainsState()
        {
            User source = new User
            {
                DisplayName = "oranges!",
                EmailAddress = "tacobell@fastfood.com",
                PendingEmail = "jelly@peanutbutter.mayo",
                Id = 2382,
                UserKey = "2h9821928j",
                EulaAcceptances =
                    new List<EulaAcceptance> { new EulaAcceptance { Id = 1, EulaAgreementId = 1, CreateTime = DateTime.Now } }
            };

            Target.CopyFrom(source);

            Assert.AreEqual(source.DisplayName, Target.DisplayName);
            Assert.AreEqual(source.EmailAddress, Target.EmailAddress);
            Assert.AreEqual(source.PendingEmail, Target.PendingEmail);
            Assert.AreEqual(source.Id, Target.Id);
            Assert.AreEqual(source.UserKey, Target.UserKey);
            Assert.AreEqual(source.EulaAcceptances.First().CreateTime, Target.EulaAcceptanceTime);
        }

        [TestMethod]
        public void GivenNullPendingEmail_WhenCopyFrom_ThenPendingEmailSetToCurrentEmail()
        {
            User source = new User
            {
                EmailAddress = "tacobell@fastfood.com"
            };

            Target.CopyFrom(source);

            Assert.AreEqual(source.EmailAddress, Target.PendingEmail);
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

        private void InitializeTargetState(string pendingEmail)
        {
            Target.DisplayName = "apples!";
            Target.EmailAddress = "jellybeans@candy.com";
            Target.PendingEmail = pendingEmail;
            Target.Id = 338;
            Target.UserKey = "34u8fj28ew";
        }
    }
}
