using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SSD
{
    [TestClass]
    public class FilterConfigTest
    {
        private GlobalFilterCollection Filters { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Filters = new GlobalFilterCollection();
            FilterConfig.RegisterGlobalFilters(Filters);
        }

        [TestMethod]
        public void GivenNullFilterCollection_WhenRegisterGlobalFilters_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => FilterConfig.RegisterGlobalFilters(null));
        }

        [TestMethod]
        public void WhenRegisterGlobalFilters_ThenRegisterErrorHandlingAttribute()
        {
            IEnumerable<HandleEntityErrorAttribute> actual = Filters.Select(f => f.Instance).OfType<HandleEntityErrorAttribute>();
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void WhenRegisterGlobalFilters_ThenRegisterTraceActionAttribute()
        {
            IEnumerable<TraceActionAttribute> actual = Filters.Select(f => f.Instance).OfType<TraceActionAttribute>();
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void WhenRegisterGlobalFilters_ThenRegisterUserIdentityMapAttribute()
        {
            IEnumerable<UserIdentityMapAttribute> actual = Filters.Select(f => f.Instance).OfType<UserIdentityMapAttribute>();
            Assert.AreEqual(1, actual.Count());
        }

        [TestMethod]
        public void WhenRegisterGlobalFilters_ThenUserIdentityMapAttributeOrderLessThanDefaultAuthenticateAndAuthorizeAttributeOrder()
        {
            UserIdentityMapAttribute userIdentityMapAttribute = Filters.Select(f => f.Instance).OfType<UserIdentityMapAttribute>().Single();
            AuthenticateAndAuthorizeAttribute otherAttribute = new AuthenticateAndAuthorizeAttribute();
            Assert.IsTrue(userIdentityMapAttribute.Order < otherAttribute.Order);
        }
    }
}
