﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using TandemBooking.Services;
using TandemBooking.ViewModels.BookingAdmin;

namespace TandemBooking.ViewComponents
{
    public class PilotSelector : ViewComponent
    {
        private BookingService _bookingService;

        public PilotSelector(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string controlName, DateTime date)
        {
            return View(new PilotSelectorViewModel
            {
                AvailablePilots = _bookingService.FindAvailablePilots(date, true),
                ControlName = controlName,
            });
        }
    }
}