using Microsoft.Linq.Translations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SSD.Domain
{
    public class ServiceOffering
    {
        private static readonly CompiledExpression<ServiceOffering, string> _NameExpression
            = DefaultTranslationOf<ServiceOffering>.Property(o => o.Name).Is(o => o.Provider.Name + " " + o.ServiceType.Name + "/" + o.Program.Name);

        public ServiceOffering()
        {
            StudentAssignedOfferings = new List<StudentAssignedOffering>();
            UsersLinkingAsFavorite = new List<User>();
        }

        public int Id { get; internal set; }

        public bool IsActive { get; set; }

        [Required]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }

        [Required]
        public int ProgramId { get; set; }
        public Program Program { get; set; }

        public ICollection<StudentAssignedOffering> StudentAssignedOfferings { get; set; }

        public ICollection<User> UsersLinkingAsFavorite { get; set; }

        public string Name
        {
            get { return _NameExpression.Evaluate(this); }
        }
    }
}
