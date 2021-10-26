using System.Collections.Generic;
using System.Collections.ObjectModel;
using Isu.Entities;
using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class JoinTrainingGroup
    {
        private List<Thread> _threads = new List<Thread>();

        public JoinTrainingGroup(Faculty faculty)
        {
            Faculty = faculty;
            Threads = new ReadOnlyCollection<Thread>(_threads);
        }

        public Faculty Faculty { get; }
        public ReadOnlyCollection<Thread> Threads { get; }

        public void AddThread(Thread thread)
        {
            if (thread is null)
                throw new IsuExtraException("Incorrect thread");
            if (_threads.Contains(thread))
                throw new IsuExtraException("Thread already exists");
            _threads.Add(thread);
        }

        public void AddStudent(Student student, Thread thread)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            if (thread is null || !_threads.Contains(thread))
                throw new IsuExtraException("Incorrect thread");
            thread.AddStudent(student);
        }

        public bool CheckForStudent(Thread thread, Student student)
        {
            if (thread is null)
                throw new IsuExtraException("Incorrect thread");
            if (!_threads.Contains(thread))
                return false;
            if (Contains(student))
                return false;
            if (thread.NumberOfStudents < thread.LimitOfStudents)
                return true;
            return false;
        }

        public bool Contains(Student student)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            return _threads.Exists(thread => thread.Students.Contains(student));
        }

        public void DeleteStudent(Student student)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            _threads.Find(thread => thread.Students.Contains(student))?.DeleteStudent(student);
        }
    }
}