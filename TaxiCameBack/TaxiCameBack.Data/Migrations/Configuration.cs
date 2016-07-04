using System.Data.Entity.Migrations;

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
            base.Seed(context);
        }
    }
}
