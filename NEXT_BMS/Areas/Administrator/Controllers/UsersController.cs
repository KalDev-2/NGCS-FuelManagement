using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Data;
using NEXT_BMS.Models;
using NEXT_BMS.Utilities;
using hu_utils;
using NEXT_BMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using DocumentFormat.OpenXml.Spreadsheet;


namespace NEXT_BMS.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class UsersController :Controller
    {
        private readonly NEXT_BMSContext _context;
        private readonly UserManagement _um;
        private readonly CurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpContext;
        public UsersController(NEXT_BMSContext context, UserManagement um, CurrentUser currentUser, IHttpContextAccessor httpContext)
        {
            _context = context;
            _um = um;
            _currentUser = currentUser;
            _httpContext = httpContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(string firstName, string middleName, string lastName, string phoneNumber, string email, string userName, string password, int genderId, int languageId)
        {

            var user = new User
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                UserName = userName,
                GenderId = genderId,
                LanguageId = languageId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                FailureCount = 0,
                LastLogin = DateTime.UtcNow,
                BlockEndDate = DateTime.MinValue,
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateAccount", "Account");
        }



        [HttpPost]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var returnData = (from manudata in _context.Users
                                  .Include(g => g.UserOrganizationUsers).ThenInclude(g => g.Organization)
                                  .Where(x =>  x.IsDeleted == false)
                                  .Select(x => new
                                  {
                                      x.Id,
                                      x.UserName,
                                      x.Email,
                                      x.PhoneNumber,
                                      EmployeeName= x.FirstName,
                                      x.IsActive
                                  })
                                  select manudata);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    returnData = returnData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    returnData = returnData.Where(m =>
                       m.UserName.Contains(searchValue)
                    || m.PhoneNumber.Contains(searchValue) 
                    || m.Email.Contains(searchValue)
                    );
                }
                recordsTotal = returnData.Count();
                var data =await returnData.Skip(skip).Take(pageSize).ToListAsync();
                var jsonData = new { draw, recordsFiltered = recordsTotal, recordsTotal, data };
                return  Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            User user = _context.Users
                .Include(z => z.UserOrganizationUsers).ThenInclude(i => i.Organization)
                .Include(z => z.UserOrganizationUsers)
                .Include(x=>x.UserRoles).ThenInclude(x=>x.Role)
                .FirstOrDefault(x=>x.Id==id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Organizations = _context.Organizations
                .Where(o => !o.IsDeleted)
                .ToList();


            ViewBag.RoleId = _context.Roles.ToList().Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            ViewBag.OrganizationId = _context.Organizations.ToList().Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });

            //ViewBag.DocumentTypeId = _context.DocumentTypes.ToList().Select(x => new SelectListItem()
            //{
            //    Value = x.Id.ToString(),
            //    Text = x.Name
            //});


            //ViewBag.BranchId = _context.Branches.ToList().Select(x => new SelectListItem()
            //{
            //    Value = x.Id.ToString(),
            //    Text = x.Name
            //});

            //ViewBag.DepartmentId = _context.BranchDepartments.ToList().Select(x => new SelectListItem()
            //{
            //    Value = x.Id.ToString(),
            //    Text = x.Department.Name+"("+x.Branch.Name+")"
            //});

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            User user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var users = _context.Users.Where(x => x.IsActive).Select(s => new
            {
                s.Id,
                Name = $"{s.FirstName} {s.LastName}"
            });

            
            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("Id,GenderId,,FirstName,MiddleName,LastName,PhoneNumber,Email,UserName,Password,CreatedDate,LastLogon,FailureCount,BlockEndDate,DefaultLanguageId,IsActive,IsDeleted")] User user)
        {
			if (ModelState.IsValid)
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
				TempData["Success"] = "OSC";
                return RedirectToAction("Index");
            }
 
            TempData["Error"] = "erroroccured";
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            User user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = _context.Users.Find(id);
            user.IsActive = false;
            user.IsDeleted = true;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["Success"] = "OSC";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
		
		public void export()
        {
            var user = new DataTable("FileData");
			user.Columns.Add("EmployeeId", typeof(string));
			user.Columns.Add("MobileNumber", typeof(string));
			user.Columns.Add("Email", typeof(string));
			user.Columns.Add("UserName", typeof(string));
			user.Columns.Add("Password", typeof(string));
			user.Columns.Add("CreatedDate", typeof(string));
			user.Columns.Add("LastLogin", typeof(string));
			user.Columns.Add("FailureCount", typeof(string));
			user.Columns.Add("BlockEndDate", typeof(string));
			user.Columns.Add("LanguageId", typeof(string));
			user.Columns.Add("IsActive", typeof(string));
			user.Columns.Add("IsDeleted", typeof(string));
			user.Columns.Add("Employee", typeof(string));
			user.Columns.Add("UserRoles", typeof(string));
            foreach (var items in _context.Users)
            {
                DataRow dr = user.NewRow();
				dr.SetField(0, items.Gender);
				dr.SetField(1, items.PhoneNumber);
				dr.SetField(2, items.Email);
				dr.SetField(3, items.UserName);
				dr.SetField(4, items.Password);
				dr.SetField(5, items.CreatedDate);
				dr.SetField(6, items.LastLogin);
				dr.SetField(7, items.FailureCount);
				dr.SetField(8, items.BlockEndDate);
				dr.SetField(9, items.LanguageId);
				dr.SetField(10, items.IsActive);
				dr.SetField(11, items.IsDeleted);
				dr.SetField(13, items.UserRoles);
                user.Rows.Add(dr);
            }
            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            var wc = ws.Cell(1, 1).InsertTable(user);
            wc.ShowAutoFilter = false;
            ws.Rows().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AppendTrailer("content-disposition", String.Format(@"attachment;filename={0}.xlsx", "user"));
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    wb.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response);
            //    memoryStream.Close();
            //}
            //Response.en''();
        }

        public ActionResult Block(int Id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Home");
            }
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var user = _context.Users.Find(Id);
            user.IsActive = false;
            //user.ActionBy = userId;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["Success"] = "Operation successfully completed.";
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Activate(int Id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Home");
            }
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var user = _context.Users.Find(Id);
            user.IsActive = true;
            //user.ActionBy = userId;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
           // BackgroundJob.Enqueue(() => Models.Mailer.ConfirmUser(user.Id));
            TempData["Success"] = "Operation successfully completed.";
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveRoles(int userId, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();
                foreach (var userRole in userRoles)
                {
                    userRole.IsActive= false;
                    userRole.IsDeleted= true;
                    _context.Update(userRole);
                }
                _context.SaveChanges();
                if (selectedRoles != null)
                {
                    foreach (string role in selectedRoles)
                    {
                        int roleId= int.Parse(role);
                        var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId==roleId);
                        if (userRole!=null)
                        {
                            userRole.IsActive= true;
                            userRole.IsDeleted= false;
                            _context.Update(userRole);
                        }
                        else
                        {
                            userRole = new UserRole();
                            userRole.RoleId = roleId;
                            userRole.UserId = userId;
                            userRole.IsActive = true;
                            if (!_context.UserRoles.Any(x => x.IsDefault && x.UserId == userId))
                            {
                                userRole.IsDefault = true;
                            }
                        }
                        _context.UserRoles.Add(userRole);
                    }
                    _context.SaveChanges();
                }
                TempData["Success"] = "Operation completed successfully.";
            }
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult saveOrganztions(int userId, int organizationId)
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (!sessionUserId.HasValue)
            {
                TempData["error"] = "Session expired. Please login again.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var userOrg = _context.UserOrganizations
                .FirstOrDefault(ur => ur.UserId == userId && ur.OrganizationId == organizationId && ur.IsDeleted == false);

            if (userOrg == null)
            {
                var newUserOrg = new UserOrganization
                {
                    OrganizationId = organizationId,
                    UserId = userId,
                    IsActive = true,
                    IsDeleted = false,
                    AssignedDate = DateTime.Now,
                    ActionBy = sessionUserId.Value
                };

                _context.UserOrganizations.Add(newUserOrg);
                _context.SaveChanges();

                TempData["Success"] = "Organization assigned successfully.";
            }
            else
            {
                TempData["info"] = "User is already assigned to this organization.";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        //updae userOrganization by changing IsActive and IsDeleted


        [HttpPost]
        public JsonResult updateOrganiztions(int userId, int organizationId)
        {
            try
            {
                var userOrg = _context.UserOrganizations
                    .FirstOrDefault(ur => ur.UserId == userId && ur.OrganizationId == organizationId && ur.IsDeleted == false);

                if (userOrg != null)
                {
                    userOrg.IsActive = false;
                    userOrg.IsDeleted = true;
                    userOrg.AssignedDate = DateTime.Now;
                    _context.UserOrganizations.Update(userOrg);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Organization unassigned successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "User is not assigned to this organization." });
                }
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }



        public ActionResult SelectApplication(int id)
        {
            var app = _context.Applications.Include(a => a.MenuCategories)
                .FirstOrDefault(x => x.Id == id);
            _um.resetApplication(app.Id);
            if (app.Url != null)
            {
                return Redirect(app.Url);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
