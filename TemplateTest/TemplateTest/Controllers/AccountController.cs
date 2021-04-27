using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DataAccessLibrary.Models;
using TemplateTest.Helpers;
using TemplateTest.Models;
using Microsoft.Extensions.Options;

namespace TemplateTest.Controllers
{
    public class AccountController : BaseController
    {
     
        public AccountController(LicentaTestContext databaseContext, IOptions<AppConfiguration> configuration)
                : base(databaseContext, configuration)
        { }

        [HttpGet]
        public IActionResult Login([FromQuery]string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return Logout();

            ViewData["Message"] = "Please fill all the fields!";
            ViewData["Error"] = false;
            ViewData["returnUrl"] = returnUrl;
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Login(LoginViewModel inputData, [FromQuery]string returnUrl)
        {


            if(ModelState.IsValid)
            {
                var user = AuthenticateUser(inputData.Email, inputData.Password);

                if(user != null)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                        new Claim(ClaimTypes.Name, user.Lastname),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("DepartmentId", user.DepartmentId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authenticationProps = new AuthenticationProperties
                    {
                        IsPersistent = inputData.IsPersistent
                    };

                   await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),authenticationProps);




                }
                else
                {
                    ViewData["Message"] = "The credentials you have entered are invalid! Please Doublecheck!";
                    ViewData["Error"] = true;
                    return View();
                }


            }
            else
            {

                ViewData["Message"] = "Please fill all the fields!";
                ViewData["Error"] = true;

                return View();

            }


            if (returnUrl == null)
                return LocalRedirect("/Home/Index");
            else
                return LocalRedirect(returnUrl);
                    
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Home/Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["Message"] = "You are not authorized to this page!!!";
            TempData["Type"] = "Danger";
            return RedirectToAction("Index", "Home");

        }



        /// <summary>
        /// This method is used to retrieve a user from the database based on its credentials.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns></returns>
        private Users AuthenticateUser(string email, string password)
        {

            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
                return user;
            
            
                var userSalt = _dbContext.Users.FirstOrDefault(x => x.Email == email);

                byte[] passHash = Utils.MakePasswordHash(password, userSalt.Salt);

                var user2 = _dbContext.Users.FirstOrDefault(x => x.Email == email && x.PasswordHash == passHash);

            



            return user2;
        }


    }
}