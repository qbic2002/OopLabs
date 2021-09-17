using System;
using Isu.Tools;

namespace Isu.Entities
{
    public class Student
    {
        public Student(Group group, string name, int id)
        {
            Group = group;
            Name = name;
            Id = id;
        }

        public Group Group { get; private set; }
        public string Name { get; }
        public int Id { get; }
        public void ChangeGroup(Group newGroup)
        {
            newGroup.AddStudent(this);
            Group.DeleteStudent(this);
            Group = newGroup;
        }
    }
}