using Isu.Tools;

namespace Isu.Entities
{
    public class Student
    {
        public Student(Group group, string name, int id)
        {
            if (group is null)
                throw new IsuException("Wrong group");
            if (name is null)
                throw new IsuException("Wrong name");
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

        public override bool Equals(object obj)
        {
            return obj is Student student && Id == student.Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}