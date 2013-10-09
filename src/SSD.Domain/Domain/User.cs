using Microsoft.Linq.Translations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.Domain
{
    public class User
    {
        public const string AnonymousValue = "Anonymous";
        public const string AnonymousEmailValue = "Anonymous@sample.com";
        public const string ActiveStatus = "Active";
        public const string InactiveStatus = "Inactive";
        public const string AwaitingAssignmentStatus = "Awaiting Assignment";
        public static readonly IReadOnlyList<string> AllStatuses = new ReadOnlyCollection<string>(new string[]
        {
            InactiveStatus,
            AwaitingAssignmentStatus,
            ActiveStatus
        });
        private static readonly CompiledExpression<User, string> _StatusExpression
            = DefaultTranslationOf<User>.Property(u => u.Status).Is(u => u.Active ? u.UserRoles.Count == 0 ? AwaitingAssignmentStatus : ActiveStatus : InactiveStatus);
        private static readonly CompiledExpression<User, DateTime?> _LastLoginExpression
            = DefaultTranslationOf<User>.Property(u => u.LastLoginTime).Is(u => u.LoginEvents.OrderByDescending(e => e.CreateTime).Select(e => (DateTime?)e.CreateTime).FirstOrDefault());

        public User()
        {
            FavoriteServiceOfferings = new List<ServiceOffering>();
            UserRoles = new List<UserRole>();
            LoginEvents = new List<LoginEvent>();
            EulaAcceptances = new List<EulaAcceptance>();
            PrivateHealthDataViewEvents = new List<PrivateHealthDataViewEvent>();
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        [Required]
        [StringLength(255)]
        public string UserKey { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string EmailAddress { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string PendingEmail { get; set; }

        public Guid ConfirmationGuid { get; set; }

        public DateTime CreateTime { get; internal set; }

        public string Comments { get; set; }

        public bool Active { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<UserRole> CreatedUserRoles { get; set; }

        public ICollection<LoginEvent> LoginEvents { get; set; }

        public ICollection<EulaAcceptance> EulaAcceptances { get; set; }

        public ICollection<ServiceOffering> FavoriteServiceOfferings { get; set; }

        public ICollection<PrivateHealthDataViewEvent> PrivateHealthDataViewEvents { get; set; }

        public ICollection<ServiceAttendance> CreatedServiceAttendances { get; set; }

        public ICollection<ServiceRequestFulfillment> CreatedServiceRequestFulfillments { get; set; }

        public ICollection<UserAccessChangeEvent> UserAccessChangeEvents { get; set; }

        public ICollection<UserAccessChangeEvent> CreatedUserAccessChangeEvents { get; set; }

        public ICollection<EulaAgreement> CreatedEulaAgreements { get; set; }

        public ICollection<CustomField> CreatedCustomFields { get; set; }

        public ICollection<CustomDataOrigin> CreatedCustomDataOrigins { get; set; }

        public bool IsValidUserInformation
        {
            get
            {
                return (!string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(EmailAddress) &&
                    !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) &&
                        !DisplayName.Equals(AnonymousValue) && !EmailAddress.Equals(AnonymousEmailValue) &&
                        !FirstName.Equals(AnonymousValue) && !LastName.Equals(AnonymousValue));
            }
        }

        public string Status
        {
            get { return _StatusExpression.Evaluate(this); }
        }

        public DateTime? LastLoginTime
        {
            get { return _LastLoginExpression.Evaluate(this); }
        }
    }
}
