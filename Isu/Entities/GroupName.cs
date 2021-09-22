using Isu.Services;
using Isu.Tools;
using static Isu.Entities.CourseNumberType;

namespace Isu.Entities
{
    public class GroupName
    {
        public GroupName(string name, GroupValidator groupValidator)
        {
            if (groupValidator is null)
                throw new IsuException("Incorrect validator");
            if (!groupValidator.NameCheck(name))
                throw new IsuException("Incorrect name of group");
            Name = name;
            CourseNumber = groupValidator.GetCourseNumber(name);
            GroupNumber = groupValidator.GetGroupNumber(name);
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
            return base.GetHashCode();
        }
    }
}