using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface IProgramManager
    {
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Program> dataTable);
        void PopulateViewModelLists(ProgramModel viewModel);
        ProgramModel GenerateCreateViewModel();
        void Create(ProgramModel viewModel);
        ProgramModel GenerateEditViewModel(int id);
        void Edit(ProgramModel viewModel);
        ProgramModel GenerateDeleteViewModel(int id);
        void Delete(int id);
        IEnumerable<string> SearchProgramNames(string term);
        void Validate(ProgramModel viewModel);
    }
}
