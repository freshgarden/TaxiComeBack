using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Core.DomainModel.Settings;
using TaxiCameBack.Core.Utilities;

namespace TaxiCameBack.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EfUnitOfWork>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(EfUnitOfWork context)
        {
            var saveRoles = false;
            // Create the admin role if it doesn't exist
            var adminRole = context.CreateSet<MembershipRole>().FirstOrDefault(x => x.RoleName == AppConstants.AdminRoleName);
            if (adminRole == null)
            {
                adminRole = new MembershipRole { RoleName = AppConstants.AdminRoleName };
                context.MembershipRole.Add(adminRole);
                saveRoles = true;
            }

            // Create the Standard role if it doesn't exist
            var standardRole = context.CreateSet<MembershipRole>().FirstOrDefault(x => x.RoleName == AppConstants.StandardMembers);
            if (standardRole == null)
            {
                standardRole = new MembershipRole { RoleName = AppConstants.StandardMembers };
                context.MembershipRole.Add(standardRole);
                saveRoles = true;
            }

            if (saveRoles)
            {
                context.SaveChanges();
            }

            // If the admin user exists then don't do anything else
            const string adminUserEmail = "taxiquayve@gmail.com";
            if (context.MembershipUser.FirstOrDefault(x => x.Email == adminUserEmail) == null)
            {
                // create the admin user and put him in the admin role
                var admin = new MembershipUser
                {
                    Email = adminUserEmail,
                    Active = true,
                    FullName = "Nguyễn Xuân Phúc",
                    Password = "taxiquayve2016",
                    PhoneNumber = "0985776625",
                    CreatedOnUtc = DateTime.UtcNow,
                    LastLoginDateUtc = DateTime.UtcNow,
                    IsLockedOut = false,
                    LastLockoutDate = null
                };
                // Hash the password
                var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                var hash = StringUtils.GenerateSaltedHash(admin.Password, salt);
                admin.Password = hash;
                admin.PasswordSalt = salt;

                // Put the admin in the admin role
                admin.Roles = new List<MembershipRole> { adminRole };

                context.MembershipUser.Add(admin);
                context.SaveChanges();
            }

            // if the settings already exist then do nothing
            // If not then add default settings
            var currentSettings = context.Settings.FirstOrDefault();
            if (currentSettings == null)
            {
                var settings = new Settings
                {
                    SiteName = "Taxi Came Back",
                    SiteUrl = "http://www.domain.com",
                    AdminEmailAddress = "my@email.com",
                    NotificationReplyEmail = "noreply@myemail.com",
                    SMTPEnableSSL = false,
                    NewMemberStartingRole = standardRole
                };

                context.Settings.Add(settings);
                context.Commit();
            }
        }
    }
}
