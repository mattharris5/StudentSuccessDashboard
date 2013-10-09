using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using System;
using System.Linq;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class DataContextInstallerTest
    {
        private DataContextInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new DataContextInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNoContainer_WhenIInstall_ThenThrowArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }
    }
}
