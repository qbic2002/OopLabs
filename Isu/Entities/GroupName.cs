using Isu.Tools;

namespace Isu.Entities
{
    public class GroupName
    {
        public GroupName(string name, CourseNumber courseNumber, int groupNumber)
        {
            if (name is null)
                throw new IsuException("Incorrect name");
            if (groupNumber < 0)
                throw new IsuException("Incorrect number of group");
            Name = name;
            CourseNumber = courseNumber;
            GroupNumber = groupNumber;
        }

        public string Name { get; }
        public CourseNumber CourseNumber { get; }
        public int GroupNumber { get; }

        public override bool Equals(object obj)
        {
            return obj is GroupName groupName && Name == groupName.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}