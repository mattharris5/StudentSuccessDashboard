using Castle.MicroKernel;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strive.DependencyInjection
{
    [TestClass]
    public class WindsorDependencyResolverTest
    {
        private WindsorDependencyResolver Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            IKernel mockKernel = MockRepository.GenerateMock<IKernel>();
            IWindsorContainer mockWindsorContainer = MockRepository.GenerateMock<IWindsorContainer>();
            mockKernel.Expect(m => m.HasComponent(typeof(int))).Return(true);
            mockKernel.Expect(m => m.HasComponent(typeof(string))).Return(true);
            mockWindsorContainer.Expect(m => m.Resolve(typeof(int))).Return(1);
            mockWindsorContainer.Expect(m => m.ResolveAll(typeof(int))).Return(new[] { 1, 2, 3 });
            mockWindsorContainer.Expect(m => m.Resolve(typeof(string))).Return("1");
            mockWindsorContainer.Expect(m => m.ResolveAll(typeof(string))).Return(new[] { "1", "2", "3" });
            mockWindsorContainer.Expect(m => m.Kernel).Return(mockKernel);
            Target = new WindsorDependencyResolver(mockWindsorContainer);
        }

        [TestMethod]
        public void GivenNullContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new WindsorDependencyResolver(null));
        }

        [TestMethod]
        public void GivenKnownType_WhenGetService_ThenReturnInstance()
        {
            int expected = 1;

            object actual = Target.GetService(typeof(int));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenUnknownType_WhenGetService_ThenReturnNull()
        {
            object actual = Target.GetService(typeof(decimal));

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GivenKnownType_WhenGetServices_ThenReturnInstances()
        {
            string[] expected = new[] { "1", "2", "3" };

            IEnumerable<object> actual = Target.GetServices(typeof(string));

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenUnknownType_WhenGetServices_ThenReturnInstances()
        {
            IEnumerable<object> actual = Target.GetServices(typeof(double));

            Assert.IsFalse(actual.Any());
        }
    }
}
