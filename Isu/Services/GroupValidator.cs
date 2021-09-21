using System;
using Isu.Entities;
using Isu.Tools;
using static Isu.Entities.CourseNumberType;

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

        public GroupValidator(char groupLiteral, int groupDigit, int numberOfCourses, int maxNumberOfGroups, int indexOfNumberOfCourse, int indexOfFirstDigitOfGroup, int indexOfSecondDigitOfGroup, int maxStudentsPerGroup)
        {
            _groupLiteral = groupLiteral;
            _groupDigit = groupDigit;
            NumberOfCourses = numberOfCourses;
            MaxStudentsPerGroup = maxStudentsPerGroup;
            _maxNumberOfGroups = maxNumberOfGroups;
            _indexOfNumberOfCourse = indexOfNumberOfCourse;
            _indexOfFirstDigitOfGroup = indexOfFirstDigitOfGroup;
            _indexOfSecondDigitOfGroup = indexOfSecondDigitOfGroup;
        }

        public int MaxStudentsPerGroup { get; }
        public int NumberOfCourses { get; }
        public CourseNumber GetCourseNumber(string groupName)
        {
            int courseNumber;
            if (!int.TryParse(groupName[_indexOfNumberOfCourse].ToString(), out courseNumber))
                throw new IsuException("wrong name of group");
            return (CourseNumber)courseNumber;
        }

        public int GetGroupNumber(string groupName)
        {
            int groupNumber;
            string stringGroupNumber = groupName[_indexOfFirstDigitOfGroup].ToString() +
                                       groupName[_indexOfSecondDigitOfGroup].ToString();
            if (!int.TryParse(stringGroupNumber, out groupNumber)) throw new IsuException("wrong name of group");
            return groupNumber;
        }

        public bool NameCheck(string groupName)
        {
            int groupNumber;
            int courseNumber;
            int groupDigit;
            if (groupName.Length != 5)
                return false;
            if (groupName[0] != _groupLiteral || !int.TryParse(groupName[1].ToString(), out groupDigit) || groupDigit != _groupDigit)
                return false;
            string stringGroupNumber = groupName[_indexOfFirstDigitOfGroup].ToString() +
                                       groupName[_indexOfSecondDigitOfGroup].ToString();
            if (!int.TryParse(stringGroupNumber, out groupNumber))
                return false;
            if (!int.TryParse(groupName[_indexOfNumberOfCourse].ToString(), out courseNumber))
                return false;
            if (courseNumber < 1 || courseNumber >= NumberOfCourses)
                return false;
            if (groupNumber < 0 || groupNumber >= _maxNumberOfGroups)
                return false;
            return true;
        }
    }
}