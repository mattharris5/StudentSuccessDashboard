using System;

namespace SSD.Domain
{
    public interface IAuditModify
    {
        DateTime? LastModifyTime { get; set; }
        int? LastModifyingUserId { get; set; }
        User LastModifyingUser { get; set; }
    }
}
