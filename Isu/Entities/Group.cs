using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isu.Services;
using Isu.Tools;
using static Isu.Entities.CourseNumberType;

namespace Isu.Entities
{
    public class Group
    {
        private GroupValidator _groupValidator;
        private int _numberOfStudents;
        private List<Student> _listOfStudents = new List<Student>();
        public Group(GroupName groupName, GroupValidator groupValidator)
        {
            StudentsInGroup = new ReadOnlyCollection<Student>(_listOfStudents);
            if (groupName is null)
                throw new IsuException("Wrong name of group");
            if (groupValidator is null)
                throw new IsuException("Incorrect validator");
            GroupName = groupName;
            _groupValidator = groupValidator;
        }

        public GroupName GroupName { get; }
        public ReadOnlyCollection<Student> StudentsInGroup { get; }
        public void AddStudent(Student student)
        {
            if (_listOfStudents.Contains(student))
                throw new IsuException("Student already in group");

            if (_numberOfStudents >= _groupValidator.MaxStudentsPerGroup)
                throw new IsuException("Too many students");
            _listOfStudents.Add(student);
            _numberOfStudents++;
        }

        public void DeleteStudent(Student student)
        {
            _listOfStudents.Remove(student);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Group;
            return other is not null && GroupName.Equals(other.GroupName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}