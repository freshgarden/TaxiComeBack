using System;
using System.Collections.Generic;
using TaxiCameBack.Core;

namespace TaxiCameBack.Services.Common
{
    public abstract class EntityService<T> : IEntityService<T> where T : BaseEntity
    {
        private readonly IRepository<T> _membershipRepository;

        protected EntityService(IRepository<T> membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }
         
        public void Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _membershipRepository.Insert(entity);
            _membershipRepository.UnitOfWork.Commit();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _membershipRepository.Delete(entity);
            _membershipRepository.UnitOfWork.Commit();
        }

        public IEnumerable<T> GetAll()
        {
            return _membershipRepository.GetAll();
        }

        public T GetById(Guid id)
        {
            return _membershipRepository.GetById(id);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _membershipRepository.Update(entity);
            _membershipRepository.UnitOfWork.Commit();
        }
    }
}
