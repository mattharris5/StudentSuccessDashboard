using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;

namespace SSD.ViewModels
{
    [TestClass]
    public class PrivateHealthFieldModelTest
    {
        private PrivateHealthFieldModel Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new PrivateHealthFieldModel();
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyTo_ThenProviderIdSet()
        {
            PrivateHealthField model = new PrivateHealthField();
            Target.SelectedProviderId = 38293;

            Target.CopyTo(model);

            Assert.AreEqual(Target.SelectedProviderId, model.ProviderId);
        }

        [TestMethod]
        public void GivenInvalidModel_WhenCopyTo_ThenThrowException()
        {
            PublicField invalid = new PublicField();

            Target.ExpectException<ArgumentException>(() => Target.CopyTo(invalid));
        }

        [TestMethod]
        public void GivenValidModel_WhenCopyFrom_ThenSelectedProviderIdSet()
        {
            PrivateHealthField model = new PrivateHealthField { ProviderId = 23828, CreatingUser = new User() };

            Target.CopyFrom(model);

            Assert.AreEqual(model.ProviderId, Target.SelectedProviderId);
        }

        [TestMethod]
        public void GivenInvalidModel_WhenCopyFrom_ThenThrowException()
        {
            PublicField invalid = new PublicField { CreatingUser = new User() };

            Target.ExpectException<ArgumentException>(() => Target.CopyFrom(invalid));
        }
    }
}
