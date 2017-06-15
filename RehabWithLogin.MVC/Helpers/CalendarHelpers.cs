using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace RehabWithLogin.MVC.Helpers
{
    public static class CalendarHelpers
    {
        private static readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private static readonly string ApplicationName = "Rehab";
        public static CalendarService GetService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = Environment.GetEnvironmentVariable("LocalAppData");
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true))
                    .Result;
            }
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            return service;
        }

        public static List<EventReminder> GetReminderList()
        {
            var reminder = new EventReminder
            {
                Method = "popup",
                Minutes = 30
            };
            var reminderList = new List<EventReminder> { reminder };
            return reminderList;
        }
    }
}
