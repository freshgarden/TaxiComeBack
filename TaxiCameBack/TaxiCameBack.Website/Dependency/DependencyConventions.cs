using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TaxiCameBack.Data;
using TaxiCameBack.Data.Contract;
using System.Web.Http.Controllers;
using Castle.Facilities.Logging;
using TaxiCameBack.Core;
using TaxiCameBack.Services.Contract;
using TaxiCameBack.Services.Schedule;
using TaxiCameBack.Services.Search;
using TaxiCameBack.Services.User;

namespace TaxiCameBack.Website.Dependency
{
    public class DependencyConventions : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                .BasedOn<IController>()
                                .LifestyleTransient());

            container.Register(
                Component.For<IQueryableUnitOfWork>()
                    .ImplementedBy<EfUnitOfWork>()
                    .DependsOn(Castle.MicroKernel.Registration.Dependency.OnValue("nameOfConnection", "TaxiCameBack"))
                    .LifestyleSingleton(),

                Component.For(typeof (IRepository<>)).ImplementedBy(typeof (EfRepository<>)),

                Component.For<IUserService>().ImplementedBy<UserService>(),

                Component.For<IContactManager>().ImplementedBy<ContactManager>(),

                Component.For<IScheduleService>().ImplementedBy<ScheduleService>(),

                Component.For<ISearchSchduleService>().ImplementedBy<SearchSchduleService>(),

                Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient()

                ).AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("Config/log4net.config"));
        }
    }
}