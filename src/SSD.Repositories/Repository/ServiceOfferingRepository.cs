using SSD.Data;
using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Repository
{
    public class ServiceOfferingRepository : IServiceOfferingRepository
    {
        public ServiceOfferingRepository(IEducationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        private IEducationContext Context { get; set; }

        public void Add(ServiceOffering item)
        {
            Context.ServiceOfferings.Add(item);
        }

        public void Remove(ServiceOffering item)
        {
            throw new NotSupportedException(string.Format("{0} entities cannot be removed.  Only \"soft\" deletes are supported via IsActive.", item.GetType().Name));
        }

        public void Update(ServiceOffering item)
        {
            Context.SetModified(item);
        }

        public IQueryable<ServiceOffering> Items
        {
            get { return Context.ServiceOfferings; }
        }

        public void AddLink(ServiceOffering serviceOffering, User user)
        {
            if (serviceOffering == null)
            {
                throw new ArgumentNullException("serviceOffering");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            serviceOffering.UsersLinkingAsFavorite.Add(user);
            user.FavoriteServiceOfferings.Add(serviceOffering);
        }

        public void DeleteLink(ServiceOffering serviceOffering, User user)
        {
            if (serviceOffering == null)
            {
                throw new ArgumentNullException("serviceOffering");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            serviceOffering.UsersLinkingAsFavorite.Remove(user);
            user.FavoriteServiceOfferings.Remove(serviceOffering);
        }
    }
}
