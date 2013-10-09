using System;

namespace SSD.Domain
{
    public class LoginEvent : IAuditCreate
    {
        public LoginEvent()
        {
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        public DateTime CreateTime { get; internal set; }

        public User CreatingUser { get; set; }
        public int CreatingUserId { get; set; }
    }
}
