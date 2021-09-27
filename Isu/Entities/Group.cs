using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private int _numberOfStudents;
        private List<Student> _listOfStudents = new List<Student>();
        private int _maxStudents;
        public Group(GroupName groupName, int maxStudents)
        {
            StudentsInGroup = new ReadOnlyCollection<Student>(_listOfStudents);
            if (groupName is null)
                throw new IsuException("Wrong name of group");
            GroupName = groupName;
            _maxStudents = maxStudents;
        }

        public GroupName GroupName { get; }
        public ReadOnlyCollection<Student> StudentsInGroup { get; }
        public void AddStudent(Student student)
        {
            if (_listOfStudents.Contains(student))
                throw new IsuException("Student already in group");

            if (_numberOfStudents >= _maxStudents)
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
            return obj is Group group && GroupName.Equals(group.GroupName);
        }

        public override int GetHashCode()
        {
            return GroupName.GetHashCode();
        }
    }
}