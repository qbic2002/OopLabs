using System.Collections.Generic;
using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class Timetable
    {
        private List<WeekDays> _daysList = new List<WeekDays>();
        public Timetable()
        {
        }

        public void AddDay(WeekDays day)
        {
            if (_daysList.Contains(day))
                throw new IsuExtraException("Day already exists");
            _daysList.Add(day);
        }
    }
}