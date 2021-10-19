using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Isu.Entities;
using IsuExtra.Entities;
using IsuExtra.Tools;

namespace IsuExtra.Services
{
    public class JoinTrainingGroupManager
    {
        private Dictionary<Student, JTGroupsOfStudent> _studentsAndJTG = new Dictionary<Student, JTGroupsOfStudent>();
        private int _teacherId = 0;
        private TimeManager _timeManager;
        public JoinTrainingGroupManager(TimeManager timeManager)
        {
            if (timeManager is null)
                throw new IsuExtraException("Incorrect time manager");
            _timeManager = timeManager;
        }

        public JoinTrainingGroup AddJTG(Faculty faculty)
        {
            var jtgGroup = new JoinTrainingGroup(faculty);
            return jtgGroup;
        }

        public Thread AddThread(JoinTrainingGroup jtgGroup, Timetable timetable, int limitOfStudents)
        {
            if (timetable is null)
                throw new IsuExtraException("Incorrect timetable");
            if (limitOfStudents <= 0)
                throw new IsuExtraException("Incorrect limit of students");
            var newThread = new Thread(timetable, limitOfStudents);
            jtgGroup.AddThread(newThread);
            return newThread;
        }

        public void AddJTGToStudent(Student student, JoinTrainingGroup jtGroup, Thread thread)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect students");
            if (jtGroup is null)
                throw new IsuExtraException("Incorrect JTG group");
            if (thread is null)
                throw new IsuExtraException("incorrect thread");

            if (!CheckStudentForJTG(student, jtGroup))
                throw new IsuExtraException("Too many JTGroups");
            if (!CheckJTGForStudent(student, jtGroup, thread))
                throw new IsuExtraException("Can't add student");

            jtGroup.AddStudent(student, thread);
            _studentsAndJTG[student].AddJTG(jtGroup);
            _timeManager.AddJTGTimetableToStudent(thread.Timetable, student);
        }

        public Teacher AddTeacher(string name)
        {
            if (name is null)
                throw new IsuExtraException("Incorrect name");
            return new Teacher(name, _teacherId++);
        }

        public void DeleteStudent(Student student, JoinTrainingGroup jtGroup)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            if (jtGroup is null)
                throw new IsuExtraException("Incorrect jtGroup");

            if (!jtGroup.Contains(student))
                throw new IsuExtraException("Student not in jtGroup");
            jtGroup.DeleteStudent(student);
        }

        public List<Thread> GetThreads(JoinTrainingGroup jtGroup)
        {
            if (jtGroup is null)
                throw new IsuExtraException("Incorrect jtGroup");
            return jtGroup.Threads.ToList();
        }

        public List<Student> GetStudents(JoinTrainingGroup jtGroup, Thread thread)
        {
            if (jtGroup is null)
                throw new IsuExtraException("Incorrect JTG group");
            if (thread is null)
                throw new IsuExtraException("incorrect thread");
            if (!jtGroup.Threads.Contains(thread))
                throw new IsuExtraException("Thread not in this group");

            return thread.Students.ToList();
        }

        public List<Student> GetFreeStudents(Group group)
        {
            if (group is null)
                throw new IsuExtraException("Incorrect group");
            return group.StudentsInGroup.Where(student => !_studentsAndJTG.ContainsKey(student) || _studentsAndJTG[student].IsEmpty()).ToList();
        }

        private bool CheckStudentForJTG(Student student, JoinTrainingGroup jtGroup)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect student");
            if (jtGroup is null)
                throw new IsuExtraException("Incorrect JTGroup");

            if (jtGroup.Faculty == student.Group.GroupName.Name[0].GetFaculty())
                return false;
            if (!_studentsAndJTG.ContainsKey(student))
                _studentsAndJTG.Add(student, new JTGroupsOfStudent());
            if (_studentsAndJTG[student].FirstJTG is null)
                return true;
            if (_studentsAndJTG[student].SecondJTG is null)
                return true;
            return false;
        }

        private bool CheckJTGForStudent(Student student, JoinTrainingGroup jtGroup, Thread thread)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect Student");
            if (jtGroup is null)
                throw new IsuExtraException("Incorrect JTG group");
            if (thread is null)
                throw new IsuExtraException("Incorrect thread");
            if (jtGroup.CheckForStudent(thread, student))
            {
                if (TimeManager.IsIntersects(thread.Timetable, _timeManager.GetTimetable(student)))
                    return false;
                return true;
            }

            return false;
        }
    }
}