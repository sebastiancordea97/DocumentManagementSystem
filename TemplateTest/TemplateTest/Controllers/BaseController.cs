using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DataAccessLibrary.Models;
using TemplateTest.Helpers;
using Microsoft.Extensions.Options;

namespace TemplateTest.Controllers
{

    public class BaseController : Controller
    {
        private int _userId = 0;
        private int _userRole = 0;
        private string _username;
        private int _departmentId = 0;

        protected readonly LicentaTestContext _dbContext;
        protected readonly AppConfiguration _configuration;

        public BaseController(LicentaTestContext databaseContext, IOptions<AppConfiguration> configuration)
        {
            _dbContext = databaseContext;
            _configuration = configuration.Value;
        }
        protected int UserId
        {
            get
            {
                if (_userId == 0)
                    _userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                return _userId;
            }
        }

        protected int UserRole
        {
            get
            {
                if (_userRole == 0)
                    _userRole = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);
                return _userRole;
            }
        }

        protected string Username
        {
            get
            {
                if (_username == null)
                    _username = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                return _username;
            }
        }

        protected int DepartmentId
        {
            get
            {
                if (_departmentId == 0)
                    _departmentId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "DepartmentId").Value);
                return _departmentId;
            }
        }

        protected void CreateSuccessMessage(string message)
        {
            TempData["Message"] = message;
            TempData["Type"] = "success";
        }

        protected void CreateErrorMessage(string message)
        {
            if (message != null)
                TempData["Message"] = message;
            CreateErrorMessage();
            TempData["Type"] = "danger";
        }

        private void CreateErrorMessage()
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    TempData["Message"] += error.ErrorMessage + " ";
                }
            }
        }
    }
}
