using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RehabWithLogin.MVC.Data;
using RehabWithLogin.MVC.Helpers;

namespace RehabWithLogin.MVC.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalendarController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public IActionResult ExportToGoogleCalendar(int workoutPlanId)
        {
            var events = CreateCalenderEvents(workoutPlanId);

            if (events == null)
                return BadRequest();

            var reminderList = CalendarHelpers.GetReminderList();

            var service = CalendarHelpers.GetService();

            foreach (var eventToAdd in events)
            {
                eventToAdd.Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = reminderList
                };
                service.Events.Insert(eventToAdd, "primary").Execute();
            }
            return RedirectToAction("Index", "WorkoutPlan");
        }

        private List<Event> CreateCalenderEvents(int workoutPlanId)
        {
            var workoutPlan =
                _unitOfWork.WorkoutPlanRepository.Get(x => x.Id == workoutPlanId, null, "WorkoutPlanWorkouts.Workout")
                    .First();
            if (workoutPlan.WorkoutPlanWorkouts == null || workoutPlan.WorkoutPlanWorkouts.Count == 0)
                return null;

            var events = new List<Event>();
            foreach (var wpw in workoutPlan.WorkoutPlanWorkouts)
            {
                events.Add(new Event
                {
                    Summary = wpw.Workout.Name,
                    Location = "",
                    Start = new EventDateTime
                    {
                        DateTime = wpw.ScheduledTime,
                        TimeZone = "Europe/Stockholm"
                    },
                    End = new EventDateTime
                    {
                        DateTime = wpw.ScheduledTime.AddHours(1),
                        TimeZone = "Europe/Stockholm"
                    }
                });
            }

            return events;
        }
    }
}