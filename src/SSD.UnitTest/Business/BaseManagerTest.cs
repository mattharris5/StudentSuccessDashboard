using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels.DataTables;
using System.Web;

namespace SSD.Business
{
    [TestClass]
    public abstract class BaseManagerTest
    {
        protected EducationSecurityPrincipal User { get; private set; }
        protected TestData Data { get; private set; }
        protected TestRepositories Repositories { get; private set; }
        protected HttpContextBase MockHttpContext { get; private set; }
        protected IDataTableBinder MockDataTableBinder { get; private set; }

        [TestInitialize]
        public void BaseInitializeTest()
        {
            User = new EducationSecurityPrincipal(new User { UserKey = "whatever" });
            Data = new TestData();
            Repositories = new TestRepositories(Data);
            MockHttpContext = MockHttpContextFactory.Create();
            MockDataTableBinder = MockRepository.GenerateMock<IDataTableBinder>();
            PermissionFactory.SetCurrent(MockRepository.GenerateMock<IPermissionFactory>());
        }
    }
}
