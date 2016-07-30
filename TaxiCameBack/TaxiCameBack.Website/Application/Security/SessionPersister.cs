﻿using System.Web;

namespace TaxiCameBack.Website.Application.Security
{
    public static class SessionPersister
    {
        private const string UsernamePersister = "username";

        public static string Username
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                if (HttpContext.Current.Session == null)
                {
                    return string.Empty;
                }
                var sessionVar = HttpContext.Current.Session[UsernamePersister];
                return sessionVar as string;
            }
            set { HttpContext.Current.Session[UsernamePersister] = value; }
        }

        public static int UserId { get; set; }

        public static string[] Roles { get; set; }

        public static string FullName { get; set; }

        public static string UserImageUrl { get; set; }
    }
}