using Microsoft.WindowsAzure;
using SSD.Domain;
using SSD.IO;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace SSD.Business
{
    public abstract class CustomFieldManager : ICustomFieldManager
    {
        private static readonly string DataFileBlobContainerName = CloudConfigurationManager.GetSetting("CustomDataFileContainerName");

        private IRepositoryContainer RepositoryContainer { get; set; }
        private ICustomDataOriginRepository CustomDataOriginRepository { get; set; }
        private ICustomFieldCategoryRepository CustomFieldCategoryRepository { get; set; }
        protected ICustomFieldRepository CustomFieldRepository { get; private set; }
        private ICustomFieldTypeRepository CustomFieldTypeRepository { get; set; }
        private ICustomFieldValueRepository CustomFieldValueRepository { get; set; }
        private IStudentRepository StudentRepository { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private IBlobContainer DataFileBlobContainer { get; set; }
        protected IDataTableBinder DataTableBinder { get; private set; }
        private IUserAuditor Auditor { get; set; }

        protected CustomFieldManager(IRepositoryContainer repositories, IBlobClient blobClient, IDataTableBinder dataTableBinder, IUserAuditor auditor)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            if (blobClient == null)
            {
                throw new ArgumentNullException("blobClient");
            }
            if (dataTableBinder == null)
            {
                throw new ArgumentNullException("dataTableBinder");
            }
            if (auditor == null)
            {
                throw new ArgumentNullException("auditor");
            }
            RepositoryContainer = repositories;
            CustomDataOriginRepository = repositories.Obtain<ICustomDataOriginRepository>();
            CustomFieldCategoryRepository = repositories.Obtain<ICustomFieldCategoryRepository>();
            CustomFieldRepository = repositories.Obtain<ICustomFieldRepository>();
            CustomFieldTypeRepository = repositories.Obtain<ICustomFieldTypeRepository>();
            CustomFieldValueRepository = repositories.Obtain<ICustomFieldValueRepository>();
            StudentRepository = repositories.Obtain<IStudentRepository>();
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            DataFileBlobContainer = blobClient.CreateContainer(DataFileBlobContainerName);
            DataTableBinder = dataTableBinder;
            Auditor = auditor;
        }

        public abstract DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<CustomField> dataTable);

        public CustomFieldModel GenerateCreateViewModel()
        {
            CustomFieldModel viewModel = CreateFieldModel();
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public CustomFieldModel GenerateEditViewModel(int id, EducationSecurityPrincipal user)
        {
            var model = CustomFieldRepository.Items.
                        Include(c => c.Categories).
                        Include(c => c.LastModifyingUser).
                        Include(c => c.CreatingUser).
                        SingleOrDefault(c => c.Id == id);
            if (model == null)
            {
                throw new EntityNotFoundException("The requested custom field could not be found");
            }
            CustomFieldModel viewModel = CreateFieldModel();
            viewModel.CopyFrom(model);
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public void Create(CustomFieldModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            CustomField item = CreateFieldEntity();
            viewModel.CopyTo(item);
            item.Categories = CustomFieldCategoryRepository.Items.Where(c => viewModel.SelectedCategories.Contains(c.Id)).ToList();
            if (item.Categories.Count != viewModel.SelectedCategories.Count())
            {
                throw new EntityNotFoundException("At least one selected category could not be found.");
            }
            item.CreatingUserId = user.Identity.UserEntity.Id;
            CustomFieldRepository.Add(item);
            RepositoryContainer.Save();
        }

        public void Edit(CustomFieldModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var model = CustomFieldRepository.Items.Include(c => c.Categories).SingleOrDefault(c => c.Id == viewModel.Id);
            if (model == null)
            {
                throw new EntityNotFoundException("The requested custom field could not be found");
            }
            if (CustomFieldValueRepository.Items.Any(v => v.CustomFieldId == viewModel.Id) && model.CustomFieldTypeId != viewModel.SelectedFieldTypeId)
            {
                throw new ValidationException(new ValidationResult("Cannot edit field type because data values have already been loaded", new[] { "SelectedFieldTypeId" }), null, viewModel);
            }
            viewModel.CopyTo(model);
            model.LastModifyingUser = user.Identity.User;
            model.LastModifyTime = DateTime.Now;
            model.Categories = CustomFieldCategoryRepository.Items.Where(c => viewModel.SelectedCategories.Contains(c.Id)).ToList();
            CustomFieldRepository.Update(model);
            RepositoryContainer.Save();
        }

        public virtual void PopulateViewModel(CustomFieldModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            viewModel.Categories = new MultiSelectList(CustomFieldCategoryRepository.Items, "Id", "Name", viewModel.SelectedCategories);
            viewModel.FieldTypes = new SelectList(CustomFieldTypeRepository.Items, "Id", "Name", viewModel.SelectedFieldTypeId);
        }

        public CustomFieldModel GenerateDeleteViewModel(int id)
        {
            var model = CustomFieldRepository.Items.SingleOrDefault(c => c.Id == id);
            if (model == null)
            {
                throw new EntityNotFoundException("Specified custom field does not exist");
            }
            CustomFieldModel viewModel = CreateFieldModel();
            viewModel.CopyFrom(model);
            return viewModel;
        }

        public void Delete(int id)
        {
            var customFieldToDelete = CustomFieldRepository.Items.SingleOrDefault(c => c.Id == id);
            if (customFieldToDelete == null)
            {
                throw new EntityNotFoundException("Specified custom field does not exist");
            }
            if (CustomFieldValueRepository.Items.Any(v => v.CustomFieldId == id))
            {
                throw new ValidationException(new ValidationResult("Custom field has associated values and cannot be deleted"), null, id);
            }
            CustomFieldRepository.Remove(customFieldToDelete);
            RepositoryContainer.Save();
        }

        public void Validate(CustomFieldModel viewModel)
        {
            if (CustomFieldRepository.Items.Any(f => f.Name == viewModel.FieldName && f.Id != viewModel.Id))
            {
                throw new ValidationException(new ValidationResult("Name already exists", new string[] { "FieldName" }), null, viewModel.FieldName);
            }
        }

        public UploadWizardModel GenerateMapFieldsViewModel(UploadWizardFileViewModel model, Type t, EducationSecurityPrincipal user)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            var blobAddress = DateTime.Now.Ticks + model.File.FileName;
            UploadFileToBlobStorage(blobAddress, model.File.InputStream);
            model.BlobAddress = blobAddress;
            model.CustomFields = CreateCustomFieldSelectViewModelsToMap(model.NumberOfFields, t, user);
            return model;
        }

        private List<CustomFieldSelectModel> CreateCustomFieldSelectViewModelsToMap(int numberOfFieldsToMap, Type t, EducationSecurityPrincipal user)
        {
            List<CustomFieldSelectModel> fieldsToMap = new List<CustomFieldSelectModel>();
            var selectList = new SelectList(LoadCustomFieldList(t, user), "Id", "Name");
            for (int i = 0; i < numberOfFieldsToMap; i++)
            {
                fieldsToMap.Add(new CustomFieldSelectModel
                {
                    Name = "Field " + (i + 1),
                    CustomFields = selectList
                });
            }
            return fieldsToMap;
        }

        private void UploadFileToBlobStorage(string blobAddress, Stream file)
        {
            var writer = new DataFileWriter();
            writer.SetContentStream(file);
            writer.Write(DataFileBlobContainer, blobAddress);
        }

        public UploadWizardCompleteModel GenerateUploadWizardCompleteViewModel(EducationSecurityPrincipal user, UploadWizardModel model)
        {
            int studentIdColumn = model.CustomFields.Select((v, i) => new { ViewModel = v, Index = i }).First(c => c.ViewModel.SelectedCustomFieldId == 0).Index;
            using (var stream = new MemoryStream())
            {
                DataFileBlobContainer.DownloadToStream(model.BlobAddress, stream);
                return ProcessDataFile(user, model, studentIdColumn, stream);
            }
        }

        private UploadWizardCompleteModel ProcessDataFile(EducationSecurityPrincipal user, UploadWizardModel model, int studentIdColumn, Stream stream)
        {
            var dataTable = DataFileParser.ExtractValues(stream, '\t', model.NumberOfFields, model.FieldNameRow, model.FirstDataRow, model.LastDataRow);
            var completeModel = CheckUploadErrors(user, model, dataTable);
            if (completeModel.RowErrors.Count() > 0)
            {
                return completeModel;
            }
            CustomDataOrigin origin = CreateCustomDataOrigin(user, model);
            Dictionary<int, CustomField> customFieldsDicitonary = CreateCustomFieldsDictionary(model);
            ProcessRows(user, studentIdColumn, dataTable, completeModel, origin, customFieldsDicitonary);
            RepositoryContainer.Save();
            HandleRowErrors(user, completeModel, customFieldsDicitonary);
            return completeModel;
        }

        private void ProcessRows(EducationSecurityPrincipal user, int studentIdColumn, DataTable dataTable, UploadWizardCompleteModel completeModel, CustomDataOrigin origin, Dictionary<int, CustomField> customFieldsDicitonary)
        {
            int numColumns = dataTable.Columns.Count;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                if (row.HasErrors)
                {
                    ProcessError(row, string.Format(CultureInfo.CurrentCulture, "Row {0} failed to process.  {1}", i + 2, row.RowError), completeModel);
                }
                else
                {
                    var studentId = row[studentIdColumn].ToString();
                    var student = StudentRepository.Items.Include("StudentAssignedOfferings.ServiceOffering.Provider").
                                                          SingleOrDefault(s => s.StudentSISId == studentId);
                    if (student == null || !PermissionFactory.Current.Create("ProcessDataFile", student).TryGrantAccess(user))
                    {
                        ProcessError(row, string.Format(CultureInfo.CurrentCulture, "You do not have permission to interact with the referenced student on row {0}", i + 2), completeModel);
                    }
                    else
                    {
                        AttemptRowReadOfUploadWizardFile(studentIdColumn, completeModel, origin, customFieldsDicitonary, numColumns, i, row, student);
                    }
                }
                completeModel.ProcessedRowCount++;
            }
        }

        private UploadWizardCompleteModel CheckUploadErrors(EducationSecurityPrincipal user, UploadWizardModel model, DataTable dataTable)
        {
            UploadWizardCompleteModel completeModel = new UploadWizardCompleteModel();
            if (dataTable.Columns.Count != model.CustomFields.Count())
            {
                completeModel.RowErrors.Add("There is a different amount of columns in the file than listed. Please re-submit the file and try again.");
                completeModel.ProcessedRowCount = completeModel.SuccessfulRowsCount = 0;
                return completeModel;
            }
            foreach (var field in model.CustomFields)
            {
                if (field.SelectedCustomFieldId != 0)
                {
                    IPermission customFieldPermission = PermissionFactory.Current.Create("UploadCustomFieldData", CustomFieldRepository.Items.Single(c => c.Id == field.SelectedCustomFieldId));
                    if (!customFieldPermission.TryGrantAccess(user))
                    {
                        completeModel.RowErrors.Add("You don't have access to one or more of the selected custom fields. Re-submit and try again.");
                        completeModel.ProcessedRowCount = completeModel.SuccessfulRowsCount = 0;
                        return completeModel;
                    }
                }
            }
            return completeModel;
        }

        private CustomDataOrigin CreateCustomDataOrigin(EducationSecurityPrincipal user, UploadWizardModel uploadModel)
        {
            var origin = new CustomDataOrigin
            {
                CreatingUser = user.Identity.User,
                FileName = uploadModel.BlobAddress,
                AzureBlobKey = uploadModel.BlobAddress,
                Source = uploadModel.Source,
                WasManualEntry = false
            };
            CustomDataOriginRepository.Add(origin);
            return origin;
        }

        private void HandleRowErrors(EducationSecurityPrincipal user, UploadWizardCompleteModel model, Dictionary<int, CustomField> customFieldsDicitonary)
        {
            if (model.RowErrors.Count > 0)
            {
                var headers = string.Empty;
                foreach (var field in customFieldsDicitonary)
                {
                    headers += (field.Value != null) ? field.Value.Name + '\t' : "Student Id\t";
                }
                if (headers.Length > 0)
                {
                    headers = headers.Remove(headers.Length - 1);
                }
                model.RowErrorValues.Insert(0, headers);
                CreateErrorDownloadFile(user, model);
            }
        }

        public DownloadFileModel RetrieveUploadErrorsFile(string blobAddress)
        {
            var memoryStream = new MemoryStream();
            var downloadFile = new DownloadFileModel
            {
                FileName = blobAddress,
                FileContentStream = memoryStream
            };
            DataFileBlobContainer.DownloadToStream(blobAddress, memoryStream);
            downloadFile.FileContentStream.Position = 0;
            return downloadFile;
        }

        private void CreateErrorDownloadFile(EducationSecurityPrincipal user, UploadWizardCompleteModel model)
        {
            var writer = new DataFileWriter();
            model.ErrorDownloadFile = new DownloadFileModel
            {
                BlobAddress = string.Format("DataFileWizardUploadErrors-{0}-{1}.txt", user.Identity.User.DisplayName, DateTime.Now.Ticks),
                FileName = string.Format("{0}-ErrorRows-{1}.txt", user.Identity.User.DisplayName, DateTime.Now.Ticks),
            };
            writer.BuildTemplate(model.RowErrorValues);
            writer.Write(DataFileBlobContainer, model.ErrorDownloadFile.BlobAddress);
        }

        private void AttemptRowReadOfUploadWizardFile(int studentIdColumn, UploadWizardCompleteModel model, CustomDataOrigin origin, Dictionary<int, CustomField> customFieldsDicitonary, int numColumns, int rowNum, DataRow row, Student student)
        {
            var successfulRow = true;
            int nonIdColumns = 0;
            List<CustomFieldValue> rowCustomFieldValues = new List<CustomFieldValue>();
            for (int i = 0; i < numColumns; i++)
            {
                if (i != studentIdColumn)
                {
                    ProcessRowCell(model, origin, customFieldsDicitonary, rowNum, row, student, ref successfulRow, ref nonIdColumns, rowCustomFieldValues, i);
                }
            }
            if (nonIdColumns == 0)
            {
                successfulRow = false;
                ProcessError(row, string.Format(CultureInfo.CurrentCulture, "Row {0} requires an additional field to Id", rowNum + 2), model);
            }
            if (successfulRow)
            {
                model.SuccessfulRowsCount++;
                foreach (var value in rowCustomFieldValues)
                {
                    CustomFieldValueRepository.Add(value);
                }
            }
        }

        private void ProcessRowCell(UploadWizardCompleteModel model, CustomDataOrigin origin, Dictionary<int, CustomField> customFieldsDicitonary, int rowNum, DataRow row, Student student, ref bool successfulRow, ref int nonIdColumns, List<CustomFieldValue> rowCustomFieldValues, int i)
        {
            string fieldValue = row[i].ToString();
            if (!string.IsNullOrWhiteSpace(fieldValue))
            {
                nonIdColumns++;
                int integerTest;
                if (customFieldsDicitonary[i].CustomFieldType.Name.Equals("Integer") && !int.TryParse(fieldValue, out integerTest))
                {
                    successfulRow = false;
                    ProcessError(row, string.Format(CultureInfo.CurrentCulture, "Custom field {0} on row {1} is malformed", i + 1, rowNum + 2), model);
                }
                else
                {
                    DateTime dateTest;
                    if (customFieldsDicitonary[i].CustomFieldType.Name.Equals("Date") && !DateTime.TryParse(fieldValue, out dateTest))
                    {
                        successfulRow = false;
                        ProcessError(row, string.Format(CultureInfo.CurrentCulture, "Custom field {0} on row {1} is malformed", i + 1, rowNum + 2), model);
                    }
                    else
                    {
                        var value = new CustomFieldValue
                        {
                            CustomDataOriginId = origin.Id,
                            StudentId = student.Id,
                            CustomFieldId = customFieldsDicitonary[i].Id,
                            Value = fieldValue
                        };
                        rowCustomFieldValues.Add(value);
                    }
                }
            }
        }

        private Dictionary<int, CustomField> CreateCustomFieldsDictionary(UploadWizardModel uploadModel)
        {
            var customFieldsDicitonary = new Dictionary<int, CustomField>();
            for (int i = 0; i < uploadModel.CustomFields.Count; i++)
            {
                if (uploadModel.CustomFields[i].SelectedCustomFieldId == 0)
                {
                    customFieldsDicitonary.Add(i, null);
                }
                else
                {
                    var selectedId = (int)uploadModel.CustomFields[i].SelectedCustomFieldId;
                    var value = CustomFieldRepository.Items.Include(c => c.CustomFieldType).Where(c => c.Id == selectedId).SingleOrDefault();
                    customFieldsDicitonary.Add(i, value);
                }
            }
            return customFieldsDicitonary;
        }

        private void ProcessError(DataRow row, string rowError, UploadWizardCompleteModel model)
        {
            var rowValue = string.Empty;
            for (int i = 0; i < row.ItemArray.Count(); i++)
            {
                rowValue += row[i].ToString() + '\t';
            }
            if (rowValue.Length > 0)
            {
                rowValue = rowValue.Remove(rowValue.Length - 1);
            }
            model.RowErrorValues.Add(rowValue);
            model.RowErrors.Add(rowError);
        }

        private List<CustomField> LoadCustomFieldList(Type t, EducationSecurityPrincipal user)
        {
            List<CustomField> customFields = new List<CustomField>();
            var studentIdField = new PublicField
            {
                Name = "Student Id"
            };
            MethodInfo generic = typeof(Queryable).GetMethod("OfType").MakeGenericMethod(new Type[] { t });
            var result = (IEnumerable<object>) generic.Invoke(null, new object[] { CustomFieldRepository.Items.OrderBy(c => c.Name) });
            var fields = result.OfType<CustomField>().ToList();
            foreach (var field in fields)
            {
                IPermission permission = PermissionFactory.Current.Create("UploadCustomFieldData", field);
                if (permission.TryGrantAccess(user))
                {
                    customFields.Add(field);
                }
            }
            customFields.Insert(0, studentIdField);
            return customFields;
        }

        public IEnumerable<Student> RetrieveStudentsList(StudentProfileExportModel model)
        {
            IQueryable<Student> students = StudentRepository.Items.
                                                             Include(s => s.School).
                                                             Include(s => s.ApprovedProviders).
                                                             Include("CustomFieldValues.CustomDataOrigin").
                                                             Include("StudentAssignedOfferings.ServiceOffering.ServiceType").
                                                             Include("StudentAssignedOfferings.ServiceOffering.Provider").
                                                             Include("StudentAssignedOfferings.ServiceOffering.Program");
            return students.Where(st => (!model.SelectedSchoolIds.Any() || model.SelectedSchoolIds.Contains(st.SchoolId))
                                        &&
                                        (!model.SelectedGrades.Any() || model.SelectedGrades.Contains(st.Grade)));
        }

        public void CheckStudentCount(StudentProfileExportModel model, int studentCountLimit)
        {
            if (StudentRepository.Items.Where(st => (model.SelectedSchoolIds.Count() == 0 ||
                                                     model.SelectedSchoolIds.Contains(st.SchoolId))
                                                 && (model.SelectedGrades.Count() == 0 ||
                                                     model.SelectedGrades.Contains(st.Grade))).Count() > studentCountLimit)
            {
                throw new ArgumentOutOfRangeException("Result set to be returned will be higher than the file size limits will allow.");
            }
        }

        public Stream GenerateStudentProfileExport(EducationSecurityPrincipal user, StudentProfileExportModel model, string templatePath)
        {
            var students = RetrieveStudentsList(model);
            var configuration = new StudentProfileExportFieldDescriptor
            {
                BirthDateIncluded = model.BirthDateIncluded,
                ParentNameIncluded = model.ParentNameIncluded,
                SelectedServiceTypes = ServiceTypeRepository.Items.Where(t => model.SelectedServiceTypeIds.Contains(t.Id)).ToList()
            };
            List<CustomField> displayFields = new List<CustomField>();
            foreach (var field in CustomFieldRepository.Items.Where(f => model.SelectedCustomFieldIds.Contains(f.Id)))
            {
                IPermission permission = PermissionFactory.Current.Create("StudentProfileExportCustomFieldData", field);
                if (permission.TryGrantAccess(user))
                {
                    displayFields.Add(field);
                }
            }
            configuration.SelectedCustomFields = displayFields;
            byte[] templateData = File.ReadAllBytes(templatePath);
            MemoryStream stream = new MemoryStream();
            stream.Write(templateData, 0, (int)templateData.Length);
            using (IExportFile export = ExportFileFactory.Current.Create(typeof(StudentProfileExportFile)))
            {
                var mapper = export.GenerateMapper();
                List<IEnumerable<object>> data = new List<IEnumerable<object>>();
                foreach (var student in students)
                {
                    data.Add(mapper.MapData(configuration, student, user, Auditor));
                }
                export.Create(stream);
                export.SetupColumnHeaders(mapper.MapColumnHeadings(configuration));
                export.FillData(data);
                export.SetupFooter(CloudConfigurationManager.GetSetting("StudentProfileExportFooter"));
                RepositoryContainer.Save();
            }
            return stream;
        }

        public CustomFieldSelectorModel GenerateSelectorViewModel(EducationSecurityPrincipal user)
        {
            CustomFieldSelectorModel viewModel = new CustomFieldSelectorModel();
            List<CustomField> displayItems = new List<CustomField>();
            foreach (var field in CustomFieldRepository.Items)
            {
                IPermission permission = PermissionFactory.Current.Create("StudentProfileExportCustomFieldData", field);
                if (permission.TryGrantAccess(user))
                {
                    displayItems.Add(field);
                }
            }
            var items = displayItems.Select(f => new { Id = f.Id, Name = (f is PrivateHealthField) ? f.Name + " *" : f.Name }).OrderBy(f => f.Name);
            viewModel.CustomFields = new MultiSelectList(items, "Id", "Name");
            return viewModel;
        }

        protected abstract CustomFieldModel CreateFieldModel();

        protected abstract CustomField CreateFieldEntity();
    }
}
