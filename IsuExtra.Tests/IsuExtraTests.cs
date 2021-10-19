using System.Collections.Generic;
using System.Linq;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using IsuExtra.Entities;
using IsuExtra.Services;
using IsuExtra.Tools;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    [TestFixture]
    public class IsuExtraTests
    {
        private JoinTrainingGroupManager _jtgManager;
        private TimeManager _timeManager;
        private IsuService _isuService;
        private GroupValidator _groupValidator;

        [SetUp]
        public void SetUp()
        {
            _timeManager = new TimeManager();
            _jtgManager = new JoinTrainingGroupManager(_timeManager);
            _groupValidator = new GroupValidator(new List<char> {'B', 'D', 'K', 'L', 'M', 'N', 'p', 'R', 'T', 'U', 'V', 'W', 'Z'}, 3, 4, 30, 2, 3, 4);
            _isuService = new IsuService(_groupValidator, 30);
        }

        [Test]
        public void AddIntersectsLessons_ThrowException()
        {
            Lesson lesson1 = _timeManager.AddLesson(new Time("08:20"), new Time("09:50"), _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            Lesson lesson2 = _timeManager.AddLesson(new Time("08:30"), new Time("10:50"), _jtgManager.AddTeacher("Test1"), new ClassRoom(112));

            Assert.Catch<IsuExtraException>(() =>
            {
                _timeManager.AddEducationDay(WeekDays.Monday, lesson1, lesson2);
            });
        }
        
        [Test]
        public void AddNotIntersectsLessons()
        {
            Lesson lesson1 = _timeManager.AddLesson(new Time("08:20"), new Time("09:50"), _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            Lesson lesson2 = _timeManager.AddLesson(new Time("11:40"), new Time("13:10"), _jtgManager.AddTeacher("Test1"), new ClassRoom(112));

            _timeManager.AddEducationDay(WeekDays.Monday, lesson1, lesson2);
        }
        
        [Test]
        public void AddExistsDayToTimetable_ThrowException()
        {
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday);
            EducationDay anotherMonday = _timeManager.AddEducationDay(WeekDays.Monday);
            
            Assert.Catch<IsuExtraException>(() =>
            {
                _timeManager.AddTimetable(monday, anotherMonday);
            });
        }

        [Test]
        public void AddStudentToJTG()
        {
            Group m3201 = _isuService.AddGroup("M3201");
            Student student = _isuService.AddStudent(m3201, "test");

            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            _timeManager.AssociateTimetableWithGroup(timetable, m3201);
            
            Lesson jtgLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, jtgLesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgTimetable = _timeManager.AddTimetable(jtgMonday, jtgTuesday);

            JoinTrainingGroup jtGroup = _jtgManager.AddJTG(Faculty.CTM);
            _jtgManager.AddThread(jtGroup, jtgTimetable, 30);
            _jtgManager.AddJTGToStudent(student, jtGroup, jtGroup.Threads.FirstOrDefault());
            if (!jtGroup.Contains(student))
                Assert.Fail();
        }
        
        [Test]
        public void AddStudentToJTG_TimetablesIntersects_ThrowException()
        {
            Group m3201 = _isuService.AddGroup("M3201");
            Student student = _isuService.AddStudent(m3201, "test");

            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            _timeManager.AssociateTimetableWithGroup(timetable, m3201);
            
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgTimetable = _timeManager.AddTimetable(jtgMonday, jtgTuesday);

            JoinTrainingGroup jtGroup = _jtgManager.AddJTG(Faculty.CTM);
            _jtgManager.AddThread(jtGroup, jtgTimetable, 30);
            Assert.Catch<IsuExtraException>(() =>
            {
                _jtgManager.AddJTGToStudent(student, jtGroup, jtGroup.Threads.FirstOrDefault());
            });
        }

        [Test]
        public void DeleteStudentFromJTG()
        {
            Group m3201 = _isuService.AddGroup("M3201");
            Student student = _isuService.AddStudent(m3201, "test");

            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            _timeManager.AssociateTimetableWithGroup(timetable, m3201);
            
            Lesson jtgLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, jtgLesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgTimetable = _timeManager.AddTimetable(jtgMonday, jtgTuesday);

            JoinTrainingGroup jtGroup = _jtgManager.AddJTG(Faculty.CTM);
            _jtgManager.AddThread(jtGroup, jtgTimetable, 30);
            _jtgManager.AddJTGToStudent(student, jtGroup, jtGroup.Threads.FirstOrDefault());
            if (!jtGroup.Contains(student))
                Assert.Fail();
            _jtgManager.DeleteStudent(student, jtGroup);
            if (jtGroup.Contains(student))
                Assert.Fail();
        }

        [Test]
        public void GetThreadsInGroup()
        {
            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            Lesson jtgLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, jtgLesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgFirstTimetable = _timeManager.AddTimetable(jtgMonday, jtgTuesday);
            
            Timetable jtgSecondTimetable = _timeManager.AddTimetable(jtgTuesday);

            JoinTrainingGroup jtGroup = _jtgManager.AddJTG(Faculty.CTM);
            Thread firstThread = _jtgManager.AddThread(jtGroup, jtgFirstTimetable, 30);
            Thread secondThread = _jtgManager.AddThread(jtGroup, jtgSecondTimetable, 30);

            var expectedList = new List<Thread>()
            {
                firstThread,
                secondThread
            };
            List<Thread> listOfThreads = _jtgManager.GetThreads(jtGroup);
            expectedList.ForEach(student => Assert.Contains(student, listOfThreads));
            Assert.AreEqual(expectedList.Count, listOfThreads.Count);
        }
        
        [Test]
        public void GetListOfStudents()
        {
            Group m3201 = _isuService.AddGroup("M3201");

            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            _timeManager.AssociateTimetableWithGroup(timetable, m3201);
            
            Lesson jtgLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, jtgLesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgFirstTimetable = _timeManager.AddTimetable(jtgMonday, jtgTuesday);
            
            Timetable jtgSecondTimetable = _timeManager.AddTimetable(jtgTuesday);

            JoinTrainingGroup jtGroup = _jtgManager.AddJTG(Faculty.CTM);
            Thread firstThread = _jtgManager.AddThread(jtGroup, jtgFirstTimetable, 30);
            Thread secondThread = _jtgManager.AddThread(jtGroup, jtgSecondTimetable, 30);
            
            Student studentA = _isuService.AddStudent(m3201, "testA");
            Student studentB = _isuService.AddStudent(m3201, "testB");
            Student studentC = _isuService.AddStudent(m3201, "testC");
            
            _jtgManager.AddJTGToStudent(studentA, jtGroup, firstThread);
            _jtgManager.AddJTGToStudent(studentB, jtGroup, secondThread);
            _jtgManager.AddJTGToStudent(studentC, jtGroup, firstThread);

            var firstExpectedList = new List<Student>()
            {
                studentA,
                studentC
            };
            var secondExpectedList = new List<Student>()
            {
                studentB
            };
            
            List<Student> listOfStudentsInThread = _jtgManager.GetStudents(jtGroup, firstThread);
            firstExpectedList.ForEach(student => Assert.Contains(student, listOfStudentsInThread));
            Assert.AreEqual(firstExpectedList.Count, listOfStudentsInThread.Count);

            listOfStudentsInThread = _jtgManager.GetStudents(jtGroup, secondThread);
            secondExpectedList.ForEach(student => Assert.Contains(student, listOfStudentsInThread));
            Assert.AreEqual(secondExpectedList.Count, listOfStudentsInThread.Count);
        }

        [Test]
        public void GetFreeStudentsInGroup()
        {
            Group m3201 = _isuService.AddGroup("M3201");

            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            _timeManager.AssociateTimetableWithGroup(timetable, m3201);
            
            Lesson jtgLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, jtgLesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgFirstTimetable = _timeManager.AddTimetable(jtgMonday, jtgTuesday);

            JoinTrainingGroup jtGroup = _jtgManager.AddJTG(Faculty.CTM);
            Thread firstThread = _jtgManager.AddThread(jtGroup, jtgFirstTimetable, 30);
            
            Student studentA = _isuService.AddStudent(m3201, "testA");
            Student studentB = _isuService.AddStudent(m3201, "testB");
            Student studentC = _isuService.AddStudent(m3201, "testC");
            
            _jtgManager.AddJTGToStudent(studentA, jtGroup, firstThread);

            var expectedList = new List<Student>()
            {
                studentB,
                studentC
            };
            List<Student> listOfFreeStudents = _jtgManager.GetFreeStudents(m3201);
            expectedList.ForEach(student => Assert.Contains(student, listOfFreeStudents));
            Assert.AreEqual(expectedList.Count, listOfFreeStudents.Count);
        }

        [Test]
        public void MergeTimetables()
        {
            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay firstMonday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable firstTimetable = _timeManager.AddTimetable(firstMonday, tuesday);

            Lesson secondLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay secondMonday = _timeManager.AddEducationDay(WeekDays.Monday, secondLesson1);
            Lesson secondLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay wednesday = _timeManager.AddEducationDay(WeekDays.Wednesday, secondLesson2);
            Timetable secondTimetable = _timeManager.AddTimetable(secondMonday, wednesday);

            Timetable merged = TimeManager.MergeTimetable(firstTimetable, secondTimetable);
            Timetable expectedTimetable = _timeManager.AddTimetable(new EducationDay(WeekDays.Monday, lesson1, secondLesson1), tuesday, wednesday);
            var listOfEducationDays = merged.DayList.ToList();
            expectedTimetable.DayList.ToList().ForEach(educationDay => Assert.Contains(educationDay, listOfEducationDays));
            Assert.AreEqual(expectedTimetable.DayList.Count, listOfEducationDays.Count);
        }

        [Test]
        public void AddSecondJTGToStudent_TimetablesIntersects_ThrowException()
        {
            Group m3201 = _isuService.AddGroup("M3201");
            Student student = _isuService.AddStudent(m3201, "test");

            Lesson lesson1 = _timeManager.AddLesson(LessonsTemplate.First, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay monday = _timeManager.AddEducationDay(WeekDays.Monday, lesson1);
            Lesson lesson2 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay tuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, lesson2);
            Timetable timetable = _timeManager.AddTimetable(monday, tuesday);
            
            _timeManager.AssociateTimetableWithGroup(timetable, m3201);
            
            Lesson jtgLesson1 = _timeManager.AddLesson(LessonsTemplate.Zero, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgMonday = _timeManager.AddEducationDay(WeekDays.Monday, jtgLesson1);
            Lesson jtgLesson2 = _timeManager.AddLesson(LessonsTemplate.Second, _jtgManager.AddTeacher("Test"), new ClassRoom(111));
            EducationDay jtgTuesday = _timeManager.AddEducationDay(WeekDays.Tuesday, jtgLesson2);
            Timetable jtgTimetable = _timeManager.AddTimetable(jtgMonday);

            JoinTrainingGroup firstJtGroup = _jtgManager.AddJTG(Faculty.CTM);
            Thread firstThread = _jtgManager.AddThread(firstJtGroup, jtgTimetable, 30);
            _jtgManager.AddJTGToStudent(student, firstJtGroup, firstThread);
            
            JoinTrainingGroup secondJtGroup = _jtgManager.AddJTG(Faculty.PT);
            Thread secondThread = _jtgManager.AddThread(secondJtGroup, _timeManager.AddTimetable(jtgMonday, jtgTuesday), 30);
            Assert.Catch<IsuExtraException>(() =>
            {
                _jtgManager.AddJTGToStudent(student, secondJtGroup, secondThread);
            });
        }
    }
}