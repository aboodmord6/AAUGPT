using AAUGPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AAUGPT.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email already exists");
                    return View(model);
                }

                var user = new User
                {
                    Id = _nextId++,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    Username = model.Username
                };

                _users.Add(user);
                Session["UserId"] = user.Id;
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }



        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _users.FirstOrDefault(u => u.Email == model.Email);

                if (user != null && VerifyPassword(model.Password, user.Password))
                {
                    Session["UserId"] = user.Id;


                    ModelState.AddModelError("", "Invalid email or password");
                }

                return View(model);
            }
        }
    }
}
    


