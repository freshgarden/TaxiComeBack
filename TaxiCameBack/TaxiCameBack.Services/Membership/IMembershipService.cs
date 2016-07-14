using System.Web;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.DomainModel.PagedList;
using TaxiCameBack.Services.Common;

namespace TaxiCameBack.Services.Membership
{
    public enum LoginAttemptStatus
    {
        LoginSuccessful,
        UserNotFound,
        PasswordIncorrect,
        PasswordAttemptsExceeded,
        UserLockedOut,
        UserNotApproved,
        Banned
    }
    public interface IMembershipService : IEntityService<MembershipUser>
    {
        MembershipUser SanitizeUser(MembershipUser membershipUser);
        bool ValidateUser(string userEmail, string password, int maxInvalidPasswordAttempts);
        string[] GetRolesForUser(string userEmail);
        MembershipUser GetUser(string userEmail, bool removeTracking = false);
        LoginAttemptStatus LastLoginStatus { get; }
        CrudResult Logon(string userEmail, string password, bool remember, ref HttpCookie cookie);
        bool ChangePassword(MembershipUser user, string oldPassword, string newPassword);
        bool ResetPassword(MembershipUser user, string newPassword);
        CrudResult CreateUser(MembershipUser newUser);
        CrudResult UpdateUser(MembershipUser user);
        PagedList<MembershipUser> GetAll(int pageIndex, int pageSize);
        string ErrorCodeToString(MembershipCreateStatus createStatus);
        void ProfileUpdated(MembershipUser user);
        int MemberCount();
        bool UpdatePasswordResetToken(MembershipUser user);
        bool ClearPasswordResetToken(MembershipUser user);
        string CreatePasswordResetToken();
        bool IsPasswordResetTokenValid(MembershipUser user, string token);
    }
}
