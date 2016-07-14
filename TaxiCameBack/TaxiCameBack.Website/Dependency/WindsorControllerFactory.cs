﻿using System;
using System.Web;
using Castle.MicroKernel;
using System.Web.Mvc;
using System.Web.Routing;

namespace TaxiCameBack.Website.Dependency
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        public WindsorControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
//                throw new HttpException(404, $"The controller for path '{requestContext.HttpContext.Request.Path}' could not be found.");
            }
            return (IController) _kernel.Resolve(controllerType);
        }
    }
}