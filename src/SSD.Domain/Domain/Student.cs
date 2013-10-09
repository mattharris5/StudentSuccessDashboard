using Microsoft.Linq.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Student
    {
        private static readonly CompiledExpression<Student, string> _FullNameExpression
            = DefaultTranslationOf<Student>.Property(s => s.FullName).Is(s => s.LastName + ", " + s.FirstName + " " + s.MiddleName);

        public Student()
        {
            Classes = new List<Class>();
            StudentAssignedOfferings = new List<StudentAssignedOffering>();
            ServiceRequests = new List<ServiceRequest>();
            ApprovedProviders = new List<Provider>();
            CustomFieldValues = new List<CustomFieldValue>();
        }

        public int Id { get; internal set; }

        [StringLength(68)]
        public string StudentKey { get; set; }

        [StringLength(36)]
        public string StudentSISId { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        public int Grade { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(200)]
        public string Parents { get; set; }

        public bool HasParentalOptOut { get; set; }

        [Required]
        public int SchoolId { get; set; }
        public School School { get; set; }

        public ICollection<Class> Classes { get; set; }

        public ICollection<StudentAssignedOffering> StudentAssignedOfferings { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; }

        public ICollection<Provider> ApprovedProviders { get; set; }

        public ICollection<CustomFieldValue> CustomFieldValues { get; set; }

        public string FullName
        {
            get { return _FullNameExpression.Evaluate(this); }
        }
    }
}
