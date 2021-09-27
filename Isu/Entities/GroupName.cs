namespace Isu.Entities
{
    public class GroupName
    {
        public GroupName(string name, CourseNumber courseNumber, int groupNumber)
        {
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