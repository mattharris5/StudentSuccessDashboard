using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Repository
{
    [TestClass]
    public class ProviderRepositoryTest
    {
        private EducationDataContext Context { get; set; }
        private ProviderRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Context = new EducationDataContext();
            Target = new ProviderRepository(Context);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}
