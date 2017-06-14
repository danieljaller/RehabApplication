using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RehabWithLogin.MVC.Data;

namespace RehabWithLogin.MVC.Controllers
{
    public class CalendarController : Controller
    {
        private static readonly string[] scopes = {CalendarService.Scope.Calendar};
        private static readonly string applicationName = "Rehab";
        private readonly IUnitOfWork _unitOfWork;

        public CalendarController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        internal CalendarService GetService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = Environment.GetEnvironmentVariable("LocalAppData");
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true))
                    .Result;
            }
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            return service;
        }

        [Authorize]
        [HttpPost]
        public IActionResult ExportToGoogleCalendar(int workoutPlanId)
        {
            var events = CreateCalenderEvents(workoutPlanId);

            if (events == null)
                return BadRequest();

            var reminderList = GetReminderList();

            var service = GetService();

            foreach (var eventToAdd in events)
            {
                eventToAdd.Reminders = new Event.RemindersData();
                eventToAdd.Reminders.UseDefault = false;
                eventToAdd.Reminders.Overrides = reminderList;
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

            return events;
        }

        private static List<EventReminder> GetReminderList()
        {
            var reminder = new EventReminder
            {
                Method = "popup",
                Minutes = 30
            };
            var reminderList = new List<EventReminder> {reminder};
            return reminderList;
        }
    }
}