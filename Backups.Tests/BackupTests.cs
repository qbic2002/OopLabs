using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.Services;
using NUnit.Framework;

namespace Backups.Tests
{
    [TestFixture]
    public class BackupTests
    {
        private BackupManager _backupManager;
        private IRepository localRepository;

        [SetUp]
        public void Setup()
        {
            _backupManager = new BackupManager();
            if (!Directory.Exists(@"./Test"))
                Directory.CreateDirectory(@"./Test");
            if (Directory.Exists(@"./Test/TestJob"))
                Directory.Delete(@"./Test/TestJob", true);
            localRepository = _backupManager.AddLocalRepository("TestJob");
        }

        [Test]
        public void CreateSplitStorageBackupJob()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupManager.AddJobObject(@".\Test\File2.txt");
            
            BackupJob job = _backupManager.AddBackupJob("TestJob", localRepository, Algorithms.SplitStorage, jobObject1, jobObject2);
            job.RemoveJobObject(jobObject1);
            Assert.AreEqual(2, job.RestorePoints.Count);
            int numberOfStorages = 0;
            job.RestorePoints.ToList().ForEach(restorePoint => numberOfStorages += restorePoint.JobObjects.Count);
            Assert.AreEqual(3, numberOfStorages);
        }
    }
}