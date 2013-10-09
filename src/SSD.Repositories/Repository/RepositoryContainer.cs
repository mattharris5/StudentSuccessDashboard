using Castle.Windsor;
using SSD.Data;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SSD.Repository
{
    public class RepositoryContainer : IRepositoryContainer
    {
        public RepositoryContainer(IWindsorContainer windsorContainer, IEducationContext context)
        {
            if (windsorContainer == null)
            {
                throw new ArgumentNullException("windsorContainer");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            WindsorContainer = windsorContainer;
            Context = context;
        }

        private IWindsorContainer WindsorContainer { get; set; }
        private IEducationContext Context { get; set; }

        public TRepository Obtain<TRepository>() where TRepository : class
        {
            TRepository repository = WindsorContainer.Resolve<TRepository>(new Dictionary<string, object> { { "context", Context } });
            if (repository == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Repository of type {0} was not registered by the {1} or could not be constructed using the {2}.", typeof(TRepository).Name, WindsorContainer.GetType().Name, Context.GetType().Name));
            }
            return repository;
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
