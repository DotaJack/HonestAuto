using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HonestAutoDBTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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