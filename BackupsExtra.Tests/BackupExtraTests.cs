using System;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.Services;
using BackupsExtra.Entities;
using BackupsExtra.Services;
using BackupsExtra.Tools;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    [TestFixture]
    public class BackupExtraTests
    {
        private BackupManager _backupManager;
        private BackupExtraManager _backupExtraManager;
        private IRepository _localRepository;

        [SetUp]
        public void Setup()
        {
            _backupManager = new BackupManager(@".\Test");
            _backupExtraManager = new BackupExtraManager(_backupManager);
            if (!Directory.Exists(@"./Test"))
                Directory.CreateDirectory(@"./Test");
            if (Directory.Exists(@"./Test/TestJob"))
                Directory.Delete(@"./Test/TestJob", true);
            _localRepository = _backupManager.AddLocalRepository("TestJob");
        }
        
        [Test]
        //[Ignore("Problem with path")]
        public void UsingCountDeleteRemoveAlgorithm()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupExtraManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@".\Test\File2.txt");
            
            BackupJob job = _backupExtraManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), new CountDelete(3), jobObject1, jobObject2);
            _backupExtraManager.CreateRestorePoint(job, DateTime.Now);
            _backupExtraManager.CreateRestorePoint(job, DateTime.Now);
            for (int i = 0; i < 20; i++)
            {
                _backupExtraManager.CreateRestorePoint(job, DateTime.Now);
                Assert.AreEqual(3, job.Backup.RestorePoints.Count);
            }
        }
        
        [Test]
        //[Ignore("Problem with path")]
        public void UsingDateDeleteRemoveAlgorithm()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupExtraManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@".\Test\File2.txt");
            
            BackupJob job = _backupExtraManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), new DateDelete(DateTime.Now), jobObject1, jobObject2);
            _backupExtraManager.CreateRestorePoint(job, DateTime.Now);
            _backupExtraManager.CreateRestorePoint(job, DateTime.Now);
            for (int i = 0; i < 20; i++)
            {
                _backupExtraManager.CreateRestorePoint(job, DateTime.Today - new TimeSpan(1,0,0,0)); 
                Assert.AreEqual(3, job.Backup.RestorePoints.Count);
            }
        }

        [Test]
        //[Ignore("Problem with path")]
        public void UsingDateDeleteRemoveAlgorithm_AllRestorePointsAreGoingToDelete_ThrowsException()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();

            JobObject jobObject1 = _backupExtraManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@".\Test\File2.txt");

            Assert.Catch<BackupsExtraException>(() =>
            {
                BackupJob job = _backupExtraManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), new DateDelete(DateTime.Today + new TimeSpan(1,0, 0, 0)), jobObject1, jobObject2);
            });
        }
    }
}