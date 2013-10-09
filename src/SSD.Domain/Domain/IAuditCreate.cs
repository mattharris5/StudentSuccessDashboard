using System;

namespace SSD.Domain
{
    public interface IAuditCreate
    {
        DateTime CreateTime { get; }
        int CreatingUserId { get; set; }
        User CreatingUser { get; set; }
    }
}
