using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    public class AgreementManager : IAgreementManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IEulaAgreementRepository EulaAgreementRepository { get; set; }
        private IUserRepository UserRepository { get; set; }

        public AgreementManager(IRepositoryContainer repositories)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            RepositoryContainer = repositories;
            EulaAgreementRepository = repositories.Obtain<IEulaAgreementRepository>();
            UserRepository = repositories.Obtain<IUserRepository>();
        }

        public EulaModel GenerateEulaAdminModel()
        {
            EulaModel model = new EulaModel();
            var eula = FindLatestAgreement();
            model.CopyFrom(eula);
            return model;
        }

        public EulaModel GeneratePromptViewModel()
        {
            return GenerateEulaAdminModel();
        }

        private EulaAgreement FindLatestAgreement()
        {
            return EulaAgreementRepository.Items.Include(e => e.CreatingUser).OrderByDescending(e => e.CreateTime).First();
        }

        public void Create(EulaModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (!user.IsInRole(SecurityRoles.DataAdmin))
            {
                throw new EntityAccessUnauthorizedException("user");
            }
            EulaAgreement agreement = new EulaAgreement
            {
                CreateTime = DateTime.Now,
                CreatingUser = user.Identity.User
            };
            viewModel.CopyTo(agreement);
            EulaAgreementRepository.Add(agreement);
            EulaAcceptance acceptance = new EulaAcceptance
            {
                EulaAgreement = agreement,
                CreatingUserId = user.Identity.User.Id,
                CreateTime = DateTime.Now
            };
            if (user.Identity.User.EulaAcceptances == null)
            {
                user.Identity.User.EulaAcceptances = new List<EulaAcceptance>();
            }
            user.Identity.User.EulaAcceptances.Add(acceptance);
            UserRepository.Update(user.Identity.User);
            RepositoryContainer.Save();
        }

        public void Log(EulaModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            EulaAcceptance acceptance = new EulaAcceptance
            {
                EulaAgreementId = viewModel.Id,
                CreatingUserId = user.Identity.User.Id,
                CreateTime = DateTime.Now
            };
            user.Identity.User.EulaAcceptances.Add(acceptance);
            UserRepository.Update(user.Identity.User);
            RepositoryContainer.Save();
        }

        public EulaModel GenerateEulaModelByUser(int userId)
        {
            User user = UserRepository.Items.Include("EulaAcceptances.EulaAgreement.CreatingUser").SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new EntityNotFoundException("user");
            }
            EulaAcceptance acceptance = user.EulaAcceptances.OrderByDescending(e => e.CreateTime).FirstOrDefault();
            if (acceptance == null)
            {
                throw new EntityNotFoundException("acceptance");
            }
            EulaModel model = new EulaModel();
            model.CopyFrom(acceptance.EulaAgreement);
            return model;
        }
    }
}
