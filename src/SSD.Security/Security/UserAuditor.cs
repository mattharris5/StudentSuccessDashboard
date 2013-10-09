using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SSD.Security
{
    public class UserAuditor : IUserAuditor
    {
        public UserAccessChangeEvent CreateAccessChangeEvent(User user, User requestor)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (requestor == null)
            {
                throw new ArgumentNullException("requestor");
            }
            UserAccessChangeEvent log = new UserAccessChangeEvent
            {
                User = user,
                UserId = user.Id,
                UserActive = user.Active,
                CreatingUser = requestor,
                CreatingUserId = requestor.Id
            };
            log.AccessXml = BuildAccessXml(user);
            return log;
        }

        public LoginEvent CreateLoginEvent(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return new LoginEvent
            {
                CreatingUser = user,
                CreatingUserId = user.Id
            };
        }

        private static XElement BuildAccessXml(User user)
        {
            IEnumerable<UserRole> userRoles = user.UserRoles;
            if (userRoles.Any())
            {
                return
                    new XElement(UserAccessChangeEvent.AccessXmlRootElement,
                        new XElement("roles", user.UserRoles.Select(ur => ur.Role).Select(r =>
                            new XElement("role", new XAttribute("id", r.Id), new XAttribute("name", r.Name)))
                        ),
                        new XElement("providers", user.UserRoles.SelectMany(ur => ur.Providers).Select(p =>
                            new XElement("provider", new XAttribute("id", p.Id), new XAttribute("name", p.Name)))
                        ),
                        new XElement("schools", user.UserRoles.SelectMany(ur => ur.Schools).Select(s =>
                            new XElement("school", new XAttribute("id", s.Id), new XAttribute("name", s.Name)))
                        )
                    );
            }
            return null;
        }

        public PrivateHealthDataViewEvent CreatePrivateHealthInfoViewEvent(User user, List<CustomFieldValue> viewedValues)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (viewedValues == null)
            {
                throw new ArgumentNullException("viewedValues");
            }
            return new PrivateHealthDataViewEvent
            {
                CreatingUser = user,
                CreatingUserId = user.Id,
                PhiValuesViewed = viewedValues
            };
        }
    }
}
