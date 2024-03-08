using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HonestAutoDBTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            // Action method for rendering the "Index" view
            return View();
        }

        public IActionResult Contact()
        {
            // Action method for rendering the "Contact" view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact([Bind("Name", "Subject", "Email", "Message")] ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Email to the mechanic
                var emailSubject = "Mechanic Application Received";
                var emailContent = $@"
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <title>Mechanic Application Received</title>
                    <style>
                        /* CSS styles */
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""content"">
                            <h2>Hello {model.Name},</h2>
                            <p>We have received your application to join Honest Auto as a mechanic. Our team will review your application shortly.</p>
                            <p>If you have any questions or need further assistance, feel free to contact us.</p>
                        </div>
                        <div class=""footer"">
                            <p>Thank you for considering Honest Auto!</p>
                            <img src=""https://i.ibb.co/m6cXxB1/Honest-Auto-Logo.png"" alt=""Honest Auto Logo"" class=""logo"" />

                        </div>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(model.Email, emailSubject, emailContent, true);

                // Email to HonestAutoLTD
                var emailSubject2 = "Mechanic Application Received";
                var emailContent2 = $@"
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <title>Mechanic Application Received</title>
                    <style>
                        /* CSS styles */
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""content"">
                            <h2>Hello HonestAutoLTD,</h2>
                            <p>A new mechanic application has been received:</p>
                            <p><strong>Name:</strong> {model.Name}</p>
                            <p><strong>Email:</strong> {model.Email}</p>
                        </div>
                        <div class=""footer"">
                            <p>Thank you for considering Honest Auto!</p>
                            <img src=""https://i.ibb.co/m6cXxB1/Honest-Auto-Logo.png"" alt=""Honest Auto Logo"" class=""logo"" />
                            <img src=""https://i.ibb.co/6RzfjLM/Honest-Auto-Type.png"" alt=""Honest Auto Type"" class=""type"" />
                        </div>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync("HonestAutoLTD@gmail.com", emailSubject2, emailContent2, true);

                // Redirect to a confirmation page or wherever appropriate
                return RedirectToAction("ContactConfirmation");
            }

            // If model state is not valid, return the view with validation errors
            return View(model);
        }

        public IActionResult ContactConfirmation()
        {
            // Action method for rendering the "ContactConfirmation" view
            return View();
        }

        public IActionResult Privacy()
        {
            // Action method for rendering the "Privacy" view
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Action method for rendering the "Error" view with error details
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}