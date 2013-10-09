using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;

namespace SSD.Business
{
    public interface IAccountManager
    {
        User EnsureUserEntity(ClaimsIdentity claimsIdentity);
        void ValidateEulaAccepted(User userEntity);
        UserModel GenerateUserProfileViewModel(EducationSecurityPrincipal user);
        void Edit(UserModel viewModel, UrlHelper helper);
        ConfirmEmailModel GenerateConfirmEmailViewModel(Guid confirmationIdentifier);
        IEnumerable<string> SearchEmails(string term);
        IEnumerable<string> SearchFirstNames(string term);
        IEnumerable<string> SearchLastNames(string term);
        UserRoleModel GenerateCreateViewModel(int id);
        UserRoleModel GenerateEditViewModel(int id);
        void Create(UserRoleModel viewModel, EducationSecurityPrincipal user);
        void Edit(UserRoleModel viewModel, EducationSecurityPrincipal user);
        void PopulateViewModel(UserRoleModel viewModel);
        UserModel GenerateListViewModel();
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<User> dataTable);
        IEnumerable<int> GetFilteredUserIds(IClientDataTable<User> dataTable);
        UserAssociationsModel GenerateUserAssociationsViewModel(int id);
        void UpdateActiveStatus(IEnumerable<int> ids, bool activeStatus, EducationSecurityPrincipal user);
        void UpdateActiveStatus(int id, bool activeStatus, EducationSecurityPrincipal user);
        MultiUserActivationModel GenerateMultiUserActivationViewModel(IEnumerable<int> ids, bool activeStatus, string ActivationString);
        UserModel GenerateUserAccessChangeEventModel(int id);
        DataTableResultModel GenerateAuditAccessDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<UserAccessChangeEvent> dataTable);
        void AuditLogin(EducationSecurityPrincipal user);
        UserModel GenerateUserLoginEventModel(int id);
        DataTableResultModel GenerateAuditLoginDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<LoginEvent> dataTable);
    }
}
