namespace TaxiCameBack.Core.Constants
{
    public static class AppConstants
    {
        public const int SaltSize = 24;
        public const string GlobalClass = "hold-transition skin-blue sidebar-mini";
        public const string CurrentAction = "CurrentAction";
        public const string CurrentController = "CurrentController";


        // Main admin role [This should never be changed]
        public const string AdminRoleName = "Admin";

        // Standard Members role
        public const string StandardMembers = "Standard Members";

        public const int AdminListPageSize = 500;

        public const int PagingGroupSize = 10;

        public const string MessageViewBagName = "Message";
        
        public const int FileUploadMaximumFileSizeInBytes = 5242880;
        public const string FileUploadAllowedExtensions = "jpg,jpeg,png,gif,pdf";
        public const string UploadFolderPath = "~/content/uploads/";
        public const int AvatarProfileSize = 85;

        // Messages constant
        public const string ForgotPasswordSubject = "Forgot password.";
        public const string ResetPasswordInvalidToken =
            "This request is either invalid or has expired. Please make a password reset request again.";
        public const string ResetPasswordText = "An email has been sent with details on how to reset your password.";
        public const string ResetPasswordEmailText =
            "A request has been made to reset your password on {0}. To reset your password follow the link below. If you did not make this request then please ignore this email. No further action is required and your password will not be changed.";

        public const string ApproveUserEmailText = "Your account has been accepted by admin. Please go to {0} to login.";
        public const string RejectUserEmailText = "Your account has been rejected by admin.";

        public const string NotificationMessage = "Customer {0} with phone {1} registed taxi came back from {2} to {3} at {4}";

    }
}
