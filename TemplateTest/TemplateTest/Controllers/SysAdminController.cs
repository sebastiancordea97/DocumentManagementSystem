
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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TemplateTest.Controllers
{
    [Authorize(Roles = "1")]
    public class SysAdminController : BaseController
    {
        private IWebHostEnvironment _webHostEnvironment;
        public SysAdminController(LicentaTestContext context, IOptions<AppConfiguration> configuration, IWebHostEnvironment environment)
            : base(context, configuration)
        {
            _webHostEnvironment = environment;
        }



        [HttpGet]
        public async Task<IActionResult> Admin([FromQuery]int id = 0)
        {
            List<Department> departments = _dbContext.Department
                                                 .Include(x => x.Users)
                                                 .ToList();



            List<Users> users = null;

            users = await _dbContext.Users
                .Include(x => x.DepartmentNavigation)
                .Include(x => x.Role)
                .ToListAsync();

            if (id > 0)
                users = users.Where(x => x.DepartmentId == id).ToList();

            ViewData["Departments"] = await _dbContext.Department.ToListAsync();
            ViewData["UserTypes"] = await _dbContext.UserRole.ToListAsync();
            ViewData["Documents"] = await _dbContext.Documents
                                    .Include(x => x.User).ToListAsync();

            Tuple<List<Department>, List<Users>> tuple = new Tuple<List<Department>, List<Users>>(departments, users);

            return View(tuple);
        }
        #region departments

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDepartment(string departmentName)
        {
            if (departmentName != null)
            {
                await _dbContext.Department.AddAsync(new Department
                {
                    DepName = departmentName,

                });
                await _dbContext.SaveChangesAsync();
                TempData["Message"] = "You succesfully added a new department!";
                TempData["Type"] = "success";
            }
            else
            {
                TempData["Message"] = "The data you choose to add is not recognized!";
                TempData["Type"] = "danger";
            }
            return Redirect("Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDepartment(string departmentName, string departmentId, int? supervisorId)
        {
            if (string.IsNullOrEmpty(departmentName))
            {
                TempData["Message"] = "The department name must have a value!";
                TempData["Type"] = "danger";
            }
            else if (string.IsNullOrEmpty(departmentId))

            {
                TempData["Message"] = "There was an error while trying to resolve the request. Please try again later!";
                TempData["Type"] = "danger";
            }
            else
            {
                var department = await _dbContext.Department.FirstOrDefaultAsync(x => x.DepId == Int32.Parse(departmentId));
                department.DepName = departmentName;
                department.Supervisor = supervisorId;



                _dbContext.Department.Update(department);
                await _dbContext.SaveChangesAsync();
                TempData["Message"] = "You succesfully updated the department " + departmentName + "!";
                TempData["Type"] = "success";
            }
            return Redirect("Admin");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDepartment([FromQuery]int id = 0)
        {
            if (id > 0)
            {
                var department = await _dbContext.Department.FirstOrDefaultAsync(x => x.DepId == id);
                if (department != null)
                {
                    _dbContext.Department.Remove(department);
                    await _dbContext.SaveChangesAsync();
                    TempData["Message"] = "You succesfully removed the department " + department.DepName + "!";
                    TempData["Type"] = "success";
                }
                else
                {
                    TempData["Message"] = "The department you requested could not be found!";
                    TempData["Type"] = "danger";
                }
            }
            else
            {
                TempData["Message"] = "Your request could not be done!";
                TempData["Type"] = "danger";
            }
            return Redirect("Admin");
        }
        #endregion

        #region users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception();
                if (string.IsNullOrEmpty(user.Password))
                    throw new Exception("The password is required!");
                if (user.DepartmentId == 0 || user.RoleId == 0)
                    throw new Exception("Both department and user type must be selected!");
                var salt = Utils.CreateSalt();
                var passHash = Utils.MakePasswordHash(user.Password,salt);
                await _dbContext.Users.AddAsync(new Users()
                {
                    Lastname = user.Lastname,
                    Email = user.Email,
                    PasswordHash = passHash,
                    DepartmentId = user.DepartmentId,
                    RoleId = user.RoleId,
                    Firstname = user.Firstname,
                    UserFunction = user.Userfunction,
                    Userrank = user.Userrank,
                    Salt = salt,


                }); ;
                await _dbContext.SaveChangesAsync();
                CreateSuccessMessage("You have succesfully added a new user: " + user.Lastname);
            }
            catch (Exception e)
            {
                CreateErrorMessage(e.Message);
            }
            return Redirect("Admin");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception();
                var dbUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
                dbUser.Lastname = user.Lastname;
                dbUser.Firstname = user.Firstname;
                dbUser.UserFunction = user.Userfunction;
                dbUser.Userrank = user.Userrank;
                dbUser.Email = user.Email;
                dbUser.DepartmentId = user.DepartmentId;


                dbUser.RoleId = user.RoleId;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    var salt = Utils.CreateSalt();
                    var passHash = Utils.MakePasswordHash(user.Password, salt); 
                    dbUser.PasswordHash = passHash;

                }
                _dbContext.Users.Update(dbUser);
                await _dbContext.SaveChangesAsync();
                CreateSuccessMessage("You have succesfully edited the user: " + user.Lastname);
            }
            catch (Exception e)
            {
                CreateErrorMessage(e.Message);
            }
            return Redirect("Admin");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser([FromQuery]int id = 0)
        {
            try
            {
                if (id <= 0)
                    throw new Exception("Your request could not be handled!");
                var user = await _dbContext.Users.FirstAsync(x => x.UserId == id);
                if (user == null)
                    throw new Exception("The user does not exist!");
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                CreateSuccessMessage("You succesfully removed the user " + user.Lastname + "!");
            }
            catch (Exception e)
            {
                CreateErrorMessage(e.Message);
            }
            return Redirect("Admin");

        }
        #endregion
        public IActionResult GetDocument(string filePath)
        {
           
            var getPath = Path.Combine(_webHostEnvironment.WebRootPath, Path.GetFullPath(filePath));
            var filename = Path.GetFileName(getPath);

            return PhysicalFile(getPath, "Application/pdf", filename);

        }
    }
}

