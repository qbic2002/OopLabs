using System;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    [TestFixture]
    public class Tests
    {
        private IIsuService _isuService;
        private GroupValidator _groupValidator;

        [SetUp]
        public void Setup()
        {
            _groupValidator = new GroupValidator('M',3,4,30,2,3,4,30);
            _isuService = new Service(_groupValidator);
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            Group m3201 = _isuService.AddGroup("M3201");
            Student student = _isuService.AddStudent(m3201, "testStudent");
            Assert.AreEqual(m3201, student.Group);
            Assert.Contains(student, m3201.StudentsInGroup);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Group m3201 = _isuService.AddGroup("M3201");
            for (int i = 0; i < 30; i++)
            {
                _isuService.AddStudent(m3201, i.ToString());
            }
            
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddStudent(m3201, "Extra");
            });
        }

        [TestCase("lkmwmflkwmfk")]
        [TestCase("MMMMM")]
        [TestCase("M2201")]
        [TestCase(("M3501"))]
        [TestCase("M3231")]
        public void CreateGroupWithInvalidName_ThrowException(string name)
        {
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddGroup(name);
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            Group firstGroup = _isuService.AddGroup("M3201");
            Group secondGroup = _isuService.AddGroup("M3202");
            Student student = _isuService.AddStudent(firstGroup, "TestStudent");
            _isuService.ChangeStudentGroup(student, secondGroup);
            if (firstGroup.StudentsInGroup.Contains(student))
                Assert.Fail("First group contains the student");
            Assert.Contains(student, secondGroup.StudentsInGroup);
            Assert.AreEqual(secondGroup, student.Group);
        }
        // My tests
        [Test]
        public void CreateGroupsWithSameName_ThrowEcxeption()
        {
            _isuService.AddGroup("M3201");
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddGroup("M3201");
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_NewGroupHasLimitOfStudents_ThrowExceptionAndGroupHasNotChanged()
        {
            Group firstGroup = _isuService.AddGroup("M3201");
            Group secondGroup = _isuService.AddGroup("M3202");
            Student student = _isuService.AddStudent(firstGroup, "TestStudent");
            for (int i = 0; i < 30; i++)
            {
                _isuService.AddStudent(secondGroup, i.ToString());
            }
            
            Assert.Catch<IsuException>(() =>
            {
                _isuService.ChangeStudentGroup(student, secondGroup);
            });
            Assert.AreEqual(firstGroup, student.Group);
            Assert.Contains(student, firstGroup.StudentsInGroup);
        }
    }
}