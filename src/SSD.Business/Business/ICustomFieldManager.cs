using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSD.Business
{
    public interface ICustomFieldManager
    {
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<CustomField> dataTable);
        CustomFieldModel GenerateCreateViewModel();
        CustomFieldModel GenerateEditViewModel(int id, EducationSecurityPrincipal user);
        void PopulateViewModel(CustomFieldModel viewModel);
        void Edit(CustomFieldModel viewModel, EducationSecurityPrincipal user);
        void Create(CustomFieldModel viewModel, EducationSecurityPrincipal user);
        CustomFieldModel GenerateDeleteViewModel(int id);
        void Delete(int id);
        void Validate(CustomFieldModel viewModel);
        UploadWizardModel GenerateMapFieldsViewModel(UploadWizardFileViewModel model,Type t, EducationSecurityPrincipal user);
        UploadWizardCompleteModel GenerateUploadWizardCompleteViewModel(EducationSecurityPrincipal user, UploadWizardModel model);
        DownloadFileModel RetrieveUploadErrorsFile(string id);
        IEnumerable<Student> RetrieveStudentsList(StudentProfileExportModel model);
        void CheckStudentCount(StudentProfileExportModel model, int studentCountLimit);
        Stream GenerateStudentProfileExport(EducationSecurityPrincipal user, StudentProfileExportModel model, string templatePath);
        CustomFieldSelectorModel GenerateSelectorViewModel(EducationSecurityPrincipal user);
    }
}
