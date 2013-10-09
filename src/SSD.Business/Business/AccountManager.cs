using Castle.Windsor;
using SSD.Collections;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewHelpers;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace SSD.Business
{
    public class AccountManager : IAccountManager
    {
        public AccountManager(IWindsorContainer windsorContainer, IEmailConfirmationManager emailConfirmationManager, IDataTableBinder dataTableBinder, IUserAuditor auditor)
        {
            if (windsorContainer == null)
            {
                throw new ArgumentNullException("windsorContainer");
            }
            if (emailConfirmationManager == null)
            {
                throw new ArgumentNullException("emailConfirmationManager");
            }
            if (dataTableBinder == null)
            {
                throw new ArgumentNullException("dataTableBinder");
            }
            if (auditor == null)
            {
                throw new ArgumentNullException("auditor");
            }
            IRepositoryContainer repositoryContainer = windsorContainer.Resolve<IRepositoryContainer>();
            UserRepository = repositoryContainer.Obtain<IUserRepository>();
            SchoolRepository = repositoryContainer.Obtain<ISchoolRepository>();
            ProviderRepository = repositoryContainer.Obtain<IProviderRepository>();
            RoleRepository = repositoryContainer.Obtain<IRoleRepository>();
            UserRoleRepository = repositoryContainer.Obtain<IUserRoleRepository>();
            UserAccessChangeEventRepository = repositoryContainer.Obtain<IUserAccessChangeEventRepository>();
            LoginEventRepository = repositoryContainer.Obtain<ILoginEventRepository>();
            EulaAgreementRepository = repositoryContainer.Obtain<IEulaAgreementRepository>();
            RepositoryContainer = repositoryContainer;
            EmailConfirmationManager = emailConfirmationManager;
            DataTableBinder = dataTableBinder;
            Auditor = auditor;
        }

        private IRepositoryContainer RepositoryContainer { get; set; }
        private IUserRepository UserRepository { get; set; }
        private ISchoolRepository SchoolRepository { get; set; }
        private IProviderRepository ProviderRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IUserRoleRepository UserRoleRepository { get; set; }
        private IUserAccessChangeEventRepository UserAccessChangeEventRepository { get; set; }
        private ILoginEventRepository LoginEventRepository { get; set; }
        private IEulaAgreementRepository EulaAgreementRepository { get; set; }
        private IEmailConfirmationManager EmailConfirmationManager { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }
        private IUserAuditor Auditor { get; set; }

        public User EnsureUserEntity(ClaimsIdentity claimsIdentity)
        {
            string userKey = EducationSecurityIdentity.FindUserKey(claimsIdentity);
            User user = UserRepository.Items
                                            .Include(u => u.EulaAcceptances)
                                            .Include("PrivateHealthDataViewEvents.PhiValuesViewed")
                                            .Include("UserRoles.Role")
                                            .Include("UserRoles.Schools")
                                            .Include("UserRoles.Providers")
                                            .SingleOrDefault(u => u.UserKey.Equals(userKey));
            if (user == null)
            {
                var email = claimsIdentity.FindFirst(ClaimTypes.Email) == null ? User.AnonymousEmailValue : claimsIdentity.FindFirst(ClaimTypes.Email).Value;
                if (email != User.AnonymousValue)
                {
                    if (UserRepository.Items.Any(u => u.EmailAddress == email))
                    {
                        email = User.AnonymousEmailValue;
                    }
                }
                user = new User
                {
                    DisplayName = claimsIdentity.FindFirst(ClaimTypes.Name) == null ? User.AnonymousValue : claimsIdentity.FindFirst(ClaimTypes.Name).Value,
                    FirstName = User.AnonymousValue,
                    LastName = User.AnonymousValue,
                    EmailAddress = email,
                    UserKey = userKey,
                    Active = true
                };
                UserRepository.Add(user);
                RepositoryContainer.Save();
            }
            return user;
        }

        public void ValidateEulaAccepted(User userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException("userEntity");
            }
            if (userEntity.EulaAcceptances.Count == 0)
            {
                throw new LicenseAgreementException("User has not agreed to a license agreement.");
            }
            else
            {
                int latestEulaId = EulaAgreementRepository.Items.OrderByDescending(e => e.CreateTime).Select(e => e.Id).First();
                if (userEntity.EulaAcceptances.OrderByDescending(e => e.CreateTime).First().EulaAgreementId != latestEulaId)
                {
                    throw new LicenseAgreementException("User has not agreed to the latest license agreement.");
                }
            }
        }

        public UserModel GenerateUserProfileViewModel(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            UserModel viewModel = new UserModel();
            User userEntity = user.Identity.UserEntity;
            viewModel.CopyFrom(userEntity);
            return viewModel;
        }

        public void Edit(UserModel viewModel, UrlHelper helper)
        {
            User user = UserRepository.Items.Where(u => u.Id == viewModel.Id).Single();
            RequestEmailChange(viewModel, helper, user);
            UserRepository.Update(user);
            RepositoryContainer.Save();
        }

        public ConfirmEmailModel GenerateConfirmEmailViewModel(Guid confirmationIdentifier)
        {
            ConfirmEmailModel viewModel;
            User user = UserRepository.Items.SingleOrDefault(u => u.ConfirmationGuid == confirmationIdentifier);
            if (user == null || UserRepository.Items.Any(u => u.Id != user.Id && u.EmailAddress == user.PendingEmail))
            {
                return new ConfirmEmailModel { Success = false };
            }
            viewModel = new ConfirmEmailModel { Success = true, UserDisplayName = user.DisplayName, UserEmailAddress = user.PendingEmail };
            EmailConfirmationManager.Process(user);
            UserRepository.Update(user);
            RepositoryContainer.Save();
            return viewModel;
        }

        public IEnumerable<string> SearchFirstNames(string term)
        {
            return UserRepository.Items.CompletionListFor(u => new CompletionProjection { Value = u.FirstName }, term);
        }

        public IEnumerable<string> SearchLastNames(string term)
        {
            return UserRepository.Items.CompletionListFor(u => new CompletionProjection { Value = u.LastName }, term);
        }

        public IEnumerable<string> SearchEmails(string term)
        {
            return UserRepository.Items.CompletionListFor(u => new CompletionProjection { Value = u.EmailAddress }, term);
        }

        public UserRoleModel GenerateCreateViewModel(int id)
        {
            var user = UserRepository.Items.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new EntityNotFoundException("User does not exist.");
            }
            UserRoleModel viewModel = new UserRoleModel();
            viewModel.CopyFrom(user);
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public UserRoleModel GenerateEditViewModel(int id)
        {
            var user = UserRepository.Items
                                           .Include("UserRoles.Role")
                                           .Include("UserRoles.Schools")
                                           .Include("UserRoles.Providers")
                                           .SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new EntityNotFoundException("User does not exist.");
            }
            UserRoleModel viewModel = new UserRoleModel();
            viewModel.CopyFrom(user);
            viewModel.UserRoleIds = user.UserRoles.Select(u => u.Id);
            viewModel.SelectedRoles = user.UserRoles.Select(u => u.Role);
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public void Create(UserRoleModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (viewModel.PostedRoles == null)
            {
                throw new ValidationException(new ValidationResult("You must select at least 1 role", new string[] { "PostedRoles" }), null, null);
            }
            var item = UserRepository.Items.
                                            Include("UserRoles.Role").
                                            Include("UserRoles.Schools").
                                            Include("UserRoles.Providers").
                                            SingleOrDefault(u => u.Id == viewModel.UserId);
            if (item == null)
            {
                throw new EntityNotFoundException("User does not exist.");
            }
            item.Active = true;
            item.Comments = viewModel.Comments;
            AddRoles(item, viewModel.PostedRoles, viewModel, user);
            UserRepository.Update(item);
            UserAccessChangeEventRepository.Add(Auditor.CreateAccessChangeEvent(item, user.Identity.User));
            RepositoryContainer.Save();
        }

        public void Edit(UserRoleModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel.PostedRoles != null && viewModel.PostedRoles.Count() > 1)
            {
                throw new ValidationException(new ValidationResult("You can't have more than 1 role", new string[] { "PostedRoles" }), null, null);
            }
            var item = UserRepository.Items.
                                            Include("UserRoles.Role").
                                            Include("UserRoles.Schools").
                                            Include("UserRoles.Providers").
                                            SingleOrDefault(u => u.Id == viewModel.UserId);
            if (item == null)
            {
                throw new EntityNotFoundException("User does not exist.");
            }
            item.Comments = viewModel.Comments;
            if (viewModel.PostedRoles == null || !viewModel.PostedRoles.Any())
            {
                //in case someone decides to manually remove all roles associated with the user
                RemoveRoles(item, Enumerable.Empty<int>().ToList());
            }
            else
            {
                //remove any roles the user does not want anymore
                RemoveRoles(item, viewModel.PostedRoles);

                //check to see if any of the roles and schools associated need to be edited
                EditRoles(item, viewModel.PostedRoles, viewModel, user);

                //add roles that the user has selected. if it's a site coordinator we need to link the role to the proper schools
                AddRoles(item, viewModel.PostedRoles, viewModel, user);
            }
            UserRepository.Update(item);
            UserAccessChangeEventRepository.Add(Auditor.CreateAccessChangeEvent(item, user.Identity.User));
            RepositoryContainer.Save();
        }

        public void PopulateViewModel(UserRoleModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            viewModel.Schools = new MultiSelectList(SchoolRepository.Items.OrderBy(s => s.Name), "Id", "Name", viewModel.SelectedSchoolIds);
            viewModel.Providers = new MultiSelectList(ProviderRepository.Items.Where(p => p.IsActive).OrderBy(p => p.Name), "Id", "Name", viewModel.SelectedProviderIds);
            viewModel.AvailableRoles = RoleRepository.Items;
        }

        public UserModel GenerateListViewModel()
        {
            var schools = SchoolRepository.Items.Select(s => s.Name).ToList().OrderBy(s => s, new NaturalSortComparer<string>());
            var roles = RoleRepository.Items.Select(r => r.Name).ToList().OrderBy(r => r, new NaturalSortComparer<string>());
            var userViewModel = new UserModel
            {
                SchoolFilterList = schools,
                RoleFilterList = roles,
                StatusFilterList = User.AllStatuses
            };
            return userViewModel;
        }

        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<User> dataTable)
        {
            IQueryable<User> items = UserRepository.Items.
                                                          Include("UserRoles.Role").
                                                          Include("UserRoles.Schools").
                                                          Include("UserRoles.Providers").
                                                          Include(u => u.LoginEvents);
            return DataTableBinder.Bind<User>(items, dataTable, requestModel);
        }

        public IEnumerable<int> GetFilteredUserIds(IClientDataTable<User> dataTable)
        {
            IQueryable<User> items = UserRepository.Items;
            items = dataTable.ApplyFilters(items);
            items = dataTable.ApplySort(items);
            return items.Select(u => u.Id);
        }

        public UserAssociationsModel GenerateUserAssociationsViewModel(int id)
        {
            var user = UserRepository.Items.Include("UserRoles.Schools").Include("UserRoles.Providers").SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new EntityNotFoundException("User does not exist.");
            }
            var viewModel = new UserAssociationsModel();
            viewModel.UserName = string.Format(CultureInfo.CurrentCulture, "{0}, {1}", user.LastName, user.FirstName);
            viewModel.SchoolNames = user.UserRoles.SelectMany(u => u.Schools)
                                                  .Select(s => s.Name)
                                                  .Distinct()
                                                  .OrderBy(s => s, new NaturalSortComparer<string>())
                                                  .ToList();
            viewModel.ProviderNames = user.UserRoles.SelectMany(u => u.Providers)
                                                    .Select(p => p.Name)
                                                    .Distinct()
                                                    .OrderBy(s => s, new NaturalSortComparer<string>())
                                                    .ToList();
            return viewModel;
        }

        public void UpdateActiveStatus(int id, bool activeStatus, EducationSecurityPrincipal user)
        {
            var item = UserRepository.Items.
                                            Include("UserRoles.Role").
                                            Include("UserRoles.Schools").
                                            Include("UserRoles.Providers").
                                            SingleOrDefault(u => u.Id == id);
            if (item == null)
            {
                throw new EntityNotFoundException("User does not exist");
            }
            item.Active = activeStatus;
            UserRepository.Update(item);
            UserAccessChangeEventRepository.Add(Auditor.CreateAccessChangeEvent(item, user.Identity.User));
            RepositoryContainer.Save();
        }

        public void UpdateActiveStatus(IEnumerable<int> ids, bool activeStatus, EducationSecurityPrincipal user)
        {
            var items = UserRepository.Items.
                        Include("UserRoles.Role").
                        Include("UserRoles.Schools").
                        Include("UserRoles.Providers").
                        Where(u => ids.Contains(u.Id)).ToList();
            if (items.Count == 0)
            {
                throw new EntityNotFoundException("Users do not exist");
            }
            foreach (var item in items)
            {
                item.Active = activeStatus;
                UserRepository.Update(item);
                UserAccessChangeEventRepository.Add(Auditor.CreateAccessChangeEvent(item, user.Identity.User));
            }
            RepositoryContainer.Save();
        }

        public MultiUserActivationModel GenerateMultiUserActivationViewModel(IEnumerable<int> ids, bool activeStatus, string ActivationString)
        {
            MultiUserActivationModel Model = new MultiUserActivationModel();
            Model.InitializeValues(ids, activeStatus, ActivationString);
            return Model;
        }

        private void RequestEmailChange(UserModel viewModel, UrlHelper helper, User user)
        {
            string originalPendingEmail = user.PendingEmail;
            if (viewModel.PendingEmail != user.EmailAddress)
            {
                viewModel.CopyTo(user);
                string actionEndpoint = helper.AbsoluteAction("ConfirmEmail", "Account");
                Uri confirmationEndpoint = new Uri(actionEndpoint);
                EmailConfirmationManager.Request(user, confirmationEndpoint);
            }
            else
            {
                viewModel.CopyTo(user);
            }
        }

        private IEnumerable<School> GetSelectedSchools(bool allSchoolsSelected, Role role, IEnumerable<int> SelectedSchoolIds)
        {
            if (role.Name.Equals(SecurityRoles.SiteCoordinator))
            {
                if (allSchoolsSelected)
                {
                    return SchoolRepository.Items;
                }
                if (SelectedSchoolIds != null && SelectedSchoolIds.Any())
                {
                    return SchoolRepository.Items.Where(school => SelectedSchoolIds.Contains(school.Id));
                }
            }
            return Enumerable.Empty<School>();
        }

        private IEnumerable<Provider> GetSelectedProviders(Role role, IEnumerable<int> SelectedProviderIds)
        {
            if (role.Name.Equals(SecurityRoles.Provider))
            {
                if (SelectedProviderIds != null && SelectedProviderIds.Any())
                {
                    return ProviderRepository.Items.Where(provider => SelectedProviderIds.Contains(provider.Id));
                }
            }
            return Enumerable.Empty<Provider>();
        }

        private void RemoveRoles(User user, IEnumerable<int> PostedRoles)
        {
            var roleIdsToRemove = user.UserRoles.Select(u => u.RoleId).Except(PostedRoles).ToArray();
            foreach (int id in roleIdsToRemove)
            {
                var userRoleToRemove = user.UserRoles.Single(u => u.RoleId == id);
                UserRoleRepository.Remove(userRoleToRemove);
            }
        }

        private void AddRoles(User user, IEnumerable<int> PostedRoles, UserRoleModel viewModel, EducationSecurityPrincipal requestor)
        {
            var rolesToAdd = PostedRoles.Except(user.UserRoles.Select(u => u.RoleId)).ToList();
            foreach (int roleId in rolesToAdd)
            {
                var role = RoleRepository.Items.SingleOrDefault(r => r.Id == roleId);
                if (role != null)
                {
                    var schools = GetSelectedSchools(viewModel.allSchoolsSelected, role, viewModel.SelectedSchoolIds).ToList();
                    var providers = GetSelectedProviders(role, viewModel.SelectedProviderIds).ToList();
                    var newUserRole = new UserRole
                    {
                        RoleId = roleId,
                        UserId = viewModel.UserId,
                        CreatingUser = requestor.Identity.User
                    };
                    user.UserRoles.Add(newUserRole);
                    UserRoleRepository.Add(newUserRole);
                    foreach (School school in schools)
                    {
                        UserRoleRepository.AddLink(newUserRole, school);
                    }
                    foreach (Provider provider in providers)
                    {
                        UserRoleRepository.AddLink(newUserRole, provider);
                    }
                }
            }
        }

        private void EditRoles(User user, IEnumerable<int> PostedRoles, UserRoleModel viewModel, EducationSecurityPrincipal requestor)
        {
            var rolesToEdit = user.UserRoles.Select(u => u.RoleId).Intersect(PostedRoles).ToList();
            foreach (int roleId in rolesToEdit)
            {
                var role = RoleRepository.Items.Where(r => r.Id == roleId).SingleOrDefault();
                if (role != null)
                {
                    var schools = GetSelectedSchools(viewModel.allSchoolsSelected, role, viewModel.SelectedSchoolIds).ToList();
                    var providers = GetSelectedProviders(role, viewModel.SelectedProviderIds).ToList();
                    var updatedUserRole = user.UserRoles.Where(u => u.RoleId == roleId).Single();
                    var schoolsToRemove = updatedUserRole.Schools.Except(schools).ToArray();
                    foreach (School school in schoolsToRemove)
                    {
                        UserRoleRepository.DeleteLink(updatedUserRole, school);
                    }
                    var schoolsToAdd = schools.Except(updatedUserRole.Schools).ToList();
                    foreach (School school in schoolsToAdd)
                    {
                        UserRoleRepository.AddLink(updatedUserRole, school);
                    }
                    var providersToRemove = updatedUserRole.Providers.Except(providers).ToArray();
                    foreach (Provider provider in providersToRemove)
                    {
                        UserRoleRepository.DeleteLink(updatedUserRole, provider);
                    }
                    var providersToAdd = providers.Except(updatedUserRole.Providers).ToList();
                    foreach (Provider provider in providersToAdd)
                    {
                        UserRoleRepository.AddLink(updatedUserRole, provider);
                    }
                    updatedUserRole.LastModifyingUser = requestor.Identity.User;
                    updatedUserRole.LastModifyTime = DateTime.Now;
                    UserRoleRepository.Update(updatedUserRole);
                }
            }
        }

        public UserModel GenerateUserAccessChangeEventModel(int id)
        {
            UserModel model = new UserModel();
            User user = UserRepository.Items.SingleOrDefault(t => t.Id == id);
            if (user == null)
            {
                throw new EntityNotFoundException("User");
            }
            model.CopyFrom(user);
            return model;
        }

        public DataTableResultModel GenerateAuditAccessDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<UserAccessChangeEvent> dataTable)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException("requestModel");
            }
            IQueryable<UserAccessChangeEvent> items = UserAccessChangeEventRepository.Items;
            return DataTableBinder.Bind<UserAccessChangeEvent>(items, dataTable, requestModel);
        }

        public void AuditLogin(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            LoginEventRepository.Add(Auditor.CreateLoginEvent(user.Identity.User));
            RepositoryContainer.Save();
        }

        public UserModel GenerateUserLoginEventModel(int id)
        {
            return GenerateUserAccessChangeEventModel(id);
        }

        public DataTableResultModel GenerateAuditLoginDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<LoginEvent> dataTable)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException("requestModel");
            }
            IQueryable<LoginEvent> items = LoginEventRepository.Items;
            return DataTableBinder.Bind<LoginEvent>(items, dataTable, requestModel);
        }
    }
}
