using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;
using TaxiCameBack.Core.DomainModel.AddressAggregate;
using TaxiCameBack.Core.DomainModel.PhoneAggregate;
using TaxiCameBack.Core.DomainModel.ProfileAddressAggregate;
using TaxiCameBack.Core.DomainModel.ProfileAggregate;
using TaxiCameBack.Core.DomainModel.ProfilePhoneAggregate;
using TaxiCameBack.Core.DomainModel.User;
using TaxiCameBack.Data.Contract;
using TaxiCameBack.Data.Migrations;

namespace TaxiCameBack.Data
{
    public class EfUnitOfWork : DbContext, IQueryableUnitOfWork
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

        #region IDbSet Members
        IDbSet<Address> _address;
        public IDbSet<Address> Address
        {
            get
            {
                if (_address == null)
                    _address = base.Set<Address>();

                return _address;
            }
        }

        IDbSet<AddressType> _addressType;
        public IDbSet<AddressType> AddressType
        {
            get
            {
                if (_addressType == null)
                    _addressType = base.Set<AddressType>();

                return _addressType;
            }
        }

        IDbSet<Phone> _phone;
        public IDbSet<Phone> Countries
        {
            get
            {
                if (_phone == null)
                    _phone = base.Set<Phone>();

                return _phone;
            }
        }

        IDbSet<PhoneType> _phoneType;
        public IDbSet<PhoneType> PhoneType
        {
            get
            {
                if (_phoneType == null)
                    _phoneType = base.Set<PhoneType>();

                return _phoneType;
            }
        }

        IDbSet<ProfileAddress> _profileAddress;
        public IDbSet<ProfileAddress> ProfileAddress
        {
            get
            {
                if (_profileAddress == null)
                    _profileAddress = base.Set<ProfileAddress>();

                return _profileAddress;
            }
        }

        IDbSet<Profile> _profile;
        public IDbSet<Profile> Profile
        {
            get
            {
                if (_profile == null)
                    _profile = base.Set<Profile>();

                return _profile;
            }
        }

        IDbSet<ProfilePhone> _profilePhone;
        public IDbSet<ProfilePhone> ProfilePhone
        {
            get
            {
                if (_profilePhone == null)
                    _profilePhone = base.Set<ProfilePhone>();

                return _profilePhone;
            }
        }
        public IDbSet<User> Users { get; set; }
        public IDbSet<UserRole> UserRoles { get; set; }

        #endregion IDbSet Members
        
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
