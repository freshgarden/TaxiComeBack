using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using TaxiCameBack.Core;
using TaxiCameBack.Core.Specification.Contract;
using TaxiCameBack.Data.Contract;

namespace TaxiCameBack.Data
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IQueryableUnitOfWork _unitOfWork;

        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public EfRepository(IQueryableUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork => _unitOfWork;

        public IEnumerable<T> GetAll()
        {
            return GetSet();
        }

        public T GetById(object id)
        {
            if (id != null)
                return GetSet().Find(id);
            return null;
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return GetSet().Where(predicate).AsEnumerable();
        }

        public void Insert(T entity)
        {
            GetSet().Add(entity);
        }

        public void Update(T entity)
        {
            if (entity != null)
                _unitOfWork.SetModified(entity);
            else 
                throw new ArgumentNullException();
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                _unitOfWork.Attach(entity);

                GetSet().Remove(entity);
            }
            else
                throw new ArgumentNullException();
        }

        public void Merge(T persisted, T current)
        {
            _unitOfWork.ApplyCurrentValues(persisted, current);
        }

        public IEnumerable<T> AllMatching(ISpecification<T> specification)
        {
            return GetSet().Where(specification.SatisfiedBy());
        }

        public IEnumerable<T> GetPaged<Property>(int pageIndex, int pageCount, Expression<Func<T, Property>> orderByExpression, bool @ascending)
        {
            var set = GetSet();

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount).ToList();
            }
        }

        public IEnumerable<T> GetFiltered(Expression<Func<T, bool>> filter)
        {
            return GetSet().Where(filter);
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }

        IDbSet<T> GetSet()
        {
            return _unitOfWork.CreateSet<T>();
        }
    }
}
