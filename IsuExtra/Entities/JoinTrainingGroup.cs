using System;
using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class JoinTrainingGroup
    {
        public JoinTrainingGroup(Faculty faculty, Timetable timetable, int limitOfStudents)
        {
            Timetable = timetable ?? throw new IsuExtraException("Incorrect timetable");
            if (limitOfStudents <= 0)
                throw new IsuExtraException("Incorrect limit of students");
            Faculty = faculty;
            LimitOfStudents = limitOfStudents;
        }

        public Faculty Faculty { get; }
        public Timetable Timetable { get; }
        public int LimitOfStudents { get; }
    }
}