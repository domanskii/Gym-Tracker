using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GymTracker.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Szczegoly1()
        {
            return View("Szczegoly1");
        }

        public IActionResult Szczegoly2()
        {
            return View("Szczegoly2");
        }

        public IActionResult Szczegoly3()
        {
            return View("Szczegoly3");
        }
        public IActionResult Szczegoly4()
        {
            return View("Szczegoly4");
        }
        public IActionResult Szczegoly5()
        {
            return View("Szczegoly5"); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
