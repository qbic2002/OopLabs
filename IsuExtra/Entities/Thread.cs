using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isu.Entities;
using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class Thread
    {
        private List<Student> _students = new List<Student>();
        public Thread(Timetable timetable, int limitOfStudents)
        {
            Timetable = timetable ?? throw new IsuExtraException("Incorrect timetable");
            if (limitOfStudents <= 0)
                throw new IsuExtraException("Incorrect limit of students");
            LimitOfStudents = limitOfStudents;
            Students = new ReadOnlyCollection<Student>(_students);
        }

        public Timetable Timetable { get; }
        public int LimitOfStudents { get; }
        public ReadOnlyCollection<Student> Students { get; }
        public int NumberOfStudents => _students.Count;

        public void AddStudent(Student student)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            if (NumberOfStudents == LimitOfStudents)
                throw new IsuExtraException("Too mane students");
            _students.Add(student);
        }

        public void DeleteStudent(Student student)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            if (_students.Contains(student))
                _students.Remove(student);
        }

        public override bool Equals(object obj)
        {
            return (obj is Thread thread) && Timetable.Equals(thread.Timetable);
        }

        public override int GetHashCode()
        {
            return Timetable.GetHashCode();
        }
    }
}