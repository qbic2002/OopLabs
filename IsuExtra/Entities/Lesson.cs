using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class Lesson
    {
        public Lesson(Time startTime, Time endTime, Teacher teacher, ClassRoom classRoom)
            : this(startTime, endTime)
        {
            Teacher = teacher ?? throw new IsuExtraException("Incorrect teacher");
            ClassRoom = classRoom ?? throw new IsuExtraException("Incorrect classroom");
        }

        public Lesson(Time startTime, Time endTime)
        {
            if (startTime is null)
                throw new IsuExtraException("Incorrect start time");
            if (endTime is null)
                throw new IsuExtraException("Incorrect end time");
            if (startTime > endTime)
                throw new IsuExtraException("Start time must be less then end time");
            StartTime = startTime;
            EndTime = endTime;
        }

        public Time StartTime { get; }
        public Time EndTime { get; }
        public Teacher Teacher { get; private set; }
        public ClassRoom ClassRoom { get; private set; }

        public void AddTeacher(Teacher teacher)
        {
            Teacher = teacher ?? throw new IsuExtraException("Incorrect teacher");
        }

        public void AddClassRoom(ClassRoom classRoom)
        {
            ClassRoom = classRoom ?? throw new IsuExtraException("Incorrect classroom");
        }
    }
}