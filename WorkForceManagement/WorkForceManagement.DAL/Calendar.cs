using System;
using System.Collections.Generic;

namespace WorkForceManagement.DAL
{
    public static class Calendar
    {
        public static List<DateTime> GenerateCalendar()
        {
            int year = DateTime.Now.Year;

            List<DateTime> calendar = new List<DateTime>()
            {
            new DateTime(year, 1, 1), //New Year's Day
            new DateTime(year, 1, 3), //New Year's Day(in lieu)
            new DateTime(year, 3, 3), //Liberation Day
            new DateTime(year, 4, 22), //Orthodox Good Friday
            new DateTime(year, 4, 23), //Orthodox Easter Saturday
            new DateTime(year, 4, 24), //Orthodox Easter Sunday 
            new DateTime(year, 4, 25), //Orthodox Easter Monday 
            new DateTime(year, 5, 1), //Labour Day
            new DateTime(year, 5, 2), //Labour Day Holiday
            new DateTime(year, 5, 6), //Saint George's Day / Army Day
            new DateTime(year, 5, 24), //Culture and Literacy Day
            new DateTime(year, 9, 6), //Unification Day
            new DateTime(year, 9, 22), //Independence Day
            new DateTime(year, 12, 24), //Christmas Eve
            new DateTime(year, 12, 25), //Christmas Day
            new DateTime(year, 12, 26), //2nd Day of Christmas
            new DateTime(year, 12, 27), //Christmas Holiday(in lieu)
            new DateTime(year, 12, 28) //Christmas Holiday(in lieu)
            };

            return calendar;
        }
    }
}
