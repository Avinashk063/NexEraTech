using Microsoft.AspNetCore.Mvc;
using NexEraTech.Application.Interface;
using NexEraTech.Domain.Models;
using NexEraTech.Infrastructure.Repository.Interface;

namespace NexEraTech.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly INexEraTechRepository _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IEmailService emailService, ILogger<HomeController> logger, INexEraTechRepository db)
        {
            _emailService = emailService;
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult BookAppointment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(Users booking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("ModelState valid. Processing appointment for: {FullName} ({Email})",
                        $"{booking.FirstName} {booking.LastName}", booking.Email);

                    // Format appointment details for email
                    var appointmentDetails = $@"
                    <p><strong>Name:</strong> {booking.FirstName} {booking.LastName}</p>
                    <p><strong>Email:</strong> {booking.Email}</p>
                    <p><strong>Phone:</strong> {booking.PhoneNumber}</p>
                    <p><strong>Company:</strong> {booking.CompanyName}</p>
                    <p><strong>Consultation Date:</strong> {booking.AppointmentDate:dddd, dd MMM yyyy h:mm tt}</p>
                    <p><strong>Project Description:</strong></p>
                    <p>{booking.ProjectDescription}</p>
                    ";

                    booking.AppointmentDate = DateTime.SpecifyKind(booking.AppointmentDate, DateTimeKind.Utc);

                    // Save appointment to the database
                    await _db.AddAsync(booking);
                    await _db.SaveAsync();

                    // Send email notification
                    await _emailService.SendAppointmentConfirmationAsync(booking, appointmentDetails);

                    _logger.LogInformation("Appointment successfully booked and emails sent for: {Email} at {Timestamp}",
                        booking.Email, DateTime.UtcNow);

                    TempData["Success"] = "Your appointment request has been received! Check your email for confirmation details.";
                    TempData["ConsultationDate"] = booking.AppointmentDate.ToString("dddd, dd MMM yyyy h:mm tt");
                    return RedirectToAction("AppointmentConfirmation");
                }
                else
                {
                    _logger.LogWarning("ModelState invalid for appointment booking. Errors: {ErrorCount}",
                        ViewData.ModelState.Values.SelectMany(v => v.Errors).Count());
                    return View(booking);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing appointment booking for {Email}: {ErrorMessage}",
                    booking.Email, ex.Message);
                ModelState.AddModelError("", $"There was an error processing your request: {ex.Message}");
                return View(booking);
            }
        }
        public IActionResult AppointmentConfirmation()
        {
            return View();
        }
    }
}
