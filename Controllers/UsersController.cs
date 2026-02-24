using Microsoft.AspNetCore.Mvc;
using HelpdeskApp.Data;
using HelpdeskApp.Models;

namespace HelpdeskApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly UserDb _userDb;

        public UsersController(UserDb userDb)
        {
            _userDb = userDb;
        }

        public IActionResult Index()
        {
            var users = _userDb.GetAllUsers();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            bool created = _userDb.CreateUser(user);
            if (!created)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(user);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleActive(int id)
        {
            _userDb.ToggleUserActive(id);
            return RedirectToAction("Index");
        }
    }
}