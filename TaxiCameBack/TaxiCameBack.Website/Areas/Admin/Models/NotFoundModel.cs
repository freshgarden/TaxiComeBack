using System;
using System.Web.Mvc;

namespace TaxiCameBack.Website.Areas.Admin.Models
{
    public class NotFoundModel : HandleErrorInfo
    {
        public NotFoundModel(Exception exception, string controllerName, string actionName)
            : base(exception, controllerName, actionName)
        {
        }

        public string RequestedUrl { get; set; }
        public string ReferrerUrl { get; set; }
    }
}