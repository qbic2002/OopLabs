using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class Timetable
    {
        private List<EducationDay> _daysList = new List<EducationDay>();
        public Timetable(params EducationDay[] educationDays)
        {
            if (educationDays is null || educationDays.Length == 0)
                throw new IsuExtraException("Incorrect education day");
            var educationDayList = educationDays.ToList();
            educationDayList.ForEach(AddDay);
            DayList = new ReadOnlyCollection<EducationDay>(_daysList);
        }

        public ReadOnlyCollection<EducationDay> DayList { get; }
        public void AddDay(EducationDay day)
        {
            if (_daysList.Exists(educationDay => educationDay.Day == day.Day))
                throw new IsuExtraException("Day already exists");
            _daysList.Add(day);
        }

        public override bool Equals(object obj)
        {
            var timetable = obj as Timetable;
            if (timetable is null)
                return false;
            bool isEducationDaysEquals = true;
            if (DayList.Count != timetable.DayList.Count)
                return false;
            _daysList.ForEach(educationDay =>
            {
                if (!timetable.DayList.Contains(educationDay))
                    isEducationDaysEquals = false;
            });
            return isEducationDaysEquals;
        }

        public override int GetHashCode()
        {
            return _daysList.GetHashCode();
        }
    }
}