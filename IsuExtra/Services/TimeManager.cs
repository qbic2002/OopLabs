using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Isu.Entities;
using IsuExtra.Entities;
using IsuExtra.Services;
using IsuExtra.Tools;

namespace IsuExtra.Services
{
    public class TimeManager
    {
        private Dictionary<Group, Timetable> _timetableAndGroup = new Dictionary<Group, Timetable>();
        private Dictionary<Student, Timetable> _timetableAndStudents = new Dictionary<Student, Timetable>();
        private Dictionary<LessonsTemplate, Lesson> _templateLessonsAndTime = new Dictionary<LessonsTemplate, Lesson>();
        public TimeManager()
        {
            _templateLessonsAndTime.Add(LessonsTemplate.Zero, new Lesson(new Time("08:20"), new Time("09:50")));
            _templateLessonsAndTime.Add(LessonsTemplate.First, new Lesson(new Time("10:00"), new Time("11:30")));
            _templateLessonsAndTime.Add(LessonsTemplate.Second, new Lesson(new Time("11:40"), new Time("13:10")));
            _templateLessonsAndTime.Add(LessonsTemplate.Third, new Lesson(new Time("13:30"), new Time("15:00")));
            _templateLessonsAndTime.Add(LessonsTemplate.Forth, new Lesson(new Time("15:20"), new Time("16:50")));

            TimetableAndGroup = new ReadOnlyDictionary<Group, Timetable>(_timetableAndGroup);
            TimetableAndStudent = new ReadOnlyDictionary<Student, Timetable>(_timetableAndStudents);
        }

        public ReadOnlyDictionary<Group, Timetable> TimetableAndGroup { get; }
        public ReadOnlyDictionary<Student, Timetable> TimetableAndStudent { get; }

        public static Timetable MergeTimetable(Timetable firstTimetable, Timetable secondTimetable)
        {
            var educationDays = new List<EducationDay>();
            firstTimetable.DayList.ToList().ForEach(firstDay =>
            {
                var educationDay = new EducationDay(firstDay.Day, firstDay.Lessons.ToArray());
                secondTimetable.DayList.ToList().ForEach(secondDay =>
                {
                    if (firstDay.Day == secondDay.Day)
                    {
                        secondDay.Lessons.ToList().ForEach(educationDay.AddLesson);
                    }
                });
                educationDays.Add(educationDay);
            });

            secondTimetable.DayList.ToList().ForEach(secondDay =>
            {
                if (!educationDays.Exists(educationDay => educationDay.Day == secondDay.Day))
                    educationDays.Add(secondDay);
            });
            return new Timetable(educationDays.ToArray());
        }

        public static bool IsIntersects(Lesson firstLesson, Lesson secondLesson)
        {
            if (firstLesson is null || secondLesson is null)
                throw new IsuExtraException("Incorrect lesson");

            if (firstLesson.StartTime <= secondLesson.StartTime && secondLesson.StartTime <= firstLesson.EndTime)
                return true;
            if (secondLesson.StartTime <= firstLesson.StartTime && firstLesson.StartTime <= secondLesson.EndTime)
                return true;
            return false;
        }

        public static bool IsIntersects(EducationDay firstDay, EducationDay secondDay)
        {
            if (firstDay is null || secondDay is null)
                throw new IsuExtraException("Incorrect days");
            if (firstDay.Day != secondDay.Day)
                return false;

            bool isIntersects = false;
            firstDay.Lessons.ToList().ForEach(firstLesson =>
            {
                secondDay.Lessons.ToList().ForEach(secondLesson =>
                {
                    if (IsIntersects(firstLesson, secondLesson))
                    {
                        isIntersects = true;
                    }
                });
            });
            return isIntersects;
        }

        public static bool IsIntersects(Timetable firstTimetable, Timetable secondTimetable)
        {
            if (firstTimetable is null || secondTimetable is null)
                throw new IsuExtraException("Incorrect timetables");

            bool isIntersect = false;
            firstTimetable.DayList.ToList().ForEach(firstDay =>
            {
                secondTimetable.DayList.ToList().ForEach(secondDay =>
                {
                    if (IsIntersects(firstDay, secondDay))
                    {
                        isIntersect = true;
                    }
                });
            });
            return isIntersect;
        }

        public Lesson AddLesson(Time startTime, Time endTime, Teacher teacher, ClassRoom classRoom)
        {
            if (startTime is null)
                throw new IsuExtraException("Incorrect start time");
            if (endTime is null)
                throw new IsuExtraException("Incorrect end time");
            if (teacher is null)
                throw new IsuExtraException("Incorrect teacher");
            if (classRoom is null)
                throw new IsuExtraException("Incorrect classroom");

            return new Lesson(startTime, endTime, teacher, classRoom);
        }

        public Lesson AddLesson(LessonsTemplate templateLesson, Teacher teacher, ClassRoom classRoom)
        {
            if (teacher is null)
                throw new IsuExtraException("Incorrect teacher");
            if (classRoom is null)
                throw new IsuExtraException("Incorrect classroom");

            Lesson lesson = _templateLessonsAndTime[templateLesson];
            if (lesson is null)
                throw new IsuExtraException("Incorrect template");
            lesson.AddTeacher(teacher);
            lesson.AddClassRoom(classRoom);
            return lesson;
        }

        public EducationDay AddEducationDay(WeekDays weekDay, params Lesson[] lessons)
        {
            if (lessons is null)
                throw new IsuExtraException("Incorrect lessons");
            return new EducationDay(weekDay, lessons);
        }

        public Timetable AddTimetable(params EducationDay[] educationDays)
        {
            if (educationDays is null || educationDays.Length == 0)
                throw new IsuExtraException("Incorrect education day");
            return new Timetable(educationDays);
        }

        public void AssociateTimetableWithGroup(Timetable timetable, Group group)
        {
            if (group is null)
                throw new IsuExtraException("Incorrect group");
            if (timetable is null)
                throw new IsuExtraException("Incorrect timetable");

            if (_timetableAndGroup.ContainsKey(group))
                throw new IsuExtraException("Group already has timetable");
            _timetableAndGroup.Add(group, timetable);
        }

        public Timetable GetTimetable(Student student)
        {
            if (student is null)
                throw new IsuExtraException("Incorrect Student");
            if (!_timetableAndStudents.ContainsKey(student))
            {
                if (!_timetableAndGroup.ContainsKey(student.Group))
                    throw new IsuExtraException("Can't get timetable");
                _timetableAndStudents.Add(student, _timetableAndGroup[student.Group]);
            }

            return _timetableAndStudents[student];
        }

        public void AddJTGTimetableToStudent(Timetable timetable, Student student)
        {
            if (timetable is null)
                throw new IsuExtraException("Incorrect timetable");
            if (student is null)
                throw new IsuExtraException("Incorrect Student");
            _timetableAndStudents[student] = MergeTimetable(timetable, GetTimetable(student));
        }
    }
}