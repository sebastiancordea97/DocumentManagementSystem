using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using DataAccessLibrary.Models;
using TemplateTest.Models;
using Microsoft.Extensions.Options;
using TemplateTest.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace TemplateTest.Controllers
{

    [Authorize]
    public class HomeController : BaseController
    {
       
        public HomeController(LicentaTestContext databaseContext, IOptions<AppConfiguration> configuration)
          : base(databaseContext, configuration)
        {

        }

        public IActionResult Index()
        {
            return UserRole switch
            {

                Models.UserRole.SysAdmin => LocalRedirect("/SysAdmin/Admin"),
                Models.UserRole.User => LocalRedirect("/User/Index"),
          
                _ => View(),
            };
        }

    }



}
