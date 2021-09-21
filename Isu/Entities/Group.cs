using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isu.Tools;
using Microsoft.VisualBasic.CompilerServices;
using static Isu.Entities.CourseNumberType;

namespace Isu.Entities
{
    public class Group
    {
        private static char _groupLiteral;
        private static int _groupDigit;
        private int _maxStudents;
        private int _numberOfCourses;
        private int _numberOfStudents;
        private List<Student> _listOfStudents = new List<Student>();
        public Group(string name, char groupLiteral, int groupDigit, int numberOfCourses, int maxNumberOfGroups, int indexOfNumberOfCourse, int indexOfFirstDigitOfGroup, int indexOfSecondDigitOfGroup, int maxStudents)
            : this(GetCourseNumber(name, indexOfNumberOfCourse), GetGroupNumber(name, indexOfFirstDigitOfGroup, indexOfSecondDigitOfGroup), numberOfCourses, maxNumberOfGroups)
        {
            _maxStudents = maxStudents;
            _groupLiteral = groupLiteral;
            _groupDigit = groupDigit;
            if (!NameCheck(name)) throw new IsuException("wrong name of group");
            FullName = name;
        }

        private Group(CourseNumber courseNumber, int numberOfGroup, int numberOfCourses, int maxNumberOfGroups)
        {
            if ((int)courseNumber < 1 || (int)courseNumber > numberOfCourses)
            {
                throw new IsuException($"wrong number of course: {(int)courseNumber}, min: 0, max: {_numberOfCourses}");
            }

            if (numberOfGroup < 0 || numberOfGroup > maxNumberOfGroups)
            {
                throw new IsuException("wrong number of group");
            }

            _numberOfCourses = numberOfCourses;

            _numberOfStudents = 0;
            StudentsInGroup = new ReadOnlyCollection<Student>(_listOfStudents);
            CourseNumber = courseNumber;
            GroupNumber = numberOfGroup;
        }

        public string FullName { get; }
        public CourseNumber CourseNumber { get; }
        public int GroupNumber { get; }
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
            var other = obj as Group;
            return other is not null && FullName == other.FullName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private static CourseNumber GetCourseNumber(string groupName, int indexOfNumberOfCourse)
        {
            int courseNumber;
            if (!int.TryParse(groupName[indexOfNumberOfCourse].ToString(), out courseNumber))
                throw new IsuException("wrong name of group");
            return (CourseNumber)courseNumber;
        }

        private static int GetGroupNumber(string groupName, int indexOfFirstDigitOfGroup, int indexOfSecondDigitOfGroup)
        {
            int groupNumber;
            string stringGroupNumber = groupName[indexOfFirstDigitOfGroup].ToString() +
                                       groupName[indexOfSecondDigitOfGroup].ToString();
            if (!int.TryParse(stringGroupNumber, out groupNumber)) throw new IsuException("wrong name of group");
            return groupNumber;
        }

        private static bool NameCheck(string name)
        {
            if (name.Length != 5) return false;
            if (name[0] != _groupLiteral || int.Parse(name[1].ToString()) != _groupDigit) return false;
            return true;
        }
    }
}