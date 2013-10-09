using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SSD.Domain
{
    public class UserAccessChangeEvent : IAuditCreate
    {
        public const string AccessXmlRootElement = "access";

        public UserAccessChangeEvent()
        {
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public bool UserActive { get; set; }

        public string AccessData { get; set; }

        [NotMapped]
        public XElement AccessXml
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AccessData))
                {
                    return new XElement(AccessXmlRootElement);
                }
                return XElement.Parse(AccessData);
            }
            set
            {
                string data = null;
                if (value != null)
                {
                    data = value.ToString();
                }
                AccessData = data;
            }
        }
    }
}
