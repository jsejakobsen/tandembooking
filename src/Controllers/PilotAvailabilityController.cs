using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TandemBooking.Models;
using TandemBooking.ViewModels;
using TandemBooking.Services;

namespace TandemBooking.Controllers
{
    [Authorize]
    public class PilotAvailabilityController : Controller
    {
        private static string[] _monthNames = new[]
        {
            "Januar",
            "Februar",
            "Mars",
            "April",
            "Mai",
            "Juni",
            "Juli",
            "August",
            "September",
            "Oktober",
            "November",
            "Desember"
        };

        private readonly TandemBookingContext _context;
        private readonly UserManager _userManager;

        public PilotAvailabilityController(TandemBookingContext context, UserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ActionResult Index(DateTime? date = null, string userId=null)
        {
            if (date == null)
            {
                date = DateTime.UtcNow;
            }

            if (userId == null || !User.IsAdmin())
            {
                userId = _userManager.GetUserId(User);
            }
            var user = _context.Users.Single(u => u.Id == userId);

            var startDate = new DateTime(date.Value.Year, date.Value.Month, 1);
            var endDate = startDate.AddMonths(1);

            var nextMonth = startDate.AddMonths(1);
            var prevMonth = startDate.AddMonths(-1);

            int startWeekDay = ((int) startDate.DayOfWeek - 1 + 7)%7;
            startDate = startDate.AddDays(-startWeekDay);

            int endWeekDay = ((int) endDate.DayOfWeek - 1 + 7)%7;
            endDate = endDate.AddDays(6 - endWeekDay);

            var availabilities = _context.PilotAvailabilities
                .Where(a => a.Pilot.Id == userId)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .OrderBy(a => a.Date)
                .ToList();

            return View(new PilotAvailabilityViewModel()
            {
                Next = nextMonth,
                Prev = prevMonth,
                MonthName = $"{_monthNames[date.Value.Month - 1]} {date.Value.Year}",
                StartDate = startDate,
                EndDate = endDate,
                Availabilities = availabilities,
                Pilot = user,
            });
        }

        public ActionResult SetAvailability(DateTime date, bool available, string userId = null)
        {
            if (userId == null || !User.IsAdmin())
            {
                userId = _userManager.GetUserId(User);
            }

            if (available)
            {
                var pilotAvailability = new PilotAvailability()
                {
                    Date = date.Date,
                    Pilot = _context.Users.Single(u => u.Id == userId)
                };
                _context.PilotAvailabilities.Add(pilotAvailability);
            }
            else
            {
                var availabilities = _context.PilotAvailabilities
                    .Where(a => a.Pilot.Id == userId)
                    .Where(a => a.Date >= date && a.Date < date.AddDays(1))
                    .ToList();

                foreach (var pilotAvailability in availabilities)
                {
                    _context.PilotAvailabilities.Remove(pilotAvailability);
                }
            }
            _context.SaveChanges();

            return Json(new {result = "ok"});
        }

    }

}