using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using Rhino.Mocks;
using SSD.Domain;
using SSD.IO;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SSD.Business
{
    [TestClass]
    public class PublicFieldManagerTest : BaseManagerTest
    {
        private PublicFieldManager Target { get; set; }
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
            Target = new PublicFieldManager(Repositories.MockRepositoryContainer, mockBlobClient, MockDataTableBinder, MockUserAuditor);
            ExportFileFactory.SetCurrent(MockRepository.GenerateMock<IExportFileFactory>());
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            IBlobClient mockBlobClient = MockRepository.GenerateMock<IBlobClient>();
            TestExtensions.ExpectException<ArgumentNullException>(() => new PublicFieldManager(null, mockBlobClient, MockDataTableBinder, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullBlobClient_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new PublicFieldManager(Repositories.MockRepositoryContainer, null, MockDataTableBinder, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenConstruct_ThenThrowException()
        {
            IBlobClient mockBlobClient = MockRepository.GenerateMock<IBlobClient>();
            TestExtensions.ExpectException<ArgumentNullException>(() => new PublicFieldManager(Repositories.MockRepositoryContainer, mockBlobClient, null, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullUserAuditor_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new PublicFieldManager(Repositories.MockRepositoryContainer, null, MockDataTableBinder, null));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<CustomField> dataTable = MockRepository.GenerateMock<IClientDataTable<CustomField>>();
            MockDataTableBinder.Expect(m => m.Bind(Arg<IQueryable<CustomField>>.List.ContainsAll(Repositories.MockCustomFieldRepository.Items.OfType<PublicField>()), Arg.Is(dataTable), Arg.Is(requestModel))).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenTheViewModelIsNull_WhenPopulateViewModel_ThenAnArgumentNullExceptionIsThrown()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.PopulateViewModel(null));
        }

        [TestMethod]
        public void WhenPopulateViewModel_ThenTheListsArePopulated()
        {
            var expectedCategories = Data.CustomFieldCategories;
            var expectedFieldTypes = Data.CustomFieldTypes;
            var viewModel = new PublicFieldModel();

            Target.PopulateViewModel(viewModel);

            Assert.IsNotNull(viewModel.Categories);
            Assert.AreEqual("Id", viewModel.Categories.DataValueField);
            Assert.AreEqual("Name", viewModel.Categories.DataTextField);
            CollectionAssert.AreEqual(expectedCategories, viewModel.Categories.Items.Cast<CustomFieldCategory>().ToList());
            Assert.IsNotNull(viewModel.FieldTypes);
            Assert.AreEqual("Id", viewModel.FieldTypes.DataValueField);
            Assert.AreEqual("Name", viewModel.FieldTypes.DataTextField);
            CollectionAssert.AreEqual(expectedFieldTypes, viewModel.FieldTypes.Items.Cast<CustomFieldType>().ToList());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenAViewModelIsReturned()
        {
            var actual = Target.GenerateCreateViewModel() as PublicFieldModel;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenTheListsArePopulated()
        {
            var expectedCategories = Data.CustomFieldCategories;
            var expectedFieldTypes = Data.CustomFieldTypes;

            var actual = Target.GenerateCreateViewModel();

            Assert.IsNotNull(actual.Categories);
            Assert.AreEqual("Id", actual.Categories.DataValueField);
            Assert.AreEqual("Name", actual.Categories.DataTextField);
            CollectionAssert.AreEqual(expectedCategories, actual.Categories.Items.Cast<CustomFieldCategory>().ToList());
            Assert.IsNotNull(actual.FieldTypes);
            Assert.AreEqual("Id", actual.FieldTypes.DataValueField);
            Assert.AreEqual("Name", actual.FieldTypes.DataTextField);
            CollectionAssert.AreEqual(expectedFieldTypes, actual.FieldTypes.Items.Cast<CustomFieldType>().ToList());
        }

        [TestMethod]
        public void GivenTheIdIsInvalid_WhenGenerateEditViewModel_ThenAnEntityNotFoundExceptionIsThrown()
        {
            TestExtensions.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(3234332, User));
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenAValidViewModelIsReturned()
        {
            var actual = Target.GenerateEditViewModel(1, User) as PublicFieldModel;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenViewModelContainsState()
        {
            var expectedState = Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == 1).SingleOrDefault();

            var actualState = Target.GenerateEditViewModel(1, User) as PublicFieldModel;

            Assert.AreEqual(expectedState.Id, actualState.Id);
            Assert.AreEqual(expectedState.Name, actualState.FieldName);
            CollectionAssert.AreEqual(expectedState.Categories.Select(c => c.Id).ToList(), actualState.SelectedCategories.ToList());
            Assert.AreEqual(expectedState.CustomFieldTypeId, actualState.SelectedFieldTypeId);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenAnArgumentNullExceptionIsThrown()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Create(null, User));
        }

        [TestMethod]
        public void GivenNullUser_WhenCreate_ThenAnArgumentNullExceptionIsThrown()
        {
            var viewModel = new PublicFieldModel();

            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Create(viewModel, null));
        }

        [TestMethod]
        public void WhenCreate_ThenSaveIsCalled()
        {
            var viewModel = new PublicFieldModel
            {
                FieldName = "Test",
                SelectedFieldTypeId = 1,
                SelectedCategories = new List<int> { 1, 2 }
            };

            Target.Create(viewModel, User);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void WhenCreate_ThenPublicFieldAddedWithCorrectState()
        {
            var viewModel = new PublicFieldModel
            {
                FieldName = "Test",
                SelectedFieldTypeId = 1,
                SelectedCategories = new List<int> { 1, 2 }
            };

            Target.Create(viewModel, User);

            Repositories.MockCustomFieldRepository.AssertWasCalled(m => m.Add(Arg<PublicField>.Matches(c => AssertPropertiesMatch(viewModel, c))));
        }

        [TestMethod]
        public void GivenAnInvalidSelectedCategory_WhenCreate_ThenSaveIsCalled()
        {
            var viewModel = new PublicFieldModel
            {
                FieldName = "Test",
                SelectedFieldTypeId = 1,
                SelectedCategories = new List<int> { 1, 100 }
            };

            Target.ExpectException<EntityNotFoundException>(() => Target.Create(viewModel, User));
        }

        [TestMethod]
        public void GivenNullModel_WhenEdit_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Edit(null, User));
        }

        [TestMethod]
        public void GivenNullUser_WhenEdit_ThenThrowException()
        {
            var viewModel = new PublicFieldModel { Id = 45454 };

            Target.ExpectException<ArgumentNullException>(() => Target.Edit(viewModel, null));
        }

        [TestMethod]
        public void GivenAnInvalidModel_WhenEdit_ThenAnEntityNotFoundException()
        {
            var viewModel = new PublicFieldModel { Id = 45454 };

            TestExtensions.ExpectException<EntityNotFoundException>(() => Target.Edit(viewModel, User));
        }
        
        [TestMethod]
        public void WhenEdit_ThenEntityUpdated_AndChangesSaved()
        {
            int publicFieldId = 1;
            var publicField = Data.CustomFields.Single(c => c.Id == publicFieldId);
            var viewModel = new PublicFieldModel
            {
                Id = publicFieldId,
                FieldName = "testing",
                SelectedFieldTypeId = 1
            };

            Target.Edit(viewModel, User);

            Repositories.MockCustomFieldRepository.AssertWasCalled(m => m.Update(publicField));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void WhenEdit_ThenEntityModifiedPropertiesSet()
        {
            int publicFieldId = 1;
            var publicField = Data.CustomFields.Single(c => c.Id == publicFieldId);
            var viewModel = new PublicFieldModel
            {
                Id = publicFieldId,
                SelectedFieldTypeId = publicField.CustomFieldTypeId
            };

            Target.Edit(viewModel, User);

            Assert.IsTrue(publicField.LastModifyTime.Value.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now));
            Assert.AreEqual(User.Identity.User, publicField.LastModifyingUser);
        }

        [TestMethod]
        public void GivenPublicFieldViewModel_AndPublicFieldHasValues_AndServiceTypeIsDifferent_WhenEdit_ThenThrowValidationException_AndValidationExceptionIsForSelectedFieldTypeId()
        {
            var viewModel = new PublicFieldModel { Id = 1, SelectedFieldTypeId = 2 };

            ValidationException actual = Target.ExpectException<ValidationException>(() => Target.Edit(viewModel, User));

            CollectionAssert.Contains(actual.ValidationResult.MemberNames.ToList(), "SelectedFieldTypeId");
        }

        [TestMethod]
        public void GivenPublicFieldViewModel_AndPublicFieldHasNoValues_AndServiceTypeIsDifferent_WhenEdit_ThenSucceed()
        {
            var viewModel = new PublicFieldModel { Id = 4, SelectedFieldTypeId = 1 };

            Target.Edit(viewModel, User);
        }

        [TestMethod]
        public void WhenIGetDeleteModel_ThenAPublicFieldIsReturned()
        {
            var result = Target.GenerateDeleteViewModel(1) as PublicFieldModel;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenAnInvalidId_WhenIGetDeleteModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(78786));
        }

        [TestMethod]
        public void GivenInvalidId_WhenDelete_ThenEntityNotFoundExceptionIsReturned()
        {
            TestExtensions.ExpectException<EntityNotFoundException>(() => Target.Delete(68686));
        }

        [TestMethod]
        public void WhenDelete_ThenEntityRemoved_AndChangesSaved()
        {
            int publicFieldId = 4;
            PublicField expected = Data.CustomFields.OfType<PublicField>().Single(c => c.Id == publicFieldId);

            Target.Delete(publicFieldId);

            Repositories.MockCustomFieldRepository.AssertWasCalled(c => c.Remove(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(c => c.Save());
        }

        [TestMethod]
        public void GivenPublicFieldHasAssociatedValues_WhenDelete_ThenThrowValidationException()
        {
            Target.ExpectException<ValidationException>(() => Target.Delete(1));
        }

        [TestMethod]
        public void GivenDuplicateFieldName_WhenValidate_ThenThrowValidationException()
        {
            var viewModel = new PublicFieldModel { FieldName = Repositories.MockCustomFieldRepository.Items.First().Name };

            Target.ExpectException<ValidationException>(() => Target.Validate(viewModel));
        }

        [TestMethod]
        public void GivenDuplicateFieldName_WhenValidate_ThenValidationExceptionMemberNamesIncludesFieldName()
        {
            var viewModel = new PublicFieldModel { FieldName = Repositories.MockCustomFieldRepository.Items.First().Name };

            ValidationException actual = Target.ExpectException<ValidationException>(() => Target.Validate(viewModel));

            Assert.IsTrue(actual.ValidationResult.MemberNames.Contains("FieldName"));
        }

        [TestMethod]
        public void GivenDuplicateFieldName_AndIdMatches_WhenValidate_ThenSucceed()
        {
            var viewModel = new PublicFieldModel { FieldName = Repositories.MockCustomFieldRepository.Items.First().Name, Id = Repositories.MockCustomFieldRepository.Items.First().Id };

            Target.Validate(viewModel);
        }

        [TestMethod]
        public void GivenFieldNameDiffers_WhenValidate_ThenSucceed()
        {
            var viewModel = new PublicFieldModel { FieldName = "this is a new name" };

            Target.Validate(viewModel);
        }

        [TestMethod]
        public void GivenTypeIsPublicField_WhenIGenerateMapFieldsViewModel_ThenAPopulatedUploadWizardViewModelIsReturnedWithOnlyPublicFields()
        {
            var expectedPublicFields = Data.CustomFields.OrderBy(c => c.Name).OfType<PublicField>().ToList();
            expectedPublicFields.Insert(0,
                new PublicField
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
            foreach (var field in Data.CustomFields.OfType<PublicField>())
            {
                PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }
            var model = Target.GenerateMapFieldsViewModel(viewModel, typeof(PublicField), User) as UploadWizardModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedPublicFields.Count, model.CustomFields[0].CustomFields.Count());
        }

        [TestMethod]
        public void GivenANullModel_WhenIGenerateMapFieldsViewModel_ThenAnArgumentNullExceptionIsThrown()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.GenerateMapFieldsViewModel(null, typeof(PublicField), User));
        }

        [TestMethod]
        public void WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/HappyPath.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 3
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ProcessedRowCount, model.SuccessfulRowsCount);
            Assert.IsTrue(model.ProcessedRowCount > 0);
            Assert.IsTrue(model.RowErrors.Count == 0);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void WhenIPostCompleteTheWizardWithPartialRights_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            var students = Repositories.MockStudentRepository.Items.Where(s => s.StudentSISId == "10" || s.StudentSISId == "20" || s.StudentSISId == "30");
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", students.ToArray()[0])).Return(MockRepository.GenerateMock<IPermission>());
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", students.ToArray()[1])).Return(permission);
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", students.ToArray()[2])).Return(permission);
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/HappyPath.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 3
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.SuccessfulRowsCount);
            Assert.IsTrue(model.ProcessedRowCount == 3);
            Assert.IsTrue(model.RowErrors.Count == 2);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenNoRightsToSelectedCustomFields_WhenIPostcompleteThenWizard_ThenExceptionThrown()
        {
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/HappyPath.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 3
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };

            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == 2).Single())).Return(permission);
                
            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(0, actualAdded.Count);
            Assert.AreEqual(1, model.RowErrors.Count);
            Assert.AreEqual("You don't have access to one or more of the selected custom fields. Re-submit and try again.", model.RowErrors[0]);
        }

        [TestMethod]
        public void GivenTheCustomFieldIsOfDateTime_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/DateField.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 2,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 5
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ProcessedRowCount, model.SuccessfulRowsCount);
            Assert.IsTrue(model.ProcessedRowCount > 0);
            Assert.IsTrue(model.RowErrors.Count == 0);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenThePublicFieldIsOfDateTimeAndThereIsAnInvalidRow_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/InvalidDateField.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 2,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 5
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ProcessedRowCount, model.SuccessfulRowsCount + 1);
            Assert.IsTrue(model.ProcessedRowCount > 0);
            Assert.IsTrue(model.RowErrors.Count == 1);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenThePublicFieldIsOfInteger_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/IntegerField.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 2,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 3
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ProcessedRowCount, model.SuccessfulRowsCount);
            Assert.IsTrue(model.ProcessedRowCount > 0);
            Assert.IsTrue(model.RowErrors.Count == 0);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenThePublicFieldIsOfIntegerAndOneOfTheRowsIsInvalid_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/InvalidIntegerField.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 2,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 3
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ProcessedRowCount, model.SuccessfulRowsCount + 1);
            Assert.IsTrue(model.ProcessedRowCount > 0);
            Assert.IsTrue(model.RowErrors.Count == 1);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenThereIsAnInvalidStudentIdInOneOfTheRows_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/InvalidStudentId.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 4
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 4
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.ProcessedRowCount, model.SuccessfulRowsCount + 1);
            Assert.IsTrue(model.ProcessedRowCount > 0);
            Assert.IsTrue(model.RowErrors.Count == 1);
            Repositories.MockCustomDataOriginRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockCustomFieldValueRepository.AssertWasCalled(m => m.Add(null), options => options.IgnoreArguments());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenFileHasTwoColumnsInAdditionToStudentId_AndLastTextColumnMissingData_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/LastTextColumnMissingValue.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 2,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 7
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 4
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(1, actualAdded.Count);
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenIPostCompleteTheWizard_ThenRecordIsImported()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/StudentApprovesProviderAndHasServiceOffering.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 2,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 7
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 4
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(1, actualAdded.Count);
        }

        [TestMethod]
        public void GivenValidIds_WhenIPostCompleteTheWizard_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(permission);
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/StudentApprovesProviderAndHasServiceOffering.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 2,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 7
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 4
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenPermissionDoesntGrantAccess_AndFileHasStudentIdThatApprovesProvider_AndProviderDoesNotServeStudent_WhenIPostCompleteTheWizard_ThenRecordIsNotImported()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(permission);

            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            User.Identity.User.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = SecurityRoles.Provider }, Providers = Data.Providers
                }
            };
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/StudentApprovesProviderButHasNoServiceOffering.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 2,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 7
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 4
                    },
                }
            };

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(0, actualAdded.Count);
        }

        [TestMethod]
        public void GivenFileHasTwoColumnsInAdditionToStudentId_AndLastIntegerColumnMissingData_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            User.Identity.User.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = SecurityRoles.DataAdmin }
                }
            };
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/LastIntegerColumnMissingValue.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 2,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 6
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(1, actualAdded.Count);
        }

        [TestMethod]
        public void GivenFileHasTwoColumnsInAdditionToStudentId_AndOnlyIdHasData_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsNotReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            User.Identity.User.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = SecurityRoles.DataAdmin }
                }
            };
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/NoOtherFieldButIDPopulated.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 2,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 6
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.IsTrue(model.SuccessfulRowsCount == 0);
            Assert.IsTrue(model.RowErrors.Count == 1);
        }

        [TestMethod]
        public void GivenFileHasTwoColumnsInAdditionToStudentId_AndLastIntegerColumnMissingData_AndColumnHasNoDelimiter_WhenIPostCompleteTheWizard_ThenAnUploadWizardCompleteViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            User.Identity.User.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = SecurityRoles.DataAdmin }
                }
            };
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/LastIntegerColumnMissingValueAndDelimiter.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 3,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 6
                    },
                }
            };
            foreach (var field in uploadModel.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    PermissionFactory.Current.Expect(m => m.Create("UploadCustomFieldData", Repositories.MockCustomFieldRepository.Items.Where(c => c.Id == field.SelectedCustomFieldId).Single())).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(6, actualAdded.Count);
        }

        [TestMethod]
        public void GivenUserIndicatesADifferentAmountOfColumnsThanAreInTheFile_WhenIPostCompleteTheWizard_ThenTheModelHasErrors()
        {
            PermissionFactory.Current.Expect(m => m.Create("ProcessDataFile", null)).IgnoreArguments().Return(MockRepository.GenerateMock<IPermission>());
            List<CustomFieldValue> actualAdded = new List<CustomFieldValue>();
            Repositories.MockCustomFieldValueRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<CustomFieldValue>((item) => { actualAdded.Add(item); }));
            User.Identity.User.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = SecurityRoles.DataAdmin }
                }
            };
            var uploadModel = new UploadWizardModel
            {
                BlobAddress = Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "DataFileWizard/ColumnNumberError.txt"),
                FieldNameRow = 1,
                FirstDataRow = 2,
                LastDataRow = 5,
                NumberOfFields = 2,
                Source = "test",
                CustomFields = new List<CustomFieldSelectModel>
                {
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 2
                    },
                    new CustomFieldSelectModel
                    {
                        SelectedCustomFieldId = 0
                    },
                }
            };

            var model = Target.GenerateUploadWizardCompleteViewModel(User, uploadModel) as UploadWizardCompleteModel;

            Assert.AreEqual(0, actualAdded.Count);
            Assert.AreEqual(1, model.RowErrors.Count);
            Assert.AreEqual("There is a different amount of columns in the file than listed. Please re-submit the file and try again.", model.RowErrors[0]);
        }

        [TestMethod]
        public void GivenABlobAddress_WhenIRetrieveUploadErrorsFile_ThenADownloadFileViewModelIsReturned()
        {
            var result = Target.RetrieveUploadErrorsFile(Path.GetFullPath(ConfigurationManager.AppSettings["FileUploadTemplatePath"] + "blah")) as DownloadFileModel;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenDataExtractModel_WhenRetrieveStudentsList_ThenCollectionOfStudentsReturned()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();

            var result = Target.RetrieveStudentsList(model);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenSelectedSchools_WhenRetrieveStudentsList_ThenCollectionFilteredBySchools()
        {
            List<School> selectedSchools = new List<School> { Data.Schools[0], Data.Schools[1] };
            var expected = Data.Students.Where(s => selectedSchools.Contains(s.School));
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedSchoolIds = selectedSchools.Select(s => s.Id) };

            var actual = Target.RetrieveStudentsList(model);

            CollectionAssert.AreEqual(expected.Select(e => e.Id).ToList(), actual.Select(a => a.Id).ToList());
        }

        [TestMethod]
        public void GivenNoSelectedSchools_AndNoSelectedGrades_WhenRetrieveStudentsList_ThenAllStudentsReturned()
        {
            var expected = Data.Students;
            StudentProfileExportModel model = new StudentProfileExportModel();

            var actual = Target.RetrieveStudentsList(model);

            CollectionAssert.AreEqual(expected.Select(e => e.Id).ToList(), actual.Select(a => a.Id).ToList());
        }

        [TestMethod]
        public void GivenSelectedGrades_WhenRetrieveStudentsList_ThenCollectionFilteredByGrade()
        {
            var expected = Data.Students.Where(s => s.Grade == 9 || s.Grade == 11);
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedGrades = new List<int> { 9, 11 } };

            var actual = Target.RetrieveStudentsList(model);

            CollectionAssert.AreEqual(expected.Select(e => e.Id).ToList(), actual.Select(a => a.Id).ToList());
        }

        [TestMethod]
        public void GivenSelectedSchools_AndSelectedGrades_WhenRetrieveStudentsList_ThenFilteredOnBoth()
        {
            List<School> selectedSchools = new List<School> { Data.Schools[0], Data.Schools[1] };
            var expected = Data.Students.Where(s => selectedSchools.Contains(s.School) && (s.Grade == 9 || s.Grade == 11));
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedSchoolIds = selectedSchools.Select(s => s.Id), SelectedGrades = new List<int> { 9, 11 } };

            var actual = Target.RetrieveStudentsList(model);

            CollectionAssert.AreEqual(expected.Select(e => e.Id).ToList(), actual.Select(a => a.Id).ToList());
        }

        [TestMethod]
        public void GivenModel_AndStudentCountLimit_WhenCheckStudentCount_ThenSucceed()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();

            Target.CheckStudentCount(model, 20);
        }

        [TestMethod]
        public void GivenModelWillGenerateMoreResultsThanFileSizeLimit_WhenCheckStudentCount_ThenThrowException()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();

            Target.ExpectException<ArgumentOutOfRangeException>(() => Target.CheckStudentCount(model, 2));
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenStreamReturned()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(model, student, User, MockUserAuditor)).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(model)).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenContextSaves()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(model, student, User, MockUserAuditor)).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(model)).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenGenerateMapperCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(model, student, User, MockUserAuditor)).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(model)).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            mockExportFile.AssertWasCalled(m => m.GenerateMapper());
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenCreateCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(model, student, User, MockUserAuditor)).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            mockExportFile.AssertWasCalled(m => m.Create(result));
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenSetupColumnHeadersCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(model, student, User, MockUserAuditor)).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            mockExportFile.AssertWasCalled(m => m.SetupColumnHeaders(colHeadings));
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_AndUserDoesntHaveAccessToAllPrivateHealthFields_WhenGenerateStudentProfileExport_ThenMapColumnHeadingsCalledWithCorrectColumnHeaders()
        {
            StudentProfileExportModel model = new StudentProfileExportModel { SelectedCustomFieldIds = new List<int> { 1, 2 } };
            StudentProfileExportModel expectedModel = new StudentProfileExportModel { SelectedCustomFieldIds = new List<int> { 1 } };
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", Data.CustomFields.Single(c => c.Id == 1))).Return(MockRepository.GenerateMock<IPermission>());
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", Data.CustomFields.Single(c => c.Id == 2))).Return(permission);
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(expectedModel, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor))).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(expectedModel, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            mockDataMapper.AssertWasCalled(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(expectedModel, a))));
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenMapColumnHeadingsCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor))).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            mockDataMapper.AssertWasCalled(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a))));
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenMapDataCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor))).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            foreach (var student in students)
            {
                mockDataMapper.AssertWasCalled(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor)));
            }
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenFillDataCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            var studentFieldData = new Dictionary<Student, IEnumerable<object>>();
            foreach (var student in students)
            {
                IEnumerable<object> rowData = new List<object>();
                mockDataMapper.Expect(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor))).Return(rowData);
                studentFieldData.Add(student, rowData);
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            foreach (var student in students)
            {
                mockDataMapper.AssertWasCalled(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor)));
            }
            mockExportFile.AssertWasCalled(m => m.FillData(Arg<IEnumerable<IEnumerable<object>>>.Matches(a => AssertDataMatches(studentFieldData, a))));
        }

        [TestMethod]
        public void GivenModel_AndIExportFile_WhenGenerateStudentProfileExport_ThenSetupFooterCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                mockDataMapper.Expect(m => m.MapData(model, student, User, MockUserAuditor)).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(model)).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            mockExportFile.AssertWasCalled(m => m.SetupFooter(CloudConfigurationManager.GetSetting("StudentProfileExportFooter")));
        }

        [TestMethod]
        public void GivenStudentHasOptOut_AndUserIsDataAdmin_WhenGenerateStudentProfileExport_ThenMapDataCalled()
        {
            StudentProfileExportModel model = new StudentProfileExportModel();
            IExportFile mockExportFile = MockRepository.GenerateMock<IExportFile>();
            IExportDataMapper mockDataMapper = MockRepository.GenerateMock<IExportDataMapper>();
            List<IEnumerable<object>> data = new List<IEnumerable<object>>();
            byte[] byteArray = File.ReadAllBytes(@"TestData\StudentProfileExportTemplate.xltx");
            MemoryStream stream = new MemoryStream(byteArray);
            var students = Target.RetrieveStudentsList(model);
            foreach (var student in students)
            {
                student.HasParentalOptOut = true;
                mockDataMapper.Expect(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor))).Return(new List<object>());
            }
            List<string> colHeadings = new List<string>();
            mockDataMapper.Expect(m => m.MapColumnHeadings(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)))).Return(colHeadings);
            mockExportFile.Expect(m => m.GenerateMapper()).Return(mockDataMapper);
            ExportFileFactory.Current.Expect(m => m.Create(typeof(StudentProfileExportFile))).Return(mockExportFile);
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            var result = Target.GenerateStudentProfileExport(User, model, @"TestData\StudentProfileExportTemplate.xltx") as Stream;

            foreach (var student in students)
            {
                mockDataMapper.AssertWasCalled(m => m.MapData(Arg<StudentProfileExportFieldDescriptor>.Matches(a => AssertDescriptorMatches(model, a)), Arg.Is(student), Arg.Is(User), Arg.Is(MockUserAuditor)));
            }
        }

        [TestMethod]
        public void WhenGenerateSelectorViewModel_ThenReturnInstance()
        {
            foreach (var field in Data.CustomFields)
            {
                PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            CustomFieldSelectorModel actual = Target.GenerateSelectorViewModel(User);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenNoPrivateHealthFields_WhenGenerateSelectorViewModel_ThenViewModelContainsCustomFieldData()
        {
            Data.CustomFields.RemoveAll(f => f is PrivateHealthField);
            List<int> expectedIds = Repositories.MockCustomFieldRepository.Items.Select(f => f.Id).ToList();
            List<string> expectedNames = Repositories.MockCustomFieldRepository.Items.Select(f => f.Name).ToList();

            foreach (var field in Data.CustomFields)
            {
                PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            CustomFieldSelectorModel actual = Target.GenerateSelectorViewModel(User);

            CollectionAssert.AreEquivalent(expectedIds, actual.CustomFields.Items.Cast<dynamic>().Select(f => f.Id).ToList());
            CollectionAssert.AreEquivalent(expectedNames, actual.CustomFields.Items.Cast<dynamic>().Select(f => f.Name).Cast<string>().ToList());
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenGenerateSelectorViewModel_ThenCustomFieldNameHasAsterisk()
        {
            Data.CustomFields.Clear();
            Data.CustomFields.Add(new PrivateHealthField { Name = "blah" });
            foreach (var field in Data.CustomFields)
            {
                PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            CustomFieldSelectorModel actual = Target.GenerateSelectorViewModel(User);

            Assert.AreEqual("blah *", actual.CustomFields.Single().Text);
        }

        [TestMethod]
        public void GivenPrivateHealthFields_AndUserDoesntHaveAccessToAll_ThenExpectedCustomFieldsReturned()
        {
            Data.CustomFields.Clear();
            Data.CustomFields.Add(new PrivateHealthField { Name = "blah", Id = 1 });
            Data.CustomFields.Add(new PrivateHealthField { Name = "bleh", Id = 2 });
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", Data.CustomFields.Single(c => c.Id == 1))).Return(MockRepository.GenerateMock<IPermission>());
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", Data.CustomFields.Single(c => c.Id == 2))).Return(permission);

            CustomFieldSelectorModel actual = Target.GenerateSelectorViewModel(User);

            Assert.AreEqual(1, actual.CustomFields.Count());
        }

        [TestMethod]
        public void WhenGenerateSelectorViewModel_ThenCustomFieldDataSorted()
        {
            foreach (var field in Data.CustomFields)
            {
                PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            CustomFieldSelectorModel viewModel = Target.GenerateSelectorViewModel(User);

            var actual = viewModel.CustomFields.ToList();
            var expected = actual.OrderBy(i => i.Text).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhenGenerateSelectorViewModel_ThenSchoolListPropertiesCorrect()
        {
            foreach (var field in Data.CustomFields)
            {
                PermissionFactory.Current.Expect(m => m.Create("StudentProfileExportCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            CustomFieldSelectorModel actual = Target.GenerateSelectorViewModel(User);

            Assert.AreEqual("Id", actual.CustomFields.DataValueField);
            Assert.AreEqual("Name", actual.CustomFields.DataTextField);
        }

        private static bool AssertPropertiesMatch(PublicFieldModel expectedState, PublicField actualState)
        {
            Assert.IsNotNull(actualState);
            Assert.AreEqual(expectedState.FieldName, actualState.Name);
            Assert.AreEqual(expectedState.SelectedFieldTypeId, actualState.CustomFieldTypeId);
            CollectionAssert.AreEqual(expectedState.SelectedCategories.ToList(), actualState.Categories.Select(c => c.Id).ToList());
            return true;
        }

        private bool AssertDescriptorMatches(StudentProfileExportModel expectedState, StudentProfileExportFieldDescriptor actualState)
        {
            Assert.AreEqual(expectedState.BirthDateIncluded, actualState.BirthDateIncluded);
            Assert.AreEqual(expectedState.ParentNameIncluded, actualState.ParentNameIncluded);
            CollectionAssert.AreEqual(Repositories.MockCustomFieldRepository.Items.Where(c => expectedState.SelectedCustomFieldIds.Contains(c.Id)).ToList(), actualState.SelectedCustomFields.ToList());
            CollectionAssert.AreEqual(Repositories.MockServiceTypeRepository.Items.Where(c => expectedState.SelectedServiceTypeIds.Contains(c.Id)).ToList(), actualState.SelectedServiceTypes.ToList());
            return true;
        }

        private bool AssertDataMatches(Dictionary<Student, IEnumerable<object>> expectedItems, IEnumerable<IEnumerable<object>> actualItems)
        {
            CollectionAssert.AreEqual(expectedItems.Values, actualItems.ToList());
            return true;
        }
    }
}
