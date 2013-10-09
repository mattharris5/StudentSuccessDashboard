using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using System;
using System.Collections;
using System.Linq;

namespace SSD.Repository
{
    [TestClass]
    public class RepositoryContainerTest
    {
        private IWindsorContainer WindsorContainer { get; set; }
        private IEducationContext MockContext { get; set; }
        private RepositoryContainer Target { get; set; }

        [TestInitialize]
        public void InitailizeTest()
        {
            WindsorContainer = MockRepository.GenerateMock<IWindsorContainer>();
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            Target = new RepositoryContainer(WindsorContainer, MockContext);
        }

        [TestMethod]
        public void GivenNullWindsorContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new RepositoryContainer(null, MockRepository.GenerateMock<IEducationContext>()));
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new RepositoryContainer(new WindsorContainer(), null));
        }

        [TestMethod]
        public void GivenRepositoryWasNotRegisteredWithWindsorContainer_WhenObtain_ThenThrowException()
        {
            Target.ExpectException<InvalidOperationException>(() => Target.Obtain<StudentRepository>());
        }

        [TestMethod]
        public void GivenWindsorContainerResolvesUsingContext_WhenObtain_ThenReturnResolvedRepository()
        {
            StudentRepository expected = new StudentRepository(MockContext);
            WindsorContainer.Expect(m => m.Resolve<StudentRepository>(Arg<IDictionary>.Matches(a => MatchDictionary(a)))).Return(expected);

            StudentRepository actual = Target.Obtain<StudentRepository>();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhenSave_ThenContextReflectsChanges()
        {
            Target.Save();

            MockContext.AssertWasCalled(m => m.SaveChanges());
        }

        private bool MatchDictionary(IDictionary a)
        {
            return a.Keys.Cast<string>().Single() == "context" && a.Values.Cast<object>().Single() == MockContext;
        }
    }
}
