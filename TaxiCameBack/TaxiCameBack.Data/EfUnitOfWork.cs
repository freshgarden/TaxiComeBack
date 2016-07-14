using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using TaxiCameBack.Core.DomainModel.Membership;
using TaxiCameBack.Data.Contract;
using TaxiCameBack.Data.Migrations;

namespace TaxiCameBack.Data
{
    public class EfUnitOfWork : DbContext, IQueryableUnitOfWork, IDisposable
    {
        public EfUnitOfWork()
            : base("TaxiCameBack")
        {
            InitiateData();
        }
        public EfUnitOfWork(string nameOfConnection)
            : base(nameOfConnection)
        {
            InitiateData();
        }

        private void InitiateData()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<EfUnitOfWork, Configuration>());
        }

        public DbSet<MembershipRole> MembershipRole { get; set; }
        public DbSet<MembershipUser> MembershipUser { get; set; }

        public void Commit()
        {
            SaveChanges();
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed;

            do
            {
                try
                {
                    SaveChanges();

                    saveFailed = false;

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });

                }
            } while (saveFailed);
        }

        public void Rollback()
        {
            ChangeTracker.Entries().ToList().ForEach(entry => entry.State = EntityState.Unchanged);
        }

        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters)
        {
            return Database.SqlQuery<T>(sqlQuery, parameters);
        }

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

        public DbSet<T> CreateSet<T>() where T : class
        {
            return Set<T>();
        }

        public void Attach<T>(T item) where T : class
        {
            Entry(item).State = EntityState.Unchanged;
        }

        public void SetModified<T>(T item) where T : class
        {
            //this operation also attach item in object state manager
            Entry(item).State = EntityState.Modified;
        }

        public void ApplyCurrentValues<T>(T original, T current) where T : class
        {
            //if it is not attached, attach original and set current values
            Entry(original).CurrentValues.SetValues(current);
        }

        #region DbContext Overrides

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                                    .Where(type => !string.IsNullOrEmpty(type.Namespace))
                                    .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                                    && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }
        }

        #endregion DbContext Overrides
    }
}
