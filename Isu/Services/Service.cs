using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Isu.Entities;
using Isu.Tools;
using CourseNumber = Isu.Entities.CourseNumberType.CourseNumber;

namespace Isu.Services
{
    public class Service : IIsuService
    {
        private int _studentId;
        private List<Student> _studentsList = new List<Student>();
        private List<List<Group>> _listOfGroupsInEachCourse = new List<List<Group>>();
        private GroupValidator _groupValidator;
        public Service(GroupValidator groupValidator)
        {
            if (groupValidator is null)
                throw new IsuException("Incorrect validator");
            _groupValidator = groupValidator;
            for (int i = 0; i < groupValidator.NumberOfCourses; i++)
            {
                _listOfGroupsInEachCourse.Add(new List<Group>());
            }
        }

        public Group AddGroup(string name)
        {
            var groupName = new GroupName(name, _groupValidator);
            var newGroup = new Group(groupName, _groupValidator);
            if (_listOfGroupsInEachCourse[(int)newGroup.GroupName.CourseNumber - 1].Contains(newGroup))
                throw new IsuException("group has been already created");

            _listOfGroupsInEachCourse[(int)newGroup.GroupName.CourseNumber - 1].Add(newGroup);
            Console.WriteLine(_listOfGroupsInEachCourse[(int)newGroup.GroupName.CourseNumber - 1].Count);
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
            if (id >= _studentId) throw new IsuException("Can't find student");
            return _studentsList[id];
        }

        public Student FindStudent(string name)
        {
            return _studentsList.Find(student => student.Name == name);
        }

        public List<Student> FindStudents(string groupName)
        {
            Group group = FindGroup(groupName);
            if (group is not null) return new List<Student>(group.StudentsInGroup);
            return null;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var listOfStudentOfCourse = new List<Student>();
            _listOfGroupsInEachCourse[(int)courseNumber - 1].ForEach(group => listOfStudentOfCourse.AddRange(group.StudentsInGroup));
            return listOfStudentOfCourse;
        }

        public Group FindGroup(string groupName)
        {
            GroupName nameOfGroup = new GroupName(groupName, _groupValidator);
            CourseNumber numberOfCourse = nameOfGroup.CourseNumber;
            return _listOfGroupsInEachCourse[(int)numberOfCourse - 1].Find(group => group.GroupName.Equals(nameOfGroup));
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            return _listOfGroupsInEachCourse[(int)courseNumber - 1];
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (newGroup is null)
            {
                throw new IsuException("Group does not exist");
            }

            student.ChangeGroup(newGroup);
        }
    }
}