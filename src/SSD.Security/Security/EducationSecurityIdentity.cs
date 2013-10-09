using SSD.Domain;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SSD.Security
{
    public sealed class EducationSecurityIdentity : IIdentity
    {
        public EducationSecurityIdentity(ClaimsIdentity baseIdentity, User userEntity)
        {
            if (baseIdentity == null)
            {
                throw new ArgumentNullException("baseIdentity");
            }
            if (userEntity == null)
            {
                throw new ArgumentNullException("userEntity");
            }
            BaseIdentity = baseIdentity;
            UserEntity = userEntity;
        }

        private ClaimsIdentity BaseIdentity { get; set; }
        public User UserEntity { get; private set; }
        public  User User
        {
            get { return UserEntity as User; }
        }

        public string AuthenticationType
        {
            get { return BaseIdentity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return BaseIdentity.IsAuthenticated; }
        }

        public string Name
        {
            get { return BaseIdentity.Name ?? UserEntity.DisplayName ?? string.Empty; }
        }

        public static string FindUserKey(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null)
            {
                throw new ArgumentNullException("claimsIdentity");
            }
            Claim nameIdentifierClaim = claimsIdentity.FindAll(ClaimTypes.NameIdentifier).SingleOrDefault();
            if (nameIdentifierClaim == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Claim type '{0}' was not found.", ClaimTypes.NameIdentifier));
            }
            return nameIdentifierClaim.Value;
        }
    }
}
