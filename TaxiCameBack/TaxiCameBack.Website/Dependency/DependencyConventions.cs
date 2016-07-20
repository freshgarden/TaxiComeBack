using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using TaxiCameBack.Data;
using TaxiCameBack.Data.Contract;
using System.Web.Http.Controllers;
using Castle.Facilities.Logging;
using TaxiCameBack.Core;
using TaxiCameBack.Services.Email;
using TaxiCameBack.Services.Membership;
using TaxiCameBack.Services.Schedule;
using TaxiCameBack.Services.Search;

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

                Component.For(typeof (IRepository<>)).ImplementedBy(typeof (EfRepository<>)).LifestyleSingleton(),

                Component.For<IMembershipService>().ImplementedBy<MembershipService>(),

                Component.For<IScheduleService>().ImplementedBy<ScheduleService>(),

                Component.For<ISearchSchduleService>().ImplementedBy<SearchSchduleService>(),

                Component.For<IEmailService>().ImplementedBy<EmailService>(),

                Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient()

                ).AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("Config/log4net.config"));
        }
    }
}