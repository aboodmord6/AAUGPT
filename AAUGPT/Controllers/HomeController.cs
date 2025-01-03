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

                var verificationToken = GenerateToken();
                var user = new User
                {
                    Id = _nextId++,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    Username = model.Username,
                    IsEmailVerified = false,
                    VerificationToken = verificationToken
                };

                _users.Add(user);

                try
                {
                    _emailService.SendVerificationEmail(user.Email, verificationToken);
                    TempData["Message"] = "Registration successful! Please check your email to verify your account.";
                    return RedirectToAction("Login");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Error sending verification email. Please try again.");
                    _users.Remove(user);
                    return View(model);
                }
            }

            return View(model);
        }

        public ActionResult VerifyEmail(string token)
        {
            var user = _users.FirstOrDefault(u => u.VerificationToken == token);
            if (user != null)
            {
                user.IsEmailVerified = true;
                user.VerificationToken = null;
                TempData["Message"] = "Email verified successfully! You can now login.";
            }
            else
            {
                TempData["Error"] = "Invalid verification token.";
            }

            return RedirectToAction("Login");
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
                    if (!user.IsEmailVerified)
                    {
                        ModelState.AddModelError("", "Please verify your email before logging in.");
                        return View(model);
                    }

                    Session["UserId"] = user.Id;

                    if (model.RememberMe)
                    {
                        var rememberMeToken = GenerateToken();
                        user.RememberMeToken = rememberMeToken;

                        var cookie = new HttpCookie("RememberMe", rememberMeToken)
                        {
                            Expires = DateTime.Now.AddDays(30)
                        };
                        Response.Cookies.Add(cookie);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }


    }
}
    


