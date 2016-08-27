using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace TaxiCameBack.Website.Application.Security
{
    public static class SessionPersister
    {
        private const string UsernamePersister = "username";
        private const string RolePersister = "role";
        private const string UserIdPersister = "userid";
        private const string FullNamePersister = "fullname";
        private const string UserImageUrlPersister = "userimageurl";
        private const int Expires = 24;

        public static string Username
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                return GetCookie<string>(UsernamePersister);
            }
            set
            {
                SetCookie(UsernamePersister, Expires, value);
            }
        }

        public static Guid UserId
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return Guid.Empty;
                }
                return GetCookie<Guid>(UserIdPersister);
            }
            set
            {
                SetCookie(UserIdPersister, Expires, value.ToString());
            }
        }

        public static string[] Roles
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }
                return GetCookie(RolePersister);
            }
            set
            {
                SetCookie(RolePersister, Expires, value);
            }
        }

        public static string FullName
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                return GetCookie<string>(FullNamePersister);
            }
            set
            {
                SetCookie(FullNamePersister, Expires, value);
            }
        }

        public static string UserImageUrl
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }
                return GetCookie<string>(UserImageUrlPersister);
            }
            set
            {
                SetCookie(UserImageUrlPersister, Expires, value);
            }
        }

        private static void SetCookie(string cookieName, int hours, string value)
        {
            HttpCookie myCookie = new HttpCookie(cookieName)
            {
                Value = !string.IsNullOrEmpty(value) ? Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(value))) : string.Empty,
                Expires = DateTime.Now.AddHours(hours)
            };
            // Add the cookie.
            HttpContext.Current.Response.Cookies.Add(myCookie);

        }

        private static void SetCookie(string cookieName, int hours, string[] values)
        {
            HttpCookie myCookie = new HttpCookie(cookieName);
            foreach (var value in values)
            {
                myCookie.Values.Add(value, value);
            }
            myCookie.Expires = DateTime.Now.AddHours(hours);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        private static T GetCookie<T>(string cookieName)
        {
            var myCookie = HttpContext.Current.Request.Cookies[cookieName];
            if (myCookie != null)
            {
                if (!string.IsNullOrEmpty(myCookie.Value))
                {
                    return (T) TypeDescriptor.GetConverter(typeof (T))
                        .ConvertFromInvariantString(
                            Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(myCookie.Value))));
                }
                return default(T);
            }
            return default(T);
        }

        private static string[] GetCookie(string cookieName)
        {
            var myCookies = HttpContext.Current.Request.Cookies[cookieName];
            var result = new List<string>();
            if (myCookies != null)
            {
                result.AddRange(myCookies.Values.AllKeys.Select(myCookie => myCookies[myCookie]));
            }

            return result.Count > 0 ? result.ToArray() : null;
        }

        public static void ClearAll()
        {
            Clear(UsernamePersister);
            Clear(FullNamePersister);
            Clear(RolePersister);
            Clear(UserIdPersister);
            Clear(UserImageUrlPersister);
        }

        public static void Clear(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                var c = new HttpCookie(name);
                c.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(c);
            }
        }
    }
}