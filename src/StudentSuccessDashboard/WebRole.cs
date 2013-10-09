using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace SSD
{
    public class WebRole : RoleEntryPoint
    {
        public static string WebRoleErrorLoggingFilePath { get; private set; }

        public override void Run()
        {
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " entry point called.", "Information");
            string blobCleanupIntevalSetting = CloudConfigurationManager.GetSetting("BlobCleanupInterval");
            var cleanup = new BlobCleanup(TimeSpan.Parse(blobCleanupIntevalSetting));
            cleanup.Run(new ReadOnlyCollection<string>(new[]
            {
                CloudConfigurationManager.GetSetting("ServiceOfferingFileContainerName"),
                CloudConfigurationManager.GetSetting("CustomDataFileContainerName")
            }));
            base.Run();
        }

        public override bool OnStart()
        {
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " OnStart began.", "Information");
            RoleEnvironment.Changing += OnRoleEnvironmentChanging;
            RoleEnvironment.Changed += OnRoleEnvironmentChanged;
            RoleEnvironment.Stopping += OnRoleEnvironmentStopping;
            InitializeDiagnostics();
            try
            {
                return base.OnStart();
            }
            finally
            {
                Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " OnStart completed.", "Information");
                Trace.Flush();
            }
        }

        public override void OnStop()
        {
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " OnStop began.", "Information");
            base.OnStop();
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " OnStop completed.", "Information");
        }

        private static void InitializeDiagnostics()
        {
            DiagnosticMonitorConfiguration diagMonitorConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
            LocalResource localResource = CreateLocalResource(diagMonitorConfig);
            diagMonitorConfig.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagMonitorConfig);
            WebRoleErrorLoggingFilePath = Path.Combine(localResource.RootPath, "WorkerRoleErrors.log");
            Trace.Listeners.Add(new DiagnosticMonitorTraceListener());
        }

        private static LocalResource CreateLocalResource(DiagnosticMonitorConfiguration diagMonitorConfig)
        {
            LocalResource localResource = RoleEnvironment.GetLocalResource("CustomLogFiles");
            DirectoryConfiguration dirConfig = CreateDirectoryConfiguration(localResource);
            diagMonitorConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);
            diagMonitorConfig.Directories.DataSources.Add(dirConfig);
            return localResource;
        }

        private static DirectoryConfiguration CreateDirectoryConfiguration(LocalResource localResource)
        {
            DirectoryConfiguration dirConfig = new DirectoryConfiguration();
            dirConfig.Container = "diagnostics-customlogfiles-container";
            dirConfig.DirectoryQuotaInMB = localResource.MaximumSizeInMegabytes;
            dirConfig.Path = localResource.RootPath;
            return dirConfig;
        }

        private void OnRoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " is changing.", "Information");
        }

        private void OnRoleEnvironmentChanged(object sender, RoleEnvironmentChangedEventArgs e)
        {
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " has been changed.", "Information");
        }

        private void OnRoleEnvironmentStopping(object sender, RoleEnvironmentStoppingEventArgs e)
        {
            Trace.WriteLine(RoleEnvironment.CurrentRoleInstance.Role.Name + " is stopping...", "Information");
        }
    }
}
