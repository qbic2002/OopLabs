using System;
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
        private IRepository _localRepository;

        [SetUp]
        public void Setup()
        {
            _backupManager = new BackupManager(@".\Test");
            if (!Directory.Exists(@"./Test"))
                Directory.CreateDirectory(@"./Test");
            if (Directory.Exists(@"./Test/TestJob"))
                Directory.Delete(@"./Test/TestJob", true);
            _localRepository = _backupManager.AddLocalRepository("TestJob");
        }

        [Test]
        [Ignore("Problem with path")]
        public void CreateSplitStorageBackupJob()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupManager.AddJobObject(@".\Test\File2.txt");
            
            BackupJob job = _backupManager.AddBackupJob("TestJob", _localRepository, new SplitStorage(), jobObject1, jobObject2);
            job.RemoveJobObject(jobObject1, DateTime.Now);
            Assert.AreEqual(2, job.Backup.RestorePoints.Count);
            int numberOfStorages = 0;
            job.Backup.RestorePoints.ToList().ForEach(restorePoint => numberOfStorages += restorePoint.Storages.Count);
            Assert.AreEqual(3, numberOfStorages);
        }
        
        [Test]
        [Ignore("Problem with path")]
        public void CreateSingleStorageBackupJob()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupManager.AddJobObject(@".\Test\File2.txt");
            
            BackupJob job = _backupManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), jobObject1, jobObject2);
            job.RemoveJobObject(jobObject1, DateTime.Now);
        }
    }
}