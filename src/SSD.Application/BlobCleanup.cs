using SSD.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace SSD
{
    public class BlobCleanup
    {
        private bool StopRequested { get; set; }
        private Thread ProcessingThread { get; set; }
        private TimeSpan PollInterval { get; set; }
        private IReadOnlyCollection<string> ContainersToClean { get; set; }

        public BlobCleanup(TimeSpan pollInterval)
        {
            PollInterval = pollInterval;
        }

        private void Cleanup()
        {
            IBlobClient cleanupClient = AzureBlobClientFactory.Create();
            foreach (string containerName in ContainersToClean)
            {
                IBlobContainer container = cleanupClient.CreateContainer(containerName);
                CleanupContainer(container);
            }
        }

        private static void CleanupContainer(IBlobContainer container)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Cleaning up container '{0}' of all aged blobs.", container.Name), "Information");
            try
            {
                container.DeleteAged(DateTime.UtcNow.AddDays(-1 * 2));
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Cleaned container '{0}' successfully.", container.Name), "Information");
            }
            catch (BlobException e)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Failed cleaning container '{0}' due to error:\r\n{1}", container.Name, e.ToString()), "Warning");
            }
        }

        public bool IsRunning
        {
            get
            {
                if (ProcessingThread == null)
                {
                    return false;
                }
                System.Threading.ThreadState currentState = ProcessingThread.ThreadState;
                return currentState == System.Threading.ThreadState.Running || currentState == System.Threading.ThreadState.WaitSleepJoin;
            }
        }

        public void Run(IReadOnlyCollection<string> containersToClean)
        {
            if (containersToClean == null)
            {
                throw new ArgumentNullException("containersToClean");
            }
            if (IsRunning)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} is already running.", GetType().Name));
            }
            ContainersToClean = containersToClean;
            StartProcessing();
        }

        private void StartProcessing()
        {
            ProcessingThread = new Thread(new ThreadStart(() =>
            {
                while (true && !StopRequested)
                {
                    Trace.WriteLine("Cleaning up blob containers.", "Information");
                    Cleanup();
                    if (!StopRequested)
                    {
                        Thread.Sleep(PollInterval);
                    }
                }
            }));
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}.{1} starting processing with '{2}' poll interval.", GetType().Name, MethodBase.GetCurrentMethod().Name, PollInterval.ToString()), "Information");
            ProcessingThread.Start();
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} is already stopped.", GetType().Name));
            }
            StopRequested = true;
            ProcessingThread.Join();
            StopRequested = false;
        }
    }
}
