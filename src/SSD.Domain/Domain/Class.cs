using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Class
    {
        public Class()
        {
            Students = new List<Student>();
        }

        public int Id { get; internal set; }

        [StringLength(68)]
        public string ClassKey { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Number { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<Student> Students { get; set; }
    }
}
