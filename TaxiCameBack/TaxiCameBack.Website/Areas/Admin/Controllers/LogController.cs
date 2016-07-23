using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TaxiCameBack.Core.Constants;
using TaxiCameBack.Core.DomainModel.Logging;
using TaxiCameBack.Services.Logging;
using TaxiCameBack.Website.Application.Attributes;
using TaxiCameBack.Website.Areas.Admin.Models;

namespace TaxiCameBack.Website.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = AppConstants.AdminRoleName)]
    public class LogController : BaseController
    {
        private readonly ILoggingService _loggingService;
        public LogController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        // GET: Admin/Log
        public ActionResult Index()
        {
            IList<LogEntry> logs = new List<LogEntry>();

            try
            {
                logs = _loggingService.ListLogFile();
            }
            catch (Exception exception)
            {
                var err = $"Unable to access logs: {exception.Message}";
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = err,
                    MessageType = GenericMessages.danger
                };

                _loggingService.Error(exception);
            }

            return View(new ListLogViewModel {LogFiles = logs});
        }

        public ActionResult ClearLog()
        {
            _loggingService.ClearLogFiles();

            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = "Log File Cleared",
                MessageType = GenericMessages.success
            };
            return RedirectToAction("Index");
        }
    }
}