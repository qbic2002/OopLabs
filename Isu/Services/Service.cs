using System.Collections.Generic;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class Service : IIsuService
    {
        private int _studentId;
        private List<Student> _studentsList = new List<Student>();
        private Dictionary<CourseNumber, List<Group>> _listOfGroupsInEachCourse = new ();
        private GroupValidator _groupValidator;
        private int _maxStudentsPerGroup;
        public Service(GroupValidator groupValidator, int maxStudentsPerGroup)
        {
            if (groupValidator is null)
                throw new IsuException("Incorrect validator");
            _groupValidator = groupValidator;
            for (int i = 1; i <= groupValidator.NumberOfCourses; i++)
            {
                _listOfGroupsInEachCourse.Add((CourseNumber)i, new List<Group>());
            }

            _maxStudentsPerGroup = maxStudentsPerGroup;
        }

        public Group AddGroup(string name)
        {
            if (!_groupValidator.NameCheck(name))
                throw new IsuException("Incorrect name of group");
            var groupName = new GroupName(name, _groupValidator.GetCourseNumber(name), _groupValidator.GetGroupNumber(name));
            var newGroup = new Group(groupName, _maxStudentsPerGroup);
            if (_listOfGroupsInEachCourse[newGroup.GroupName.CourseNumber].Contains(newGroup))
                throw new IsuException("group has been already created");
            _listOfGroupsInEachCourse[newGroup.GroupName.CourseNumber].Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            if (group is null)
                throw new IsuException("Group does not exist");
            var student = new Student(group, name, _studentId);

            group.AddStudent(student);
            _studentsList.Add(student);
            _studentId++;
            return student;
        }

        public Student GetStudent(int id)
        {
            if (id >= _studentId)
                throw new IsuException("Can't find student");
            return _studentsList[id];
        }

        public Student FindStudent(string name)
        {
            return _studentsList.Find(student => student.Name == name);
        }

        public List<Student> FindStudents(string groupName)
        {
            Group group = FindGroup(groupName);
            if (group is not null)
                return new List<Student>(group.StudentsInGroup);
            return null;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var listOfStudentOfCourse = new List<Student>();
            _listOfGroupsInEachCourse[courseNumber].ForEach(group => listOfStudentOfCourse.AddRange(group.StudentsInGroup));
            return listOfStudentOfCourse;
        }

        public Group FindGroup(string groupName)
        {
            if (!_groupValidator.NameCheck(groupName))
                throw new IsuException("Incorrect name of group");
            CourseNumber numberOfCourse = _groupValidator.GetCourseNumber(groupName);
            return _listOfGroupsInEachCourse[numberOfCourse].Find(group => group.GroupName.Name.Equals(groupName));
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            return _listOfGroupsInEachCourse[courseNumber];
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (newGroup is null)
                throw new IsuException("Group does not exist");

            student.ChangeGroup(newGroup);
        }
    }
}