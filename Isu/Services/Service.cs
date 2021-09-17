using System.Collections.Generic;
using Isu.Entities;
using Isu.Tools;
using CourseNumber = Isu.Entities.CourseNumberType.CourseNumber;

namespace Isu.Services
{
    public class Service : IIsuService
    {
        private int _maxStudentsPerGroup;
        private char _groupLiteral;
        private int _groupDigit;
        private int _indexOfNumberOfCourse;
        private int _numberOfCourses;
        private int _maxNumberOfGroups;
        private int _indexOfFirstNumberOfGroup;
        private int _indexOfSecondNumberOfGroup;
        private int _studentId;
        private List<Student> _studentsList = new List<Student>();
        private List<List<Group>> _listOfGroupsInEachCourse = new List<List<Group>>();
        public Service(char groupLiteral, int groupDigit, int numberOfCourses, int maxNumberOfGroups, int indexOfNumberOfCourse, int indexOfFirstNumberOfGroup, int indexOfSecondNumberOfGroup, int maxStudentsPerGroup)
        {
            _maxStudentsPerGroup = maxStudentsPerGroup;
            _groupLiteral = groupLiteral;
            _groupDigit = groupDigit;
            _numberOfCourses = numberOfCourses;
            _maxNumberOfGroups = maxNumberOfGroups;
            _indexOfNumberOfCourse = indexOfNumberOfCourse;
            _indexOfFirstNumberOfGroup = indexOfFirstNumberOfGroup;
            _indexOfSecondNumberOfGroup = indexOfSecondNumberOfGroup;
            for (int i = 0; i < _numberOfCourses; i++)
            {
                _listOfGroupsInEachCourse.Add(new List<Group>());
            }
        }

        public Group AddGroup(string name)
        {
            var newGroup = new Group(name, _groupLiteral, _groupDigit, _numberOfCourses, _maxNumberOfGroups, _indexOfNumberOfCourse, _indexOfFirstNumberOfGroup, _indexOfSecondNumberOfGroup, _maxStudentsPerGroup);
            foreach (Group group in _listOfGroupsInEachCourse[(int)newGroup.CourseNumber - 1])
            {
                if (group.FullName == name) throw new IsuException("group has been already created");
            }

            _listOfGroupsInEachCourse[(int)newGroup.CourseNumber - 1].Add(newGroup);
            return newGroup;
        }

        public Student AddStudent(Group group, string name)
        {
            if (group == null)
                throw new IsuException("Group does not exist");
            var student = new Student(group, name, _studentId);

            group.AddStudent(student);

            _studentsList.Add(student);
            _studentId++;
            return student;
        }

        public Student GetStudent(int id)
        {
            if (id >= _studentId) throw new IsuException("Can't find student");
            return _studentsList[id];
        }

        public Student FindStudent(string name)
        {
            foreach (Student student in _studentsList)
            {
                if (student.Name == name)
                {
                    return student;
                }
            }

            return null;
        }

        public List<Student> FindStudents(string groupName)
        {
            Group group = FindGroup(groupName);
            if (group != null) return group.StudentsInGroup;
            return null;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var listOfStudentOfCourse = new List<Student>();
            foreach (Group group in _listOfGroupsInEachCourse[(int)courseNumber - 1])
            {
                listOfStudentOfCourse.AddRange(group.StudentsInGroup);
            }

            return listOfStudentOfCourse;
        }

        public Group FindGroup(string groupName)
        {
            if (!int.TryParse(groupName[_indexOfNumberOfCourse].ToString(), out int numberOfCourse)) throw new IsuException("wrong name of a group");
            if (numberOfCourse < 0 || numberOfCourse > _numberOfCourses) return null;
            foreach (Group group in _listOfGroupsInEachCourse[numberOfCourse - 1])
            {
                if (group.FullName == groupName)
                {
                    return group;
                }
            }

            return null;
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            return _listOfGroupsInEachCourse[(int)courseNumber - 1];
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (newGroup == null)
            {
                throw new IsuException("Group does not exist");
            }

            student.ChangeGroup(newGroup);
        }
    }
}