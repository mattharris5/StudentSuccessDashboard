using System.Collections.Generic;

namespace SSD.Security
{
    public interface ISecurityConfiguration
    {
        IEnumerable<string> AdministratorEmailAddresses { get; }
    }
}
