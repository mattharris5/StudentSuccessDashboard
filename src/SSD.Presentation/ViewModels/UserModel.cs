using SSD.DataAnnotations;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.ViewModels
{
    public class UserModel : IStateCopier<User>
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string UserKey { get; set; }

        [Required]
        [DoesNotEqual(User.AnonymousValue)]
        public string FirstName { get; set; }

        [Required]
        [DoesNotEqual(User.AnonymousValue)]
        public string LastName { get; set; }

        [Required]
        [DoesNotEqual(User.AnonymousValue)]
        public string DisplayName { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        [DoesNotEqual(User.AnonymousValue)]
        public string EmailAddress { get; set; }

        [EmailAddress]
        [StringLength(255)]
        [DoesNotEqual(User.AnonymousEmailValue)]
        public string PendingEmail { get; set; }
        
        public string EndUserMessage { get; set; }

        public DateTime? EulaAcceptanceTime { get; set; }

        public IEnumerable<string> StatusFilterList { get; set; }

        public IEnumerable<string> RoleFilterList { get; set; }

        public IEnumerable<string> SchoolFilterList { get; set; }

        public void CopyTo(User model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.FirstName = FirstName;
            model.LastName = LastName;
            model.DisplayName = DisplayName;
            if (string.IsNullOrWhiteSpace(PendingEmail))
            {
                model.PendingEmail = null;
                model.ConfirmationGuid = Guid.Empty;
            }
            else if (model.PendingEmail != PendingEmail)
            {
                model.PendingEmail = PendingEmail;
                model.ConfirmationGuid = Guid.NewGuid();
            }
            model.UserKey = UserKey;
        }

        public void CopyFrom(User model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            FirstName = model.FirstName;
            LastName = model.LastName;
            DisplayName = model.DisplayName;
            EmailAddress = model.EmailAddress;
            PendingEmail = model.PendingEmail == null ? model.EmailAddress : model.PendingEmail;
            Id = model.Id;
            UserKey = model.UserKey;
            var acceptances = (model as User).EulaAcceptances;
            if (acceptances.Any())
            {
                EulaAcceptanceTime = acceptances.OrderByDescending(e => e.CreateTime).First().CreateTime;
            }
        }
    }
}
