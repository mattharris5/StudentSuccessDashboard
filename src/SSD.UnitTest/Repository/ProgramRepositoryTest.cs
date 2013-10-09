using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class ProgramRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Program> MockDbSet { get; set; }
        private ProgramRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Program>>();
            MockContext.Expect(m => m.Programs).Return(MockDbSet);
            Target = new ProgramRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ProgramRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsProgram_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAProgram_WhenAdd_ThenAddToContext()
        {
            var expected = new Program { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAProgram_WhenUpdate_ThenTheContextSetsModified()
        {
            var expected = new Program { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAProgram_WhenRemove_ThenRemoveFromContext()
        {
            var expected = new Program { Id = 1 };

            Target.Remove(expected);

            MockDbSet.AssertWasCalled(m => m.Remove(expected));
        }
    }
}
