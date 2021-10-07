using IsuExtra.Entities;
using IsuExtra.Tools;

namespace IsuExtra.Services
{
    public class JoinTrainingGroupManager
    {
        public JoinTrainingGroupManager()
        {
        }

        public JoinTrainingGroup AddJTG(Faculty faculty, Timetable timetable, int limitOfStudents)
        {
            if (timetable is null)
                throw new IsuExtraException("Incorrect timetable");
            if (limitOfStudents <= 0)
                throw new IsuExtraException("Incorrect limit of students");
            var jtgGroup = new JoinTrainingGroup(faculty, timetable, limitOfStudents);
            return jtgGroup;
        }
    }
}