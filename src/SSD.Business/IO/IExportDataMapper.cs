using SSD.Security;
using System.Collections.Generic;

namespace SSD.IO
{
    public interface IExportDataMapper
    {
        IEnumerable<string> MapColumnHeadings(object descriptor);
        IEnumerable<object> MapData(object descriptor, object data, EducationSecurityPrincipal user, IUserAuditor auditor);
    }
}
