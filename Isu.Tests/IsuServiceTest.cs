using System.Collections.Generic;
using System.Linq;
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
            _groupValidator = new GroupValidator('M', 3, 4, 30, 2, 3, 4);
            _isuService = new IsuService(_groupValidator, 30);
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

        [Test]
        public void GetStudentTest_ReturnStudent()
        {
            Group group = _isuService.AddGroup("M3201");
            _isuService.AddStudent(group, "0");
            _isuService.AddStudent(group, "1");
            Student student = _isuService.AddStudent(group, "2");
            _isuService.AddStudent(group, "3");
            Assert.AreEqual(student, _isuService.GetStudent(2));
        }

        [Test]
        public void FindStudentTest_ReturnStudent()
        {
            Group group = _isuService.AddGroup("M3201");
            _isuService.AddStudent(group, "0");
            _isuService.AddStudent(group, "1");
            Student student = _isuService.AddStudent(group, "2");
            _isuService.AddStudent(group, "3");
            Assert.AreEqual(student, _isuService.FindStudent("2"));
        }

        [Test]
        public void FindStudentsTest_FindByGroupName_ReturnStudents()
        {
            Group group = _isuService.AddGroup("M3201");
            Group otherGroup = _isuService.AddGroup("M3202");
            var expectedList = new List<Student>();
            for (int i = 0; i < 5; i++)
            {
                expectedList.Add(_isuService.AddStudent(group, i.ToString()));
            }
            for (int i = 0; i < 5; i++)
            {
                _isuService.AddStudent(otherGroup, i.ToString());
            }

            List<Student> getList = _isuService.FindStudents("M3201");
            expectedList.ForEach((student) => Assert.Contains(student, getList));
            Assert.AreEqual(expectedList.Count, getList.Count);
        }
        
        [Test]
        public void FindStudentsTest_FindByCourseNumber_ReturnStudents()
        {
            Group group = _isuService.AddGroup("M3201");
            Group otherGroup = _isuService.AddGroup("M3101");
            var expectedList = new List<Student>();
            for (int i = 0; i < 5; i++)
            {
                expectedList.Add(_isuService.AddStudent(group, i.ToString()));
            }
            for (int i = 0; i < 5; i++)
            {
                _isuService.AddStudent(otherGroup, i.ToString());
            }

            List<Student> getList = _isuService.FindStudents(CourseNumber.Second);
            expectedList.ForEach(student => Assert.Contains(student, getList));
            Assert.AreEqual(expectedList.Count, getList.Count);
        }

        [Test]
        public void FindGroupTest_ReturnGroup()
        {
            Group group = _isuService.AddGroup("M3201");
            _isuService.AddGroup("M3101");
            Assert.AreEqual(group, _isuService.FindGroup("M3201"));
        }

        [Test]
        public void FindGroupsTest_ReturnGroups()
        {
            var expectedList = new List<Group>();
            for (int i = 0; i < 5; i++)
            {
                expectedList.Add(_isuService.AddGroup("M320" + i));
                _isuService.AddGroup("M310" + i);
                _isuService.AddGroup("M330" + i);
            }

            List<Group> getList = _isuService.FindGroups(CourseNumber.Second);
            expectedList.ForEach(group => Assert.Contains(group, getList));
            Assert.AreEqual(expectedList.Count, getList.Count);
        }
    }
}