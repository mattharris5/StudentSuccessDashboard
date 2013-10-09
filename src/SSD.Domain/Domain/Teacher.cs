using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Teacher
    {
        public int Id { get; internal set; }

        [StringLength(68)]
        public string TeacherKey { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(36)]
        public string Number { get; set; }

        [Phone]
        [StringLength(15)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
