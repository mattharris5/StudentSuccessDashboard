using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.IO;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SSD.Business
{
    [TestClass]
    public class PrivateHealthFieldManagerTest : BaseManagerTest
    {
        private PrivateHealthFieldManager Target { get; set; }
        private IUserAuditor MockUserAuditor { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            IBlobClient mockBlobClient = MockRepository.GenerateMock<IBlobClient>();
            IBlobContainer mockBlobContainer = MockRepository.GenerateMock<IBlobContainer>();
            mockBlobContainer.Expect(m => m.DownloadToStream(null, null)).IgnoreArguments().Do(new Action<string, Stream>((address, target) =>
            {
                byte[] byteArray = File.ReadAllBytes(address);
                target.Write(byteArray, 0, (int)byteArray.Length);
            }));
            mockBlobContainer.Expect(m => m.UploadFromStream(null, null)).IgnoreArguments().Do(new Action<string, Stream>((address, stream) =>
            {
                File.WriteAllBytes(address, ((MemoryStream)stream).ToArray());
            }));
            mockBlobClient.Expect(m => m.CreateContainer(null)).IgnoreArguments().Return(mockBlobContainer);
            MockUserAuditor = MockRepository.GenerateMock<IUserAuditor>();
            Target = new PrivateHealthFieldManager(Repositories.MockRepositoryContainer, mockBlobClient, MockDataTableBinder, MockUserAuditor);
            ExportFileFactory.SetCurrent(MockRepository.GenerateMock<IExportFileFactory>());
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<CustomField> dataTable = MockRepository.GenerateMock<IClientDataTable<CustomField>>();
            MockDataTableBinder.Expect(m => m.Bind(Arg<IQueryable<CustomField>>.List.ContainsAll(Repositories.MockCustomFieldRepository.Items.OfType<PrivateHealthField>()), Arg.Is(dataTable), Arg.Is(requestModel))).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenPopulateViewModel_ThenProviderListPopulated()
        {
            var expectedItems = Data.Providers.ToList();
            PrivateHealthFieldModel viewModel = new PrivateHealthFieldModel();

            Target.PopulateViewModel(viewModel);

            CollectionAssert.AreEqual(expectedItems, viewModel.Providers.Items.Cast<Provider>().ToList());
            Assert.AreEqual("Id", viewModel.Providers.DataValueField);
            Assert.AreEqual("Name", viewModel.Providers.DataTextField);
        }

        [TestMethod]
        public void GivenInvalidViewModel_WhenPopulateViewModel_ThenThrowException()
        {
            PublicFieldModel invalid = new PublicFieldModel();

            Target.ExpectException<ArgumentException>(() => Target.PopulateViewModel(invalid));
        }

        [TestMethod]
        public void WhenCreate_ThenPublicFieldAddedWithCorrectState()
        {
            var viewModel = new PrivateHealthFieldModel
            {
                FieldName = "Test",
                SelectedFieldTypeId = 1,
                SelectedCategories = new List<int> { 1, 2 },
                SelectedProviderId = 3829
            };

            Target.Create(viewModel, User);

            Repositories.MockCustomFieldRepository.AssertWasCalled(m => m.Add(Arg<PrivateHealthField>.Matches(c => AssertPropertiesMatch(viewModel, c))));
        }

        [TestMethod]
        public void GivenTypeIsPrivateHealthField_WhenIGenerateMapFieldsViewModel_ThenAPopulatedUploadWizardViewModelIsReturnedWithOnlyPrivateHealthFields()
        {
            var expectedPublicFields = Data.CustomFields.OrderBy(c => c.Name).OfType<PrivateHealthField>().ToList();
            expectedPublicFields.Insert(0,
                new PrivateHealthField
                {
                    Id = 0,
                    Name = "Student Id"
                }
            );
            var viewModel = new UploadWizardFileViewModel
            {
                NumberOfFields = 1,
                File = MockRepository.GenerateMock<HttpPostedFileBase>()
            };
            viewModel.File.Expect(f => f.InputStream).Return(new MemoryStream());
            PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Data.CustomFields.Last())).Return(MockRepository.GenerateMock<IPermission>());

            var model = Target.GenerateMapFieldsViewModel(viewModel, typeof(PrivateHealthField), User) as UploadWizardModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedPublicFields.Count, model.CustomFields[0].CustomFields.Count());
        }

        [TestMethod]
        public void GivenTypeIsPrivateHealthField_AndUserDoesntHaveAccessToAllFields_WhenIGenerateMapFieldsViewModel_ThenAPopulatedUploadWizardViewModelIsReturnedWithOnlyExpectedPrivateHealthFields()
        {
            var expectedPublicFields = Data.CustomFields.OrderBy(c => c.Name).OfType<PrivateHealthField>().ToList();
            expectedPublicFields.Insert(0,
                new PrivateHealthField
                {
                    Id = 0,
                    Name = "Student Id"
                }
            );
            var viewModel = new UploadWizardFileViewModel
            {
                NumberOfFields = 1,
                File = MockRepository.GenerateMock<HttpPostedFileBase>()
            };
            viewModel.File.Expect(f => f.InputStream).Return(new MemoryStream());
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Data.CustomFields.Last())).Return(permission);

            var model = Target.GenerateMapFieldsViewModel(viewModel, typeof(PrivateHealthField), User) as UploadWizardModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedPublicFields.Count - 1, model.CustomFields[0].CustomFields.Count());
        }

        private bool AssertPropertiesMatch(PrivateHealthFieldModel expectedState, PrivateHealthField actualState)
        {
            Assert.IsNotNull(actualState);
            Assert.AreEqual(expectedState.FieldName, actualState.Name);
            Assert.AreEqual(expectedState.SelectedFieldTypeId, actualState.CustomFieldTypeId);
            CollectionAssert.AreEqual(expectedState.SelectedCategories.ToList(), actualState.Categories.Select(c => c.Id).ToList());
            Assert.AreEqual(expectedState.SelectedProviderId, actualState.ProviderId);
            return true;
        }
    }
}
