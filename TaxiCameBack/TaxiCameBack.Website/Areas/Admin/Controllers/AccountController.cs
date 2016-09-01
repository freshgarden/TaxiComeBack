using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Email;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Services.Email;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Settings;
using TaxiCameBack.Website.Application;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Application.Extension;
using TaxiCameBack.Website.Application.Security;
using TaxiCameBack.Website.Areas.Admin.Models;
using TaxiCameBack.Website.Areas.Admin.Models.Mapping;
using TaxiCameBack.Website.App_LocalResources;
using TaxiCameBack.Website.Models;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly IEmailService _emailService;
        private readonly ISettingsService _settingsService;
        public AccountController(IMembershipService membershipService, IEmailService emailService, ISettingsService settingsService)
        {
            _membershipService = membershipService;
            _emailService = emailService;
            _settingsService = settingsService;
        }

        [CustomAuthorize]
        public ActionResult Index(int? p, string search)
        {
            if (SessionPersister.Roles.Contains(AppConstants.AdminRoleName))
                return ListUsers(p, search);
            return RedirectToAction("Index", "Schedule");
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
        public ActionResult Edit(Guid id)
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
                ModelState.AddModelError("ErrorMessage", App_LocalResources.ApproveUser.approve_user_failed);
                return View(userModel);
            }
            else
            {
                var message = new GenericMessageViewModel
                {
                    Message = App_LocalResources.ApproveUser.approve_user_success,
                    MessageType = GenericMessages.success
                };

                TempData[AppConstants.MessageViewBagName] = message;
                return RedirectToAction("Index", "Account", new {area = "Admin"});
            }
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
                var reuslt = _membershipService.Logon(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe);
                if (reuslt.Errors.Count <= 0)
                {
                    // get here Login failed, check the login status
                    var loginStatus = _membershipService.LastLoginStatus;

                    switch (loginStatus)
                    {
                        // Account or password wrong!
                        case LoginAttemptStatus.UserNotFound:
                        case LoginAttemptStatus.PasswordIncorrect:
                            ModelState.AddModelError("ErrorMessage", App_LocalResources.Login.password_incorect);
                            break;
                        // Account locked
                        case LoginAttemptStatus.UserLockedOut:
                            ModelState.AddModelError("ErrorMessage", App_LocalResources.Login.user_locked);
                            break;
                        case LoginAttemptStatus.PasswordAttemptsExceeded:
                            ModelState.AddModelError("ErrorMessage", App_LocalResources.Login.password_attemp);
                            break;
                        // Account not approved
                        case LoginAttemptStatus.UserNotApproved:
                            ModelState.AddModelError("ErrorMessage", App_LocalResources.Login.user_not_approve);
                            break;
                        // Account banned
                        case LoginAttemptStatus.Banned:
                            ModelState.AddModelError("ErrorMessage", App_LocalResources.Login.user_banned);
                            break;
                    }
                    // If have not error redirect to Index Backend
                    if (ModelState.Errors() != null)
                    {
                        return View(loginViewModel);
                    }
                    
                    SessionPersister.Username = loginViewModel.UserName;
                    SessionPersister.UserId = _membershipService.GetUser(loginViewModel.UserName).UserId;
                    SessionPersister.Roles = _membershipService.GetRolesForUser(loginViewModel.UserName).ToArray();
                    SessionPersister.FullName = _membershipService.GetUser(loginViewModel.UserName).FullName;
                    SessionPersister.UserImageUrl = _membershipService.GetUser(loginViewModel.UserName).Avatar;

                    return _membershipService.GetRolesForUser(SessionPersister.Username).Contains(AppConstants.AdminRoleName) ? RedirectToAction("Index", "Account", new {area = "Admin"}) : RedirectToAction("Index", "Schedule", new {area = "Admin"});
                }
                ModelState.AddModelError(string.Empty, reuslt.Errors[0]);
            }
            else
            {
                return View(loginViewModel);
            }
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        public ActionResult LogOff()
        {
            SessionPersister.ClearAll();
            return RedirectToAction("Login", "Account", new {area = "Admin"});
        }

        [HttpPost]
        public JsonResult doesUserNameExist(string email)
        {

            var user = _membershipService.GetUser(email);

            return Json(user == null);
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
            if (ModelState.IsValid)
            {
                var userToSave = new Core.DomainModel.Membership.MembershipUser
                {
                    Email = registerViewModel.Email,
                    Password = registerViewModel.Password,
                    FullName = registerViewModel.FullName,
                    PhoneNumber = registerViewModel.Phone
                };

                var createStatus = _membershipService.CreateUser(userToSave);

                if (createStatus.Errors.Count != 0)
                {
                    ModelState.AddModelError("ErrorMessage", App_LocalResources.Register.register_failed);
                    return View(registerViewModel);
                }

                TempData["Message"] = App_LocalResources.Register.register_success;
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }
            else
            {
                return View(registerViewModel);
            }
        }
        
        private static MemberViewModels.SelfMemberEditViewModel PopulateMemberViewModel(MembershipUser user)
        {
            var viewModel = new MemberViewModels.SelfMemberEditViewModel
            {
                Id = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Carmakers = user.Carmakers,
                DateOfBirth = user.DateOfBirth,
                Day = user.DateOfBirth?.Day ?? 0,
                Month = user.DateOfBirth?.Month ?? 0,
                Year = user.DateOfBirth?.Year ?? 0,
                CarNumber = user.CarNumber,
                Address = user.Address
            };
            return viewModel;
        }

        [CustomAuthorize]
        public ActionResult EditProfile()
        {
            var loggedOnUser = _membershipService.GetUser(SessionPersister.Username);
            // Check is has permission
            if (SessionPersister.Roles.Contains(AppConstants.AdminRoleName) ||
                loggedOnUser.Email.Equals(SessionPersister.Username))
            {
                var viewModel = PopulateMemberViewModel(loggedOnUser);
                return View(viewModel);
            }
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = "Access denied",
                MessageType = GenericMessages.danger
            };
            return _membershipService.GetRolesForUser(SessionPersister.Username).Contains(AppConstants.AdminRoleName)
                ? RedirectToAction("Index", "Account", new { area = "Admin" })
                : RedirectToAction("Index", "Schedule", new { area = "Admin" });
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(MemberViewModels.SelfMemberEditViewModel editViewModel)
        {
            var loggedOnUser = _membershipService.GetById(editViewModel.Id);
            if (SessionPersister.Roles.Contains(AppConstants.AdminRoleName) ||
                loggedOnUser.Email.Equals(SessionPersister.Username))
            {
                // Sort image out first
                if (editViewModel.File != null)
                {
                    // Before we save anything, check the user already has an upload folder and if not create one
                    var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(AppConstants.UploadFolderPath, editViewModel.Id));
                    if (uploadFolderPath != null && Directory.Exists(uploadFolderPath))
                    {
                        Directory.Delete(uploadFolderPath, true);
                    }

                    if (uploadFolderPath != null && !Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    var uploadResult = AppHelpers.UploadFile(editViewModel.File, uploadFolderPath, true);
                    if (!uploadResult.UploadSuccessful)
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = uploadResult.ErrorMessage,
                            MessageType = GenericMessages.danger
                        };
                        return View(editViewModel);
                    }

                    // Save avatar to user
                    loggedOnUser.Avatar = uploadResult.UploadedFileName;
                }

                editViewModel.Avatar = loggedOnUser.Avatar;
                loggedOnUser.Address = !string.IsNullOrEmpty(editViewModel.Address) ? editViewModel.Address : loggedOnUser.Address;
                loggedOnUser.Gender = !string.IsNullOrEmpty(editViewModel.Gender) ? editViewModel.Gender : loggedOnUser.Gender;
                loggedOnUser.CarNumber = !string.IsNullOrEmpty(editViewModel.CarNumber) ? editViewModel.CarNumber : loggedOnUser.CarNumber;
                loggedOnUser.Carmakers = !string.IsNullOrEmpty(editViewModel.Carmakers) ? editViewModel.Carmakers : loggedOnUser.Carmakers;
                loggedOnUser.PhoneNumber = editViewModel.PhoneNumber;
                loggedOnUser.FullName = editViewModel.FullName;
                if (editViewModel.Day > 0 && editViewModel.Month > 0 && editViewModel.Year > 0)
                {
                    if (editViewModel.Day.HasValue && editViewModel.Month.HasValue && editViewModel.Year.HasValue)
                        loggedOnUser.DateOfBirth = new DateTime(editViewModel.Year.Value, editViewModel.Month.Value, editViewModel.Day.Value);
                }

                var result = _membershipService.ProfileUpdated(loggedOnUser);

                if (result.Success)
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = UserProfile.profile_updated,
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("EditProfile");
                }

                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = UserProfile.profile_update_fail,
                    MessageType = GenericMessages.danger
                };
                return View(editViewModel);
            }

            return _membershipService.GetRolesForUser(SessionPersister.Username).Contains(AppConstants.AdminRoleName)
                ? RedirectToAction("Index", "Account", new { area = "Admin" })
                : RedirectToAction("Index", "Schedule", new { area = "Admin" });
        }

        [CustomAuthorize]
        public PartialViewResult ChangePassword()
        {
            return PartialView("_ChangePassword", new ChangePasswordViewModel());
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loggedOnUser = _membershipService.GetUser(SessionPersister.Username);
                var result = _membershipService.ChangePassword(loggedOnUser, model.OldPassword, model.NewPassword);
                if (result)
                {
                    // We use temp data because we are doing a redirect
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = App_LocalResources.ChangePassword.change_password_success,
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("EditProfile");
                }

                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = App_LocalResources.ChangePassword.change_password_fail,
                    MessageType = GenericMessages.danger
                };
                return RedirectToAction("EditProfile");
            }


            return RedirectToAction("EditProfile");
        }
        
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(MemberViewModels.ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _membershipService.GetUser(forgotPasswordViewModel.EmailAddress);

                // not exist user
                if (user == null)
                {
                    ModelState.AddModelError("ErrorMessage", App_LocalResources.ForgotPassword.ERR_MESSAGE_NO_EXIST);
                    return View(forgotPasswordViewModel);
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
                    var url = new Uri(string.Concat(_settingsService.GetSettings().SiteUrl.TrimEnd('/'), Url.Action("ResetPassword", "Account", new { user.Email, token = user.PasswordResetToken })));

                    var sb = new StringBuilder();
                    sb.AppendFormat("<p>{0}</p>", string.Format(AppConstants.ResetPasswordEmailText, _settingsService.GetSettings().SiteName));
                    sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", url);

                    var email = new Email
                    {
                        EmailTo = user.Email,
                        NameTo = user.FullName,
                        Subject = AppConstants.ForgotPasswordSubject
                    };
                    email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                    await Task.Run(() => _emailService.SendMail(email));

                    TempData["Message"] = App_LocalResources.ForgotPassword.SUC_MESSAGE_SEND_EMAIL;
                    return RedirectToAction("Login", "Account", new { area = "Admin" });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ErrorMessage", App_LocalResources.ForgotPassword.reset_password_error);
                    return View(forgotPasswordViewModel);
                }
            }
            else
            {
                return View(forgotPasswordViewModel);
            }
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
                ModelState.AddModelError("ErrorMessage", App_LocalResources.ResetPassword.reset_password_error);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(MemberViewModels.ResetPasswordViewModel postedViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(postedViewModel.Email))
                {
                    var user = _membershipService.GetUser(postedViewModel.Email);

                    // if the user id wasn't found then we can't proceed
                    // if the token submitted is not valid then do not proceed
                    if (user == null || user.PasswordResetToken == null || !_membershipService.IsPasswordResetTokenValid(user, postedViewModel.Token))
                    {
                        ModelState.AddModelError("ErrorMessage", App_LocalResources.ResetPassword.reset_password_error);
                        return View(postedViewModel);
                    }

                    if (!_membershipService.ResetPassword(user, postedViewModel.NewPassword))
                    {
                        ModelState.AddModelError("ErrorMessage", App_LocalResources.ResetPassword.reset_password_error);
                        return View(postedViewModel);
                    }
                }
                TempData["Message"] = App_LocalResources.ResetPassword.SUC_MESSAGE_RESET_PASSWORD;
                return RedirectToAction("Login", "Account", new { area = "Admin" });
            }
            else
            {
                return View(postedViewModel);
            }
        }
    }
}