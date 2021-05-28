using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;
        private readonly IAccountRepository _accountRepository;

        public HomeController(ILogger<HomeController> logger, INationalParkRepository npRepo, ITrailRepository trailRepo, IAccountRepository accountRepository)
        {
            _logger = logger;
            _npRepo = npRepo;
            _trailRepo = trailRepo;
            _accountRepository = accountRepository;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM listOfParkAndTrails = new IndexVM()
            {
                NationalParksList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath),
                TrailsList = await _trailRepo.GetAllAsync(SD.TrailAPIPath),
            };
            return View(listOfParkAndTrails);
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

        [HttpGet]
        public IActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User objUser)
        {
            var user = await _accountRepository.LoginAsync(SD.AccountAPIPath + "authenticate/", objUser);
            if (objUser.Token == null)
            {
                // Error
                return View();
            }
            HttpContext.Session.SetString("JWToken",user.Token);
            return RedirectToAction("~/Home/Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User objUser)
        {
            bool user = await _accountRepository.RegisterAsync(SD.AccountAPIPath + "register/", objUser);
            if (user == false)
            {
                // Error
                return View();
            }
            return RedirectToAction("~/Home/Login");
        }

        public async Task<IActionResult> LogoutAsync()
        {
            HttpContext.Session.SetString("JWToken", "");
            return RedirectToAction("~/Home/Index");
        }
    }
}
