using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class GroupValidator
    {
        private static char _groupLiteral;
        private static int _groupDigit;
        private int _indexOfNumberOfCourse;
        private int _maxNumberOfGroups;
        private int _indexOfFirstDigitOfGroup;
        private int _indexOfSecondDigitOfGroup;

        public GroupValidator(char groupLiteral, int groupDigit, int numberOfCourses, int maxNumberOfGroups, int indexOfNumberOfCourse, int indexOfFirstDigitOfGroup, int indexOfSecondDigitOfGroup)
        {
            _groupLiteral = groupLiteral;
            _groupDigit = groupDigit;
            NumberOfCourses = numberOfCourses;
            _maxNumberOfGroups = maxNumberOfGroups;
            _indexOfNumberOfCourse = indexOfNumberOfCourse;
            _indexOfFirstDigitOfGroup = indexOfFirstDigitOfGroup;
            _indexOfSecondDigitOfGroup = indexOfSecondDigitOfGroup;
        }

        public int NumberOfCourses { get; }
        public CourseNumber GetCourseNumber(string groupName)
        {
            if (!int.TryParse(groupName[_indexOfNumberOfCourse].ToString(), out int courseNumber))
                throw new IsuException("wrong name of group");
            return (CourseNumber)(courseNumber - 1);
        }

        public int GetGroupNumber(string groupName)
        {
            string stringGroupNumber = groupName[_indexOfFirstDigitOfGroup].ToString() +
                                       groupName[_indexOfSecondDigitOfGroup].ToString();
            if (!int.TryParse(stringGroupNumber, out int groupNumber))
                throw new IsuException("wrong name of group");
            return groupNumber;
        }

        public bool NameCheck(string groupName)
        {
            if (groupName.Length != 5)
                return false;
            if (groupName[0] != _groupLiteral || !int.TryParse(groupName[1].ToString(), out int groupDigit) || groupDigit != _groupDigit)
                return false;
            string stringGroupNumber = groupName[_indexOfFirstDigitOfGroup].ToString() +
                                       groupName[_indexOfSecondDigitOfGroup].ToString();
            if (!int.TryParse(stringGroupNumber, out int groupNumber))
                return false;
            if (!int.TryParse(groupName[_indexOfNumberOfCourse].ToString(), out int courseNumber))
                return false;
            if (courseNumber < 1 || courseNumber >= NumberOfCourses)
                return false;
            if (groupNumber < 0 || groupNumber >= _maxNumberOfGroups)
                return false;
            return true;
        }
    }
}