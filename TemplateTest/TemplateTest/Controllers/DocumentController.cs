using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using TemplateTest.Models;
using TemplateTest.Helpers;
using Microsoft.Extensions.Options;

namespace TemplateTest.Controllers
{
    

   
    public class DocumentController : BaseController
    {


        public DocumentController(LicentaTestContext databaseContext, IOptions<AppConfiguration> configuration)
            : base(databaseContext, configuration)
        {

        }



        [HttpGet]
        public IActionResult Index()
        {

            //if (id == 0)
            //    id = UserId;
            //List<Document> documents = null;


            return View();
        }

       



    }
}