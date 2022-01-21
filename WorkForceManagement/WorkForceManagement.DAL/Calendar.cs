using System;
using System.Collections.Generic;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace WorkForceManagement.DAL
{
    public static class Calendar
    {
        public static List<DateTime> GenerateCalendar()
        {
            List<DateTime> calendar = new List<DateTime>();
            int year = DateTime.Now.Year;

            const string ApiKey = "AIzaSyBHsIolzVysJEA57h5YiaSAQ070D8XZSFw";
            const string CalendarId = "en.bulgarian.official#holiday@group.v.calendar.google.com";

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey,
                ApplicationName = "BG Calendar"
            });

            var request = service.Events.List(CalendarId);
            request.Fields = "items(summary,start,end)";
            var response = request.ExecuteAsync().Result;

            foreach (var item in response.Items)
            {
                DateTime Holiday = DateTime.Parse(item.Start.Date);
                if (Holiday.Year == year && !calendar.Contains(Holiday))//add holidays from the curr year only and ignore duplicate holidays
                    calendar.Add(Holiday);
            }
            return calendar;
        }
    }
}
