using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using UserModule.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace UserModule.Controllers
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

            //var user = new User() { EmployeeId = 1, Username = "Admin", Password = "123", Email = "admin@gmail.com" };
            
            //Set the value into a session key
           // HttpContext.Session.SetString("UserSession", JsonConvert.SerializeObject(user));
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}