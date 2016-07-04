using System;
using System.Collections.Generic;
using TaxiCameBack.Core;

namespace TaxiCameBack.Services.Common
{
    public abstract class EntityService<T> : IEntityService<T> where T : BaseEntity
    {
        private readonly IRepository<T> _repository;

        protected EntityService(IRepository<T> repository)
        {
            _repository = repository;
        }
         
        public void Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _repository.Insert(entity);
            _repository.UnitOfWork.Commit();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _repository.Delete(entity);
            _repository.UnitOfWork.Commit();
        }

        public IEnumerable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _repository.Update(entity);
            _repository.UnitOfWork.Commit();
        }
    }
}
