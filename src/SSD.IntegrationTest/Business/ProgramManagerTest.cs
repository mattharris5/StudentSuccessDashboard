using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Repository;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ProgramManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ProgramManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ProgramManager(repositoryContainer, new DataTableBinder());
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            if (EducationContext != null)
            {
                EducationContext.Dispose();
            }
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelHasSelectedProviders()
        {
            int id = 2;
            var expected = new List<int> { 2, 3 };

            ProgramModel actual = Target.GenerateEditViewModel(id);

            CollectionAssert.AreEqual(expected, actual.SelectedProviders.ToList());
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelHasSelectedServiceTypes()
        {
            int id = 3;
            var expected = new List<int> { 1, 2, };

            ProgramModel actual = Target.GenerateEditViewModel(id);

            CollectionAssert.AreEqual(expected, actual.SelectedServiceTypes.ToList());
        }

        [TestMethod]
        public void GivenValidViewModelHasSelectedProviders_WhenEdit_ThenProviderAssociationsChange()
        {
            try
            {
                EducationContext.Database.ExecuteSqlCommand("Update SSD.StudentAssignedOffering Set IsActive = 0");
                var expected = new int[] { 1, 2 };
                ProgramModel viewModel = new ProgramModel { Id = 2, Name = "something valid", SelectedProviders = expected, SelectedServiceTypes = new List<int> { 2 } };

                Target.Edit(viewModel);

                using (EducationDataContext assertContext = new EducationDataContext())
                {
                    var actual = assertContext.Programs.Include("ServiceOfferings.Provider").Single(p => p.Id == viewModel.Id).ServiceOfferings.Where(s => s.IsActive).Select(s => s.Provider).Distinct();
                    CollectionAssert.AreEquivalent(expected, actual.Select(p => p.Id).ToList());
                }
            }
            finally
            {
                AssemblySetup.ForceDeleteEducationDatabase("SSD");
            }
        }

        [TestMethod]
        public void GivenValidViewModelHasSelectedServiceTypes_WhenEdit_ThenServiceTypeAssociationsChange()
        {
            try
            {
                EducationContext.Database.ExecuteSqlCommand("Update SSD.StudentAssignedOffering Set IsActive = 0");
                var expected = new int[] { 1, 3 };
                ProgramModel viewModel = new ProgramModel { Id = 3, Name = "something valid", SelectedProviders = new List<int> { 3 }, SelectedServiceTypes = expected };

                Target.Edit(viewModel);

                using (EducationDataContext assertContext = new EducationDataContext())
                {
                    var actual = assertContext.Programs.Include("ServiceOfferings.ServiceType").Where(p => p.Id == viewModel.Id).Single().ServiceOfferings.Where(s => s.IsActive).Select(s => s.ServiceType).Distinct();
                    CollectionAssert.AreEqual(expected, actual.Select(t => t.Id).ToList());
                }
            }
            finally
            {
                AssemblySetup.ForceDeleteEducationDatabase("SSD");
            }
        }

        [TestMethod]
        public void GivenNoServiceTypes_WhenCreate_ThenSaveFails()
        {
            ProgramModel viewModel = new ProgramModel { Name = "something valid", SelectedProviders = new List<int> { 1, 2 } };

            Target.ExpectException<DbEntityValidationException>(() => Target.Create(viewModel));
        }

        [TestMethod]
        public void GivenNoProviders_WhenCreate_ThenSaveFails()
        {
            ProgramModel viewModel = new ProgramModel { Name = "something valid", SelectedServiceTypes = new List<int>{ 1, 2 } };

            Target.ExpectException<DbEntityValidationException>(() => Target.Create(viewModel));
        }
    }
}
