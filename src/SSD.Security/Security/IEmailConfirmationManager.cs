using SSD.Domain;
using System;

namespace SSD.Security
{
    public interface IEmailConfirmationManager
    {
        void Process(User user);
        void Request(User user, Uri confirmationEndpoint);
    }
}
