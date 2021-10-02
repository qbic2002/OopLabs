using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isu.Tools;

namespace Isu.Entities
{
    public class Group
    {
        private List<Student> _listOfStudents = new List<Student>();
        private int _maxStudents;
        public Group(GroupName groupName, int maxStudents)
        {
            if (groupName is null)
                throw new IsuException("Wrong name of group");
            GroupName = groupName;
            _maxStudents = maxStudents;
            StudentsInGroup = new ReadOnlyCollection<Student>(_listOfStudents);
        }

        public GroupName GroupName { get; }
        public ReadOnlyCollection<Student> StudentsInGroup { get; }
        private int NumberOfStudents => _listOfStudents.Count;
        public void AddStudent(Student student)
        {
            if (_listOfStudents.Contains(student))
                throw new IsuException("Student already in group");

            if (NumberOfStudents >= _maxStudents)
                throw new IsuException("Too many students");
            _listOfStudents.Add(student);
        }

        public void DeleteStudent(Student student)
        {
            if (student is null)
                throw new IsuException("Student does not exist");
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