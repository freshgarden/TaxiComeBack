using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Email;
using TaxiCameBack.Services.Email;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Extension;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Areas.Admin.Models;
using TaxiCameBack.Website.ViewModels.Mapping;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly IEmailService _emailService;
        public AccountController(IMembershipService membershipService, IEmailService emailService)
        {
            _membershipService = membershipService;
            _emailService = emailService;
        }

        [CustomAuthorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult Index(int? p, string search)
        {
            return ListUsers(p, search);
        }

        [CustomAuthorize(Roles = AppConstants.AdminRoleName)]
        private ActionResult ListUsers(int? p, string search)
        {
            var pageIndex = p ?? 1;
            var allUsers = _membershipService.GetAll(pageIndex, AppConstants.AdminListPageSize);

            // Redisplay list of users
            var allViewModelUsers = allUsers.Select(ViewModelMapping.UserToSingleMemberListViewModel).ToList();
            var memberListModel = new MemberListViewModel
            {
                Users = allViewModelUsers,
                Id = _membershipService.GetUser(SessionPersister.Username).UserId,
                PageIndex = pageIndex,
                Search = search,
                TotalCount = allUsers.Count,
                TotalPages = allUsers.TotalPages
            };

            return View("Index", memberListModel);
        }

        [CustomAuthorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult Edit(int id)
        {
            var user = _membershipService.GetById(id);
            var viewModel = ViewModelMapping.UserToMemberEditViewModel(user);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult Edit(MemberEditViewModel userModel)
        {
            var user = _membershipService.GetById(userModel.Id);

            user.Email = userModel.Email;
            user.Active = userModel.IsApproved;
            user.IsLockedOut = userModel.IsLockedOut;

            var result = _membershipService.UpdateUser(user);

            if (result.Errors.Count > 0)
            {
                ModelState.AddModelError(string.Empty, result.Errors[0]);
            }
            else
            {
                return RedirectToAction("Index", "Account", new {area = "Admin"});
            }

            return View(userModel);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            // Create the empty view model
            var viewModel = new LoginViewModel();
            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }
            viewModel.Message = (string) TempData["Message"];
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cookie = null;
                var reuslt = _membershipService.Logon(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe, ref cookie);
                if (reuslt.Errors.Count <= 0)
                {
                    // get here Login failed, check the login status
                    var loginStatus = _membershipService.LastLoginStatus;

                    switch (loginStatus)
                    {
                        case LoginAttemptStatus.UserNotFound:
                        case LoginAttemptStatus.PasswordIncorrect:
                            ModelState.AddModelError(string.Empty, "Password Incorrect");
                            break;

                        case LoginAttemptStatus.PasswordAttemptsExceeded:
                            ModelState.AddModelError(string.Empty, "Password Attempts Exceeded");
                            break;

                        case LoginAttemptStatus.UserLockedOut:
                            ModelState.AddModelError(string.Empty, "User Locked Out");
                            break;
                            
                        case LoginAttemptStatus.UserNotApproved:
                            ModelState.AddModelError(string.Empty, "User Not Approved");
                            break;
                    }
                    if (ModelState.Errors() != null)
                    {
                        return View(loginViewModel);
                    }
                    if (cookie != null)
                        Response.Cookies.Add(cookie);
                    SessionPersister.Username = loginViewModel.UserName;
                    SessionPersister.UserId = _membershipService.GetUser(loginViewModel.UserName).UserId;
                    SessionPersister.Roles = _membershipService.GetRolesForUser(loginViewModel.UserName).ToArray();
//                    if (Url.IsLocalUrl(loginViewModel.ReturnUrl) && loginViewModel.ReturnUrl.Length > 1 && loginViewModel.ReturnUrl.StartsWith("/")
//                                        && !loginViewModel.ReturnUrl.StartsWith("//") && !loginViewModel.ReturnUrl.StartsWith("/\\"))
//                    {
//                        return Redirect(loginViewModel.ReturnUrl);
//                    }
                    
                    return _membershipService.GetRolesForUser(SessionPersister.Username).Contains(AppConstants.AdminRoleName) ? RedirectToAction("Index", "Account", new {area = "Admin"}) : RedirectToAction("Index", "Schedule", new {area = "Admin"});
                }
                ModelState.AddModelError(string.Empty, reuslt.Errors[0]);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "There was an error login");
            }
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        public ActionResult LogOff()
        {
            SessionPersister.Username = string.Empty;
            Array.Clear(SessionPersister.Roles, 0, SessionPersister.Roles.Length);
            return RedirectToAction("Login", "Account", new {area = "Admin"});
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                var error = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                ModelState.AddModelError(string.Empty, error);
                return View(registerViewModel);
            }

            var userToSave = new Core.DomainModel.Membership.MembershipUser
            {
                Email = registerViewModel.Email,
                Password = registerViewModel.Password,
                FullName = registerViewModel.FullName
            };

            var createStatus = _membershipService.CreateUser(userToSave);

            if (createStatus.Errors.Count != 0)
            {
                ModelState.AddModelError(string.Empty, createStatus.Errors[0]);
                return View(registerViewModel);
            }
            TempData["Message"] = "Create new account was successful! Please wait while administrator approved.";
            return RedirectToAction("Login", "Account", new {area = "Admin"});
        }

        [HttpGet]
        public ViewResult PasswordResetSent()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(MemberViewModels.ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordViewModel);
            }

            var user = _membershipService.GetUser(forgotPasswordViewModel.EmailAddress);

            // If the email address is not registered then display the 'email sent' confirmation the same as if 
            // the email address was registered. There is no harm in doing this and it avoids exposing registered 
            // email addresses which could be a privacy issue if the forum is of a sensitive nature. */
            if (user == null)
            {
                return RedirectToAction("PasswordResetSent", "Account");
            }
            
            // If the user is registered then create a security token and a timestamp that will allow a change of password
            if (!_membershipService.UpdatePasswordResetToken(user).Success)
            {
                return View(forgotPasswordViewModel);
            }
            
            // At this point the email address is registered and a security token has been created
            // so send an email with instructions on how to change the password
            try
            {
                var url = new Uri(string.Concat(AppConstants.SiteUrl.TrimEnd('/'), Url.Action("ResetPassword", "Account", new { user.Email, token = user.PasswordResetToken })));

                var sb = new StringBuilder();
                sb.AppendFormat("<p>{0}</p>", string.Format(AppConstants.ResetPasswordEmailText, AppConstants.SiteName));
                sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", url);

                var email = new Email
                {
                    EmailTo = user.Email,
                    NameTo = user.FullName,
                    Subject = AppConstants.ForgotPasswordSubject
                };
                email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                _emailService.SendMail(email);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "Reset password was error: " + exception.Message);
                return View(forgotPasswordViewModel);
            }

            return RedirectToAction("PasswordResetSent", "Account");
        }

        [HttpGet]
        public ActionResult ResetPassword(string email, string token)
        {
            var model = new MemberViewModels.ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, AppConstants.ResetPasswordInvalidToken);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(MemberViewModels.ResetPasswordViewModel postedViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(postedViewModel);
            }

            if (!string.IsNullOrEmpty(postedViewModel.Email))
            {
                var user = _membershipService.GetUser(postedViewModel.Email);

                // if the user id wasn't found then we can't proceed
                // if the token submitted is not valid then do not proceed
                if (user == null || user.PasswordResetToken == null || !_membershipService.IsPasswordResetTokenValid(user, postedViewModel.Token))
                {
                    ModelState.AddModelError(string.Empty, AppConstants.ResetPasswordInvalidToken);
                    return View(postedViewModel);
                }

                if (!_membershipService.ResetPassword(user, postedViewModel.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, AppConstants.ResetPasswordInvalidToken);
                    return View(postedViewModel);
                }
            }

            return RedirectToAction("PasswordChanged", "Account");
        }

        [HttpGet]
        public ViewResult PasswordChanged()
        {
            return View();
        }
    }
}