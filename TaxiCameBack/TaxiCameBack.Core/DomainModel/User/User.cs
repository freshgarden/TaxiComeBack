using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaxiCameBack.Core.DomainModel.User
{
    public class User : BaseEntity
    {
        private ICollection<UserRole> _userRoles;
        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }
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

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }
        public virtual ICollection<Schedule.Schedule> Schedules { get; set; }
    }
}
