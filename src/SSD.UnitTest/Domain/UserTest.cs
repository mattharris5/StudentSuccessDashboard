using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SSD.Domain
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void WhenConstruct_ThenCreateTimeSet()
        {
            User actual = new User();

            Assert.IsTrue(actual.CreateTime.WithinTimeSpanOf(TimeSpan.FromMilliseconds(20), DateTime.Now));
        }

        [TestMethod]
        public void WhenIConstruct_ThenFavoriteServiceOfferingsNotNull()
        {
            User target = new User();

            Assert.IsNotNull(target.FavoriteServiceOfferings);
        }

        [TestMethod]
        public void WhenIConstruct_ThenUserRolesNotNull()
        {
            User target = new User();

            Assert.IsNotNull(target.UserRoles);
        }

        [TestMethod]
        public void WhenIConstruct_ThenLoginEventsNotNull()
        {
            User target = new User();

            Assert.IsNotNull(target.LoginEvents);
        }

        [TestMethod]
        public void WhenIConstruct_ThenEulaAcceptancesNotNull()
        {
            User target = new User();

            Assert.IsNotNull(target.EulaAcceptances);
        }

        [TestMethod]
        public void WhenIConstruct_ThenPrivateHealthDataViewEventsNotNull()
        {
            User target = new User();

            Assert.IsNotNull(target.PrivateHealthDataViewEvents);
        }

        [TestMethod]
        public void GivenDisplayNameSetToEmpty_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenEmailAddressSetToEmpty_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "dfd", EmailAddress = "" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenDisplayNameSetToAnonymous_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "Anonymous", EmailAddress = "blah" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenEmailSetToAnonymous_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "dfd", EmailAddress = "Anonymous@sample.com" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenFirstNameSetToAnonymous_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "blah", EmailAddress = "blah", FirstName = "Anonymous", }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenLastNameSetToAnonymous_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "blah", EmailAddress = "blah", LastName = "Anonymous" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenFirstNameSetToEmpty_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "blah", EmailAddress = "blah", FirstName = "" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenLastNameSetToEmpty_WhenConstruct_ThenIsValidUserInformationFalse()
        {
            Assert.IsFalse(new User { DisplayName = "blah", EmailAddress = "blah", FirstName = "bleh", LastName = "" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenAllFieldsSet_WhenConstruct_ThenIsValidUserInformationTrue()
        {
            Assert.IsTrue(new User { DisplayName = "display", EmailAddress = "email@email.com", FirstName = "first", LastName = "last" }.IsValidUserInformation);
        }

        [TestMethod]
        public void GivenUserInactive_WhenGetStatus_ThenReturnInactive()
        {
            User target = new User { Active = false };

            Assert.AreEqual(User.InactiveStatus, target.Status);
        }

        [TestMethod]
        public void GivenUserActive_AndUserHasRole_WhenGetStatus_ThenReturnActive()
        {
            User target = new User { Active = true, UserRoles = new List<UserRole> { new UserRole() } };

            Assert.AreEqual(User.ActiveStatus, target.Status);
        }

        [TestMethod]
        public void GivenUserActive_AndUserHasNoRole_WhenGetStatus_ThenReturnAwaitingAssignment()
        {
            User target = new User { Active = true };

            Assert.AreEqual(User.AwaitingAssignmentStatus, target.Status);
        }

        [TestMethod]
        public void GivenNoLoginEvents_WhenGetLastLoginTime_ThenReturnNull()
        {
            User target = new User();

            Assert.IsNull(target.LastLoginTime);
        }

        [TestMethod]
        public void GivenLoginEvents_WhenGetLastLoginTime_ThenLatestTime()
        {
            DateTime? expected = DateTime.Now;
            User target = new User
            {
                LoginEvents = new List<LoginEvent>
                {
                    new LoginEvent { CreateTime = DateTime.Now.AddDays(-1) },
                    new LoginEvent { CreateTime = DateTime.Now.AddDays(-3) },
                    new LoginEvent { CreateTime = expected.Value },
                    new LoginEvent { CreateTime = DateTime.Now.AddDays(-2) }
                }
            };

            Assert.AreEqual(expected, target.LastLoginTime);
        }
    }
}
