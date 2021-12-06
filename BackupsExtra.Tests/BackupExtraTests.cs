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
            _backupExtraManager = new BackupExtraManager(_backupManager, new ConsoleLogger());
            if (!Directory.Exists(@"./Test"))
                Directory.CreateDirectory(@"./Test");
            if (Directory.Exists(@"./Test/TestJob"))
                Directory.Delete(@"./Test/TestJob", true);
            _localRepository = _backupManager.AddLocalRepository("TestJob");
        }
        
        [Test]
        [Ignore("Problem with path")]
        public void UsingCountDeleteRemoveAlgorithm()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupExtraManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@".\Test\File2.txt");
            
            ExtraBackupJob job = _backupExtraManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), new RemoveAlgorithm(true, new CountPredicate(3)), jobObject1, jobObject2);
            job.CreateRestorePoint(DateTime.Now);
            job.CreateRestorePoint(DateTime.Now);
            for (int i = 0; i < 20; i++)
            {
                job.CreateRestorePoint(DateTime.Now);
                Assert.AreEqual(3, job.Backup.RestorePoints.Count);
            }
        }
        
        [Test]
        [Ignore("Problem with path")]
        public void UsingDateDeleteRemoveAlgorithm()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();
            
            JobObject jobObject1 = _backupExtraManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@".\Test\File2.txt");
            
            ExtraBackupJob job = _backupExtraManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), new RemoveAlgorithm(true,new DatePredicate(DateTime.Today)), jobObject1, jobObject2);
            job.CreateRestorePoint(DateTime.Now);
            job.CreateRestorePoint(DateTime.Now);
            for (int i = 0; i < 20; i++)
            {
                job.CreateRestorePoint(DateTime.Today - new TimeSpan(1,0,0,0));
                Assert.AreEqual(3, job.Backup.RestorePoints.Count);
            }
        }

        [Test]
        [Ignore("Problem with path")]
        public void UsingDateDeleteRemoveAlgorithm_AllRestorePointsAreGoingToDelete_ThrowsException()
        {
            File.Create(@".\Test\File1.txt").Dispose();
            File.Create(@".\Test\File2.txt").Dispose();

            JobObject jobObject1 = _backupExtraManager.AddJobObject(@".\Test\File1.txt");
            JobObject jobObject2 = _backupExtraManager.AddJobObject(@".\Test\File2.txt");

            Assert.Catch<BackupsExtraException>(() =>
            {
                ExtraBackupJob job = _backupExtraManager.AddBackupJob("TestJob", _localRepository, new SingleStorage(), new RemoveAlgorithm(true,new DatePredicate(DateTime.Today + new TimeSpan(1,0, 0, 0))), jobObject1, jobObject2);
            });
        }

        [Test]
        [Ignore("Problem with path")]
        public void MergeTest()
        {
            RestorePoint restorePoint1 = new RestorePoint(_localRepository, 1, new SplitStorage(), DateTime.Now, new JobObject(@".\Test\File1.txt"), new JobObject(@".\Test\File2.txt"));
            RestorePoint restorePoint2 = new RestorePoint(_localRepository, 2, new SplitStorage(), DateTime.Now, new JobObject(@".\Test\File2.txt"));
            RestorePoint newRestorePoint = RestorePointManager.Merge(restorePoint1, restorePoint2);
            Assert.True(2 == newRestorePoint.JobObjects.Count);
        }
    }
}