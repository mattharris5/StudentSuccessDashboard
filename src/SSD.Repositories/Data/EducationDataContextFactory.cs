using Castle.MicroKernel;
using Microsoft.WindowsAzure;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;

namespace SSD.Data
{
    public static class EducationDataContextFactory
    {
        private static readonly object LockObject = new object();

        public static EducationDataContext Create(IKernel kernel)
        {
            lock (LockObject)
            {
                EducationDataContext instance = new EducationDataContext();
                IDataContextConfigurator configurator = kernel.Resolve<IDataContextConfigurator>();
                configurator.Configure(kernel, instance);
                return instance;
            }
        }
    }
}
