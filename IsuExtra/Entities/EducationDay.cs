﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IsuExtra.Services;
using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class EducationDay
    {
        private List<Lesson> _lessons = new List<Lesson>();

        public EducationDay(WeekDays weekDay, params Lesson[] lessons)
        {
            if (lessons is null)
                throw new IsuExtraException("Incorrect lessons");
            var listLesson = lessons.ToList();
            listLesson.ForEach(AddLesson);
            Day = weekDay;
            Lessons = new ReadOnlyCollection<Lesson>(_lessons);
        }

        public WeekDays Day { get; }
        public ReadOnlyCollection<Lesson> Lessons { get; }

        public void AddLesson(Lesson lesson)
        {
            if (lesson is null)
                throw new IsuExtraException("Incorrect lesson");
            _lessons.ForEach(varLesson =>
            {
                if (TimeManager.IsIntersects(varLesson, lesson))
                    throw new IsuExtraException("Lesson intersects");
            });
            _lessons.Add(lesson);
        }

        public override bool Equals(object obj)
        {
            var educationDay = obj as EducationDay;
            if (educationDay is null)
                return false;
            if (Day != educationDay.Day)
                return false;
            if (_lessons.Count != educationDay._lessons.Count)
                return false;
            return _lessons.All(lesson => educationDay._lessons.Contains(lesson));
        }

        public override int GetHashCode()
        {
            return _lessons.GetHashCode();
        }
    }
}