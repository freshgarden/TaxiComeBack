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

        public const int AdminListPageSize = 10;

        public const int PagingGroupSize = 10;

        public const string SiteUrl = "http://localhost";

        public const string SiteName = "Taxi Came Back";

        // Email config
        public const string Smtp = "smtp.gmail.com";
        public const string SmtpUsername = "admin@gmail.com";
        public const string SmtpPassword = "password";
        public const int SmtpPort = 587;
        public const bool SmtpEnableSsl = true;
        public const string SmtpFromEmail = "admin@local.com";

        // Messages constant
        public const string ForgotPasswordSubject = "Forgot password.";
        public const string ResetPasswordInvalidToken =
            "This request is either invalid or has expired. Please make a password reset request again.";
        public const string ResetPasswordText = "An email has been sent with details on how to reset your password.";
        public const string ResetPasswordEmailText =
            "A request has been made to reset your password on {0}. To reset your password follow the link below. If you did not make this request then please ignore this email. No further action is required and your password will not be changed.";

    }
}
