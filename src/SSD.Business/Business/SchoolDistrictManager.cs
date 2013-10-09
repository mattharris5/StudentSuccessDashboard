using SSD.Collections;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Business
{
    public class SchoolDistrictManager : ISchoolDistrictManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IPropertyRepository PropertyRepository { get; set; }
        private IStudentRepository StudentRepository { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private ISubjectRepository SubjectRepository { get; set; }
        private IPriorityRepository PriorityRepository { get; set; }
        private IFulfillmentStatusRepository FulfillmentStatusRepository { get; set; }
        private IProviderRepository ProviderRepository { get; set; }
        private ISchoolRepository SchoolRepository { get; set; }
        private IPrivateHealthDataViewEventRepository PrivateHealthDataViewEventRepository { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }
        private IUserAuditor Auditor { get; set; }

        public SchoolDistrictManager(IRepositoryContainer repositories, IDataTableBinder dataTableBinder, IUserAuditor auditor)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
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
            PropertyRepository = repositories.Obtain<IPropertyRepository>();
            StudentRepository = repositories.Obtain<IStudentRepository>();
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            SubjectRepository = repositories.Obtain<ISubjectRepository>();
            PriorityRepository = repositories.Obtain<IPriorityRepository>();
            FulfillmentStatusRepository = repositories.Obtain<IFulfillmentStatusRepository>();
            ProviderRepository = repositories.Obtain<IProviderRepository>();
            SchoolRepository = repositories.Obtain<ISchoolRepository>();
            PrivateHealthDataViewEventRepository = repositories.Obtain<IPrivateHealthDataViewEventRepository>();
            DataTableBinder = dataTableBinder;
            Auditor = auditor;
        }

        public StudentListOptionsModel GenerateListOptionsViewModel(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var viewModel = new StudentListOptionsModel
            {
                IsProvider = !user.IsInRole(SecurityRoles.DataAdmin) && !user.IsInRole(SecurityRoles.SiteCoordinator) && user.IsInRole(SecurityRoles.Provider)
            };
            InitializeFilterLists(viewModel);
            return viewModel;
        }

        public StudentApprovalListOptionsModel GenerateApprovalListOptionsViewModel()
        {
            var viewModel = new StudentApprovalListOptionsModel();
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public AddStudentApprovalModel GenerateAddStudentApprovalViewModel(int id)
        {
            var viewModel = new AddStudentApprovalModel();
            viewModel.StudentId = id;
            PopulateViewModelLists(viewModel);
            return viewModel;
        }

        public IEnumerable<string> SearchFirstNames(EducationSecurityPrincipal user, string term)
        {
            return GetAllowedStudentList(user).CompletionListFor(s => new CompletionProjection { Value = s.FirstName }, term);
        }

        public IEnumerable<string> SearchLastNames(EducationSecurityPrincipal user, string term)
        {
            return GetAllowedStudentList(user).CompletionListFor(s => new CompletionProjection { Value = s.LastName }, term);
        }

        public IEnumerable<string> SearchIdentifiers(EducationSecurityPrincipal user, string term)
        {
            return GetAllowedStudentList(user).CompletionListFor(s => new CompletionProjection { Value = s.StudentSISId }, term);
        }

        private IQueryable<Student> GetAllowedStudentList(EducationSecurityPrincipal user)
        {
            return StudentRepository.GetAllowedList(user);
        }

        public StudentDetailModel GenerateStudentDetailViewModel(EducationSecurityPrincipal user, int id)
        {
            Student student = StudentRepository.Items.Include(s => s.ApprovedProviders).
                                                      Include("CustomFieldValues.CustomDataOrigin").
                                                      Include("CustomFieldValues.CustomField").
                                                      Include("Classes.Teacher").
                                                      Include("ServiceRequests.CreatingUser").
                                                      Include("ServiceRequests.ServiceType").
                                                      Include("ServiceRequests.Subject").
                                                      Include("ServiceRequests.FulfillmentDetails.FulfillmentStatus").
                                                      Include("ServiceRequests.FulfillmentDetails.CreatingUser").
                                                      Include("StudentAssignedOfferings.ServiceOffering.Provider").
                                                      Include("StudentAssignedOfferings.ServiceOffering.ServiceType").
                                                      Include("StudentAssignedOfferings.ServiceOffering.Program").
                                                      Include("StudentAssignedOfferings.CreatingUser").
                                                      Include(s => s.School).
                                                      SingleOrDefault(i => i.Id == id);
            if (student == null)
            {
                throw new EntityNotFoundException("Requested student could not be found.");
            }
            IViewStudentDetailPermission permission = (IViewStudentDetailPermission)PermissionFactory.Current.Create("ViewStudentDetail", student);
            permission.GrantAccess(user);
            StudentDetailModel viewModel = new StudentDetailModel();
            viewModel.OnlyUploadedCustomField = permission.CustomFieldOnly;
            List<CustomFieldValue> displayFields = new List<CustomFieldValue>();
            if (permission.CustomFieldOnly)
            {
                displayFields = student.CustomFieldValues.Where(c => c.CustomDataOrigin.CreatingUserId == user.Identity.User.Id).ToList();
            }
            else
            {
                foreach (var field in student.CustomFieldValues)
                {
                    IPermission fieldPermission = PermissionFactory.Current.Create("ViewStudentCustomFieldData", field.CustomField);
                    if ((field.CustomDataOrigin.CreatingUserId == user.Identity.User.Id) || (fieldPermission.TryGrantAccess(user)))
                    {
                        displayFields.Add(field);
                    }
                }
            }
            PrivateHealthDataViewEventRepository.Add(Auditor.CreatePrivateHealthInfoViewEvent(user.Identity.User, displayFields.Where(c => c.CustomField is PrivateHealthField).ToList()));
            RepositoryContainer.Save();
            student.CustomFieldValues = displayFields;
            viewModel.CopyFrom(student);
            if (permission.CustomFieldOnly)
            {
                viewModel.DateOfBirth = null;
                viewModel.Parents = null;
                viewModel.ServiceRequests = Enumerable.Empty<ServiceRequest>();
                viewModel.StudentAssignedOfferings = Enumerable.Empty<StudentAssignedOffering>();
                viewModel.Classes = Enumerable.Empty<Class>();
            }
            return viewModel;
        }

        public IEnumerable<Property> FindStudentProperties()
        {
            return PropertyRepository.Items.Where(p => p.EntityName.Equals(typeof(Student).FullName));
        }

        public DataTableResultModel GenerateApprovalDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Student> dataTable)
        {
            IQueryable<Student> items = StudentRepository.Items;
            return DataTableBinder.Bind<Student>(items, dataTable, requestModel);
        }

        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Student> dataTable)
        {
            IQueryable<Student> items = StudentRepository.Items.Include(s => s.ApprovedProviders).
                                                                Include(s => s.School).
                                                                Include("CustomFieldValues.CustomDataOrigin.CreatingUser").
                                                                Include("ServiceRequests.ServiceType").
                                                                Include("ServiceRequests.Priority").
                                                                Include("ServiceRequests.Subject").
                                                                Include("ServiceRequests.FulfillmentDetails.FulfillmentStatus").
                                                                Include("StudentAssignedOfferings.ServiceOffering.Provider").
                                                                Include("StudentAssignedOfferings.ServiceOffering.ServiceType").
                                                                Include("StudentAssignedOfferings.ServiceOffering.Program");
            return DataTableBinder.Bind<Student>(items, dataTable, requestModel);
        }

        public IEnumerable<int> GetFilteredFinderStudentIds(EducationSecurityPrincipal user, IClientDataTable<Student> dataTable)
        {
            IQueryable<Student> items = StudentRepository.Items;
            items = dataTable.ApplyFilters(items);
            items = dataTable.ApplySort(items);
            if (user.IsInRole(SecurityRoles.DataAdmin) || user.IsInRole(SecurityRoles.Provider))
            {
                return items.Select(s => s.Id);
            }
            else
            {
                //if user has any associated schools (site coordinator)
                var userSchoolIds = user.Identity.User.UserRoles.SelectMany(ur => ur.Schools).Select(s => s.Id);
                if (userSchoolIds.Any())
                {
                    return items.Where(s => userSchoolIds.Contains(s.School.Id)).Select(s => s.Id);
                }
                return Enumerable.Empty<int>().ToList();
            }
        }

        public void AddProviders(AddStudentApprovalModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            var student = StudentRepository.Items.SingleOrDefault(s => s.Id == viewModel.StudentId);
            if (student == null)
            {
                throw new EntityNotFoundException("Student with specified identifier was not found.");
            }
            var selectedProviders = ProviderRepository.Items.Where(p => viewModel.ProvidersToAdd.Contains(p.Id)).ToList();
            if (selectedProviders.Count < viewModel.ProvidersToAdd.Count())
            {
                throw new EntityNotFoundException("At least one provider selected cannot be associated with the student because it was not found using the given identifier.");
            }
            foreach (Provider provider in selectedProviders)
            {
                StudentRepository.AddLink(student, provider);
            }
            RepositoryContainer.Save();
        }

        public RemoveApprovedProviderModel GenerateRemoveProviderViewModel(int id, int providerId)
        {
            var student = StudentRepository.Items.Include(s => s.ApprovedProviders).SingleOrDefault(s => s.Id == id);
            if (student == null)
            {
                throw new EntityNotFoundException("Student with specified identifier was not found.");
            }
            var provider = student.ApprovedProviders.SingleOrDefault(p => p.Id == providerId);
            if (provider == null)
            {
                throw new EntityNotFoundException("Provider could not be found associated with specified student.");
            }
            return new RemoveApprovedProviderModel { ProviderId = providerId, ProviderName = provider.Name, StudentId = student.Id, StudentName = student.FullName };
        }

        public void RemoveProvider(RemoveApprovedProviderModel viewModel)
        {
            var student = StudentRepository.Items.Include(s => s.ApprovedProviders).SingleOrDefault(s => s.Id == viewModel.StudentId);
            if (student == null)
            {
                throw new EntityNotFoundException("Student with specified identifier was not found.");
            }
            var provider = student.ApprovedProviders.SingleOrDefault(p => p.Id == viewModel.ProviderId);
            if (provider == null)
            {
                throw new EntityNotFoundException("Provider could not be found associated with specified student.");
            }
            StudentRepository.DeleteLink(student, provider);
            RepositoryContainer.Save();
        }

        public RemoveApprovedProvidersBySchoolModel GenerateRemoveProvidersBySchoolViewModel()
        {
            RemoveApprovedProvidersBySchoolModel viewModel = new RemoveApprovedProvidersBySchoolModel();
            viewModel.Schools = new MultiSelectList(SchoolRepository.Items, "Id", "Name");
            return viewModel;
        }

        public void RemoveAllProviders()
        {
            StudentRepository.ResetApprovals();
            RepositoryContainer.Save();
        }

        public void RemoveAllProviders(IEnumerable<int> schoolIds)
        {
            StudentRepository.ResetApprovals(schoolIds);
            RepositoryContainer.Save();
        }

        public void SetStudentOptOutState(int id, bool hasParentalOptOut)
        {
            var student = StudentRepository.Items.SingleOrDefault(s => s.Id == id);
            if (student == null)
            {
                throw new EntityNotFoundException("Student with the specified identifier was not found.");
            }
            student.HasParentalOptOut = hasParentalOptOut;
            StudentRepository.Update(student);
            RepositoryContainer.Save();
        }

        public void PopulateViewModelLists(AddStudentApprovalModel viewModel)
        {
            var student = StudentRepository.Items.Include(s => s.ApprovedProviders).SingleOrDefault(s => s.Id == viewModel.StudentId);
            if (student == null)
            {
                throw new EntityNotFoundException("Student with the specified identifier was not found.");
            }
            IEnumerable<Provider> providersNotYetApproved = ProviderRepository.Items;
            foreach (Provider studentProvider in student.ApprovedProviders)
            {
                providersNotYetApproved = providersNotYetApproved.Where(p => p.Id != studentProvider.Id);
            }
            viewModel.Providers = new MultiSelectList(providersNotYetApproved, "Id", "Name");
        }

        public int CountStudentsWithApprovedProviders()
        {
            return StudentRepository.Items.Where(s => s.ApprovedProviders.Any()).Count();
        }

        public SchoolSelectorModel GenerateSchoolSelectorViewModel()
        {
            var viewModel = new SchoolSelectorModel();
            viewModel.Schools = new MultiSelectList(SchoolRepository.Items, "Id", "Name");
            return viewModel;
        }

        public GradeSelectorModel GenerateGradeSelectorViewModel()
        {
            var viewModel = new GradeSelectorModel();
            viewModel.Grades = new MultiSelectList(StudentRepository.Items.Select(g => g.Grade).Distinct());
            return viewModel;
        }

        private void InitializeFilterLists(StudentListOptionsModel viewModel)
        {
            viewModel.GradeFilterList = StudentRepository.Items.Select(g => g.Grade).Distinct().ToList();
            viewModel.SchoolFilterList = LoadSchoolNameList();
            viewModel.PriorityFilterList = PriorityRepository.Items.Select(p => p.Name).ToList();
            viewModel.RequestStatusFilterList = FulfillmentStatusRepository.Items.Select(f => f.Name).ToList();
            viewModel.ServiceTypeFilterList = ServiceTypeRepository.Items.Where(s => s.IsActive).Select(t => t.Name).ToList();
            viewModel.SubjectFilterList = SubjectRepository.Items.Select(s => s.Name).ToList();
        }

        private void PopulateViewModel(StudentApprovalListOptionsModel viewModel)
        {
            viewModel.SchoolFilterList = LoadSchoolNameList();
            viewModel.ProviderFilterList = ProviderRepository.Items.Where(p => p.IsActive).Select(p => p.Name).ToList().OrderBy(p => p);
            viewModel.TotalStudentsWithApproval = CountStudentsWithApprovedProviders();
        }

        private IEnumerable<string> LoadSchoolNameList()
        {
            var materializedSchoolList = StudentRepository.Items.Include(s => s.School).Select(s => s.School.Name).Distinct().ToList();
            return materializedSchoolList.OrderBy(s => s, new NaturalSortComparer<string>());
        }
    }
}
