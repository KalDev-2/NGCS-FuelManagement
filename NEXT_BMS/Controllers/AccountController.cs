using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Data;
using hu_utils;
using NEXT_BMS.Models;
using NEXT_BMS.Utilities;
using NEXT_BMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
namespace NEXT_BMS.Controllers

{

    public class AccountController : Controller
    {
        private readonly NEXT_BMSContext _context;
        private readonly UserManagement _um;
        private readonly CurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpContext;
        public AccountController(NEXT_BMSContext context, UserManagement um, CurrentUser currentUser, IHttpContextAccessor httpContext)
        {
            _context = context;
            _um = um;
            _currentUser = currentUser;
            _httpContext = httpContext;
        }

        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult GuestLogIn()
        {
            return View();
        }

        
        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }


        public IActionResult CreateAccount()
        {
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name");
            ViewData["LanguageId"] = new SelectList(_context.Languages, "Id", "Name");

            return View();

        }

        [HttpPost]
        public IActionResult CreateAccount(string firstName, string middleName, string phoneNumber, string email, string lastName, int genderId, string userName, string password)
        {
            
            try
            {

                var newUser = new User
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                UserName = userName,
                Password = password,
                LanguageId = 1,
                Email = email,
                PhoneNumber = phoneNumber,
                GenderId = genderId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false,

            };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                encryptPassword(newUser.Id, newUser.Password);

                TempData["Success"] = " you have successfully registered!!";
                return RedirectToAction("Login");
               
            }

            catch (Exception ex)
            {
                TempData["Error"] = "Not registered.";
                return View();
            }
 
        }
        


        private void encryptPassword(int userId, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            user.Password = Password._one_way_encrypt(password, userId);
            _context.Update(user);
            _context.SaveChanges();
        }
        [HttpPost]
        public ActionResult LogIn(LoginViewModel login, string Headedto, string FingerPrint, string Browser, string Platform, string TimeZone, string UserAgent)
        {
           
            if (Headedto != null) TempData["Headedto"] = Headedto;
            var users = _context.Users
                .Where(x => x.UserName == login.UserName)
                .Include(x => x.UserRoles);
            var user = users.FirstOrDefault();
           

            if (user != null)
            { 

                if (user.BlockEndDate > DateTime.Now)
                {
                    TempData["Error"] = "Your account has been blocked until " + user.BlockEndDate + " due to multiple login failure.";
                    return RedirectToAction("LogIn", "Account");
                }

                if (users.Count() > 1)
                {
                    user.UserName = user.UserName + user.LastName.Substring(0, 1);
                    _context.Update(user);
                    _context.SaveChanges();
                    //BackgroundJob.Enqueue(() => Mailer.ConfirmUser(user.Id));
                }

                if (user.Password == Password._one_way_encrypt(login.Password, user.Id))
                {
                    if (!user.IsActive && user.LastLogin == null)
                    {
                        TempData["Info"] = "Your account is not activated yet. Please contact the system administrator.";
                        return RedirectToAction("Login", "Home");
                    }
                    if (!user.IsActive)
                    {
                        TempData["Error"] = "Your account has been blocked. Please contact the system administrator.";
                        return RedirectToAction("LogIn", "Account");
                    }
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    user.FailureCount = 0;
                    user.LastLogin = DateTime.Now;
                    _context.Update(user);
                    _context.SaveChanges();

                    // -------------UserLogons-------------
                    _um.saveUserLogons(user, FingerPrint, Browser, Platform, TimeZone, UserAgent);
                    // -------------User Basic info-------------
                    _um.setUserBasicInfo(user);
                    // -------------User Image-------------
                    //_um.setUserImage(user);
                    // -------------User Department-------------
                    //  _um.SetDepartmentInfo(user.Employee);
                    // -------------User Applications-------------
                    _um.loadApplications(user.Id);
                    // -------------User Roles-------------
                    _um.LoadUserRole(user.Id);
                    // -------------User Stores-------------
                    //_um.setUserStores(user.EmployeeId);
                    // -------------Language-------------
                    _um.populateLanguage();
                   // _um.setUserOrganizations(user.Id);
                    if (Headedto != null) return Redirect(Headedto);
                    return RedirectToAction("Index", "Home", new {  Area = "Administrator" });
                }
                else
                {

                    if (user.FailureCount == 5)
                    {
                        user.BlockEndDate = DateTime.Now.AddMinutes(15);
                        _context.Entry(user).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (user.FailureCount == 4)
                        {
                            //BackgroundJob.Enqueue(() => Mailer.crackAttempt(user.Id));
                        }
                        user.FailureCount += 1;
                        _context.Entry(user).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
            TempData["Error"] = "Invalid User name or password";
            return RedirectToAction("LogIn", "Account");
        }

       

        public ActionResult ResendPasswordToEmail(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            string password_reset_code = Password._get_password_reset_code(6);
            var userPasswordReset = _context.UserPasswordResets.Include(x => x.User).FirstOrDefault(x => x.UserId == id && x.Validated == false && x.ExpiryDate > DateTime.Now);

            if (userPasswordReset != null)
            {
                //BackgroundJob.Enqueue(() => Mailer.ConfirmUser(userPasswordReset.Id));
            }
            else
            {
                userPasswordReset = new UserPasswordReset
                {
                    UserId = id.Value,
                    Validated = false,
                    VerificationCode = password_reset_code,
                    Token = Password._one_way_encrypt(password_reset_code, id.Value),
                    ExpiryDate = DateTime.Now.AddHours(48)
                };
                _context.UserPasswordResets.Add(userPasswordReset);
                _context.SaveChanges();
                // BackgroundJob.Enqueue(() => Mailer.ConfirmUser(userPasswordReset.Id));
            }
            TempData["Success"] = "Reset code has been sent to the user email. " + userPasswordReset.User.Email;
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(PasswordResetViewModel prvm)
        {
            if (!string.IsNullOrEmpty(prvm.VerificationCode))
            {
                var USrPR = _context.UserPasswordResets.Find(prvm.uprId);
                if (USrPR.Validated == false)
                {
                    if (USrPR.VerificationCode == prvm.VerificationCode)
                    {
                        USrPR.Validated = true;
                        _context.Entry(USrPR).State = EntityState.Modified;
                        var user = USrPR.User;
                        ChangePasswordViewModel cpvm = new ChangePasswordViewModel
                        {
                            OldPassword = user.Password,
                            Password = prvm.Password,
                            ConfirmPassword = prvm.ConfirmPassword
                        };
                        savechangedPassword(cpvm, user);
                        _context.SaveChanges();
                    }
                    else
                    {
                        TempData["Error"] = "Invalid reset code. Please check your email and try again.";
                        return RedirectToAction("ResetPassword", "Home", new { id = prvm.Token });
                    }
                }
            }
            TempData["Success"] = "Successfully changed your password. You can login now with your new password.";
            return RedirectToAction("Login", "Home");
        }

        public ActionResult ForgotPassword()
        {
            return RedirectToAction("ForgotPassword", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {
            if (_context.Users.Any(u => u.Email == Email && u.IsActive == true && u.IsDeleted == false))
            {
                var userId = _context.Users.FirstOrDefault(u => u.Email == Email && u.IsActive == true && u.IsDeleted == false).Id;
                ResendPasswordToEmail(userId);
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "The information you provided does not exist.";
            return RedirectToAction("ForgotPassword", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("LogIn", "Home");
            }
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var user = _context.Users.Find(userId);

            if (user.Password == hu_utils.Password._one_way_encrypt(changePassword.OldPassword, user.Id))
            {
                savechangedPassword(changePassword, user);

                return RedirectToAction("ChangePassword", "Home");
            }
            TempData["Error"] = "The old password is incorrect please try again.";
            return RedirectToAction("ChangePassword", "Home");
        }

        private void savechangedPassword(ChangePasswordViewModel changePassword, User user)
        {
            UserPassword upwd = new UserPassword
            {
                UserId = user.Id,
                Password = hu_utils.Password._one_way_encrypt(changePassword.OldPassword, user.Id),
                ChangedDate = DateTime.Now
            };
            _context.UserPasswords.Add(upwd);

            user.Password = hu_utils.Password._one_way_encrypt(changePassword.Password, user.Id);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["Success"] = "You have changed your password successfully.";
            //BackgroundJob.Enqueue(() => Mailer.PasswordChanged(user.Id));
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
                    userRole.IsActive = false;
                    userRole.IsDeleted = true;
                    _context.Update(userRole);
                }
                _context.SaveChanges();
                if (selectedRoles != null)
                {
                    foreach (string role in selectedRoles)
                    {
                        int roleId = int.Parse(role);
                        var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);
                        if (userRole != null)
                        {
                            userRole.IsActive = true;
                            userRole.IsDeleted = false;
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

        public ActionResult Logout()
        {
            TempData["Headedto"] = null;
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new {area=""});
        }

        public ActionResult ResetPassword(string id, string verification_code)
        {
            //if (!string.IsNullOrEmpty(id))
            //{
            //    if (_context.UserPasswordResets.Any(x => x.Token == id && x.ExpiryDate > DateTime.Now && x.Validated != true))
            //    {
            //        var USrPR = _context.UserPasswordResets.FirstOrDefault(x => x.Token == id);

            //        var prvm =new PasswordResetViewModel
            //        {
            //            FullName = USrPR.User.Employee.FullName,
            //            Token = USrPR.Token,
            //            ExpiryDate = USrPR.ExpiryDate,
            //            Photo = USrPR.User.Employee.PhotoUrl,
            //            uprId = USrPR.Id,
            //            VerificationCode = verification_code
            //        };

            //        return View(prvm);
            //    }
            //    else
            //    {
            //        TempData["Error"] = "The link you are trying to access is already verified or expired.";
            //    }

            //}
            return RedirectToAction("Login");
        }

        public ActionResult ChangePassword()
        {
            //if (string.IsNullOrEmpty(_session.GetString("UserId")))
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //int userId = int.Parse(_session.GetString("UserId"));
            //ViewBag.pwdChanges = _context.UserPasswords.Where(x => x.UserId == userId);
            return View();
        }

    }
}
