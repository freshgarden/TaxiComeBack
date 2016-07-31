using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using TaxiCameBack.Core;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.DomainModel.PagedList;
using TaxiCameBack.Core.Utilities;
using TaxiCameBack.Data;
using TaxiCameBack.Data.Contract;
using TaxiCameBack.Services.Common;
using TaxiCameBack.Services.Logging;
using TaxiCameBack.Services.Settings;

namespace TaxiCameBack.Services.Membership
{
    public class MembershipService : EntityService<MembershipUser>, IMembershipService
    {
        private IRepository<MembershipUser> _membershipRepository;
        private IQueryableUnitOfWork _unitOfWork;
        private ILoggingService _loggingService;
        private ISettingsService _settingsService;
        public MembershipService(
            IRepository<MembershipUser> membershipRepository,
            IQueryableUnitOfWork unitOfWork,
            ILoggingService loggingService,
            ISettingsService settingsService)
            : base(membershipRepository)
        {
            _membershipRepository = membershipRepository;
            _unitOfWork = unitOfWork;
            _loggingService = loggingService;
            _settingsService = settingsService;
        }

        public MembershipUser SanitizeUser(MembershipUser membershipUser)
        {
            membershipUser.Email = StringUtils.SafePlainText(membershipUser.Email);
            membershipUser.Password = StringUtils.SafePlainText(membershipUser.Password);
            membershipUser.FullName = StringUtils.SafePlainText(membershipUser.FullName);
            membershipUser.Address = StringUtils.SafePlainText(membershipUser.Address);
            membershipUser.Carmakers = StringUtils.SafePlainText(membershipUser.Carmakers);
            membershipUser.CarNumber = StringUtils.SafePlainText(membershipUser.CarNumber);
            membershipUser.Gender = StringUtils.SafePlainText(membershipUser.Gender);
            return membershipUser;
        }

        public CrudResult CreateUser(MembershipUser newUser)
        {
            var settings = _settingsService.GetSettings();
            newUser = SanitizeUser(newUser);
            var crudResult = new CrudResult();
            var status = MembershipCreateStatus.Success;

            if (string.IsNullOrEmpty(newUser.Email))
            {
                status = MembershipCreateStatus.InvalidEmail;
            }

            if (GetUserByEmail(newUser.Email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }

            if (string.IsNullOrEmpty(newUser.Password))
            {
                status = MembershipCreateStatus.InvalidPassword;
            }

            if (status == MembershipCreateStatus.Success)
            {
                var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                var hash = StringUtils.GenerateSaltedHash(newUser.Password, salt);
                newUser.Password = hash;
                newUser.PasswordSalt = salt;

                newUser.Roles = new List<MembershipRole>
                {
                    settings.NewMemberStartingRole
                };

                // set dates
                newUser.CreatedOnUtc = DateTime.UtcNow;
                //                newUser.LastLockoutDate = (DateTime) SqlDateTime.MinValue;
                newUser.LastLoginDateUtc = DateTime.UtcNow;
                newUser.IsLockedOut = false;
                newUser.Active = false;

                try
                {
                    _membershipRepository.Insert(newUser);
                    _membershipRepository.UnitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _membershipRepository.UnitOfWork.Rollback();
                    crudResult.AddError(ex.Message);
                    _loggingService.Error(ex);
                }
            }
            if (status != MembershipCreateStatus.Success)
                crudResult.AddError(ErrorCodeToString(status));
            return crudResult;
        }

        public CrudResult UpdateUser(MembershipUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            var crudResult = new CrudResult();
            user = SanitizeUser(user);
            _membershipRepository.Update(user);

            try
            {
                _membershipRepository.UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _membershipRepository.UnitOfWork.Rollback();
                crudResult.AddError(ex.Message);
                _loggingService.Error(ex);
            }
            return crudResult;
        }

        public PagedList<MembershipUser> GetAll(int pageIndex, int pageSize)
        {
            var totalCount = ((EfUnitOfWork)_unitOfWork).MembershipUser.Count();
            var results = ((EfUnitOfWork)_unitOfWork).MembershipUser
                                .OrderBy(x => x.Email)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return new PagedList<MembershipUser>(results, pageIndex, pageSize, totalCount);
        }

        public string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    return "Duplicate Email";

                case MembershipCreateStatus.InvalidPassword:
                    return "Invalid Password";

                case MembershipCreateStatus.InvalidEmail:
                    return "Invalid Email";

                case MembershipCreateStatus.UserRejected:
                    return "User Rejected";

                default:
                    return "Unknown Error";
            }
        }

        public CrudResult ProfileUpdated(MembershipUser user)
        {
            var result = new CrudResult();
            var oldUser = _membershipRepository.GetById(user.UserId);
            user = SanitizeUser(user);
            try
            {
                _membershipRepository.Merge(oldUser, user);
                _membershipRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _membershipRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
                _loggingService.Error(exception);
            }
            return result;
        }

        public int MemberCount()
        {
            throw new NotImplementedException();
        }

        public CrudResult UpdatePasswordResetToken(MembershipUser user)
        {
            user = SanitizeUser(user);
            var result = new CrudResult();
            var existingUser = GetUser(user.Email);
            if (existingUser == null)
            {
                result.AddError("Cannot find user.");
                return result;
            }
            existingUser.PasswordResetToken = CreatePasswordResetToken();
            existingUser.PasswordResetTokenCreatedAt = DateTime.UtcNow;

            try
            {
                _membershipRepository.UnitOfWork.Commit();
            }
            catch (Exception exception)
            {
                _membershipRepository.UnitOfWork.Rollback();
                result.AddError(exception.Message);
                _loggingService.Error(exception);
            }
            return result;
        }

        public bool ClearPasswordResetToken(MembershipUser user)
        {
            user = SanitizeUser(user);
            var existingUser = _membershipRepository.GetById(user.UserId);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.PasswordResetToken = null;
            existingUser.PasswordResetTokenCreatedAt = null;
            return true;
        }

        /// <summary>
        /// Generate a password reset token, a guid is sufficient
        /// </summary>
        public string CreatePasswordResetToken()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

        /// <summary>
        /// To be valid:
        /// - The user record must contain a password reset token
        /// - The given token must match the token in the user record
        /// - The token timestamp must be less than 24 hours ago
        /// </summary>
        public bool IsPasswordResetTokenValid(MembershipUser user, string token)
        {
            user = SanitizeUser(user);
            var existingUser = _membershipRepository.GetById(user.UserId);
            if (string.IsNullOrEmpty(existingUser?.PasswordResetToken))
            {
                return false;
            }
            // A security token must have an expiry date
            if (existingUser.PasswordResetTokenCreatedAt == null)
            {
                return false;
            }
            // The security token is only valid for 48 hours
            if ((DateTime.UtcNow - existingUser.PasswordResetTokenCreatedAt.Value).TotalHours >= 48)
            {
                return false;
            }
            return existingUser.PasswordResetToken == token;
        }

        /// <summary>
        /// Return last login status
        /// </summary>
        public LoginAttemptStatus LastLoginStatus { get; private set; } = LoginAttemptStatus.LoginSuccessful;

        /// <summary>
        /// Validate a user by password
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="password"></param>
        /// <param name="maxInvalidPasswordAttempts"> </param>
        /// <returns></returns>
        public bool ValidateUser(string userEmail, string password, int maxInvalidPasswordAttempts)
        {
            userEmail = StringUtils.SafePlainText(userEmail);
            password = StringUtils.SafePlainText(password);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUser(userEmail);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
                return false;
            }

            if (!user.Active)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotApproved;
                return false;
            }

            var allowedPasswordAttempts = maxInvalidPasswordAttempts;
            if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordAttemptsExceeded;
                return false;
            }

            var salt = user.PasswordSalt;
            var hash = StringUtils.GenerateSaltedHash(password, salt);
            var passwordMatches = hash == user.Password;

            user.FailedPasswordAttemptCount = passwordMatches ? 0 : user.FailedPasswordAttemptCount + 1;

            if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            {
                user.IsLockedOut = true;
                user.LastLockoutDate = DateTime.UtcNow;
            }

            if (!passwordMatches)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                return false;
            }

            return LastLoginStatus == LoginAttemptStatus.LoginSuccessful;
        }

        public List<string> GetRolesForUser(string userEmail)
        {
            userEmail = StringUtils.SafePlainText(userEmail);
            var roles = new List<string>();
            var user = GetUser(userEmail);

            if (user != null)
            {
                roles.AddRange(user.Roles.Select(role => role.RoleName));
            }
            return roles;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="removeTracking"></param>
        /// <returns></returns>
        public MembershipUser GetUser(string userEmail, bool removeTracking = false)
        {
            //            if (removeTracking)
            //            {
            //                member = ((EfUnitOfWork)_unitOfWork).MembershipUser.Include(x => x.Roles)
            //                    .AsNoTracking().FirstOrDefault(u => u.Email.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase));
            //            }
            //            else
            //            {
            //                member =
            //                    ((EfUnitOfWork)_unitOfWork).MembershipUser.Include(x => x.Roles)
            //                    .FirstOrDefault(name => name.Email.Equals(userEmail, StringComparison.CurrentCultureIgnoreCase));
            //            }

            userEmail = StringUtils.SafePlainText(userEmail);
            var member = _membershipRepository.FindBy(x => x.Email == userEmail).FirstOrDefault();

            // Do a check to log out the user if they are logged in and have been deleted
            if (member == null && HttpContext.Current.User.Identity.Name == userEmail)
            {
                // Member is null so doesn't exist, yet they are logged in with that username - Log them out
                FormsAuthentication.SignOut();
            }

            return member;
        }

        public MembershipUser GetUserByEmail(string email)
        {
            return
                _membershipRepository.FindBy(
                    e => e.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }

        public CrudResult Logon(string userEmail, string password, bool remember)
        {
            var crudReuslt = new CrudResult();
            if (ValidateUser(userEmail, password, 48))
            {
                var user = GetUser(userEmail);
                if (user.Active && !user.IsLockedOut)
                {
                    user.LastLoginDateUtc = DateTime.Now;

                    try
                    {
                        _membershipRepository.UnitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        _membershipRepository.UnitOfWork.Rollback();
                        crudReuslt.AddError(ex.Message);
                        _loggingService.Error(ex);
                    }
                }
            }
            return crudReuslt;
        }

        /// <summary>
        /// Reset a user password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ResetPassword(MembershipUser user, string newPassword)
        {
            user = SanitizeUser(user);
            var existingUser = GetUser(user.Email);
            if (existingUser == null)
                return false;

            var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
            var newHash = StringUtils.GenerateSaltedHash(newPassword, salt);

            existingUser.Password = newHash;
            existingUser.PasswordSalt = salt;

            try
            {
                _membershipRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception);
                return false;
            }
        }

        public bool ChangePassword(MembershipUser user, string oldPassword, string newPassword)
        {
            oldPassword = StringUtils.SafePlainText(oldPassword);
            newPassword = StringUtils.SafePlainText(newPassword);

            var existingUser = GetById(user.UserId);
            var salt = existingUser.PasswordSalt;
            var oldHash = StringUtils.GenerateSaltedHash(oldPassword, salt);

            if (oldHash != existingUser.Password)
            {
                // Old password is wrong - do not allow update
                return false;
            }

            // Cleared to go ahead with new password
            salt = StringUtils.CreateSalt(AppConstants.SaltSize);
            var newHash = StringUtils.GenerateSaltedHash(newPassword, salt);

            existingUser.Password = newHash;
            existingUser.PasswordSalt = salt;

            try
            {
                _membershipRepository.UnitOfWork.Commit();
                return true;
            }
            catch (Exception exception)
            {
                _membershipRepository.UnitOfWork.Rollback();
                _loggingService.Error(exception);
                return false;
            }
        }
    }
}
