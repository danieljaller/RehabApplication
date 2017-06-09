using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using RehabWithLogin.MVC.Data;

namespace RehabWithLogin.MVC.Controllers
{
    public class CalendarController : Controller
    {
        static string[] scopes = { CalendarService.Scope.Calendar };
        static string applicationName = "Rehab";
        private readonly IUnitOfWork _unitOfWork;

        public CalendarController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public void GetCalendar(int workoutPlanId)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetEnvironmentVariable("LocalAppData");
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    User.Identity.Name,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            var calendar = service.Calendars.Get("primary").Execute();
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();


            AddWorkoutPlanToCalendar(2, service);

        }

        public void AddWorkoutPlanToCalendar(int workoutPlanId, CalendarService service)
        {
            var workoutPlan =
                _unitOfWork.WorkoutPlanRepository.Get(x => x.Id == workoutPlanId, null, "WorkoutPlanWorkouts.Workout").First();
            var events = new List<Event>();
            foreach (var wpw in workoutPlan.WorkoutPlanWorkouts)
            {
                events.Add(new Event
                {
                    Summary = wpw.Workout.Name,
                    Location = "",
                    Start = new EventDateTime()
                    {
                        DateTime = wpw.ScheduledTime,
                        TimeZone = "Europe/Stockholm"
                    },
                    End = new EventDateTime()
                    {
                        DateTime = wpw.ScheduledTime.AddHours(1),
                        TimeZone = "Europe/Stockholm"
                    }
                });
            }
            foreach (var eventToAdd in events)
            {
                service.Events.Insert(eventToAdd, "primary").Execute();
            }

        }

    }
}
