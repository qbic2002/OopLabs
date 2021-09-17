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

        [SetUp]
        public void Setup()
        {
            _isuService = new Service('M',3,4,99,2,3,4,30);
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            try
            {
                Group m3201 = _isuService.AddGroup("M3201");
                Student student = _isuService.AddStudent(m3201, "testStudent");
                if (student.Group != m3201)
                    Assert.Fail();
                if (!m3201.StudentsInGroup.Contains(student))
                    Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
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
            if (!secondGroup.StudentsInGroup.Contains(student))
                Assert.Fail("New group does not contain the student");
            if (student.Group != secondGroup)
                Assert.Fail("Wrong student's group");
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
            if (student.Group != firstGroup)
                Assert.Fail();
            if (!firstGroup.StudentsInGroup.Contains(student))
                Assert.Fail();
        }
    }
}