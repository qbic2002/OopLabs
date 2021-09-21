using Isu.Services;
using Isu.Tools;
using static Isu.Entities.CourseNumberType;

namespace Isu.Entities
{
    public class GroupName
    {
        private GroupValidator _groupValidator;
        public GroupName(string name, GroupValidator groupValidator)
        {
            if (groupValidator is null)
                throw new IsuException("Incorrect validator");
            if (!groupValidator.NameCheck(name))
                throw new IsuException("Incorrect name of group");
            Name = name;
            _groupValidator = groupValidator;
            CourseNumber = _groupValidator.GetCourseNumber(name);
            GroupNumber = _groupValidator.GetGroupNumber(name);
        }

        public string Name { get; }
        public CourseNumber CourseNumber { get; }
        public int GroupNumber { get; }

        public override bool Equals(object obj)
        {
            var other = obj as GroupName;
            return other is not null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}