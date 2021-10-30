using System;
using System.Linq;
using System.Reflection;
using Backups.Entities;
using Backups.Services;

namespace Backups
{
    internal class Program
    {
        private static void Main()
        {
            BackupManager backupManager = new BackupManager();

            JobObject jobObject1 = backupManager.AddJobObject(@"C:\Users\golov\Backups\File1.txt");
            JobObject jobObject2 = backupManager.AddJobObject(@"C:\Users\golov\Backups\File2.txt");
            JobObject jobObject3 = backupManager.AddJobObject(@"C:\Users\golov\Backups\File3.txt");

            IRepository local = backupManager.AddLocalRepository("test");
            BackupJob job = backupManager.AddBackupJob("test", local, Algorithms.SingleStorage, jobObject1, jobObject2);
            job.RemoveJobObject(jobObject1);
            job.AddJobObject(jobObject3);
        }
    }
}
