using SSD.Domain;
using System.Collections.Generic;

namespace SSD.Security
{
    public interface IUserAuditor
    {
        UserAccessChangeEvent CreateAccessChangeEvent(User user, User requestor);
        LoginEvent CreateLoginEvent(User user);
        PrivateHealthDataViewEvent CreatePrivateHealthInfoViewEvent(User user, List<CustomFieldValue> viewedValues);
    }
}
