using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD.Repository
{
    [TestClass]
    public class CustomFieldRepositoryTest
    {
        private EducationDataContext Context { get; set; }
        private CustomFieldRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Context = new EducationDataContext();
            Target = new CustomFieldRepository(Context);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }

        [TestMethod]
        public void GivenPrivateHealthFields_WhenGetItemsOfPrivateHealthFieldType_ThenPrivateFieldsReturned()
        {
            var expected = Context.CustomFields.OfType<PrivateHealthField>();

            var actual = Target.Items.OfType<PrivateHealthField>();

            Assert.AreEqual(2, actual.Count());
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        [TestMethod]
        public void GivenPrivateHealthFields_WhenGetItemsOfCustomFieldType_ThenNoPrivateFieldReturned()
        {
            var actual = Target.Items.OfType<PublicField>();

            Assert.AreEqual(4, actual.Count());
        }
    }
}
