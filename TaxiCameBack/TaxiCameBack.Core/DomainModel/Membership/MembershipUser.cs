﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.Membership
{
    /// <summary>
    /// Status values returned when creating a user
    /// </summary>
    public enum MembershipCreateStatus
    {
        Success,
        DuplicateEmail,
        InvalidPassword,
        InvalidEmail,
        UserRejected
    }
    public class MembershipUser : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        [Key]
        public int UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenCreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual IList<MembershipRole> Roles { get; set; }
        public virtual ICollection<Schedule.Schedule> Schedules { get; set; }
    }
}
