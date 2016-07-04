using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TaxiCameBack.Core.Specification.Contract;

namespace TaxiCameBack.Core
{
    /// <summary>
    /// Base interface to implement Repository Pattern
    /// </summary>
    /// <typeparam name="T">Type of entity for this repository </typeparam>
    public partial interface IRepository<T>  : IDisposable where T : BaseEntity
    {
        /// <summary>
        /// Get the unit of work in this repository
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        IEnumerable<T> GetAll();
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(object id);

        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);

        /// <summary>
        /// Sets modified entity into the repository. 
        /// When calling Commit() method in UnitOfWork 
        /// these changes will be saved into the storage
        /// </summary>
        /// <param name="persisted">The persisted item</param>
        /// <param name="current">The current item</param>
        void Merge(T persisted, T current);

        /// <summary>
        /// Get all elements of type T that matching a
        /// Specification <paramref name="specification"/>
        /// </summary>
        /// <param name="specification">Specification that result meet</param>
        /// <returns></returns>
        IEnumerable<T> AllMatching(ISpecification<T> specification);

        /// <summary>
        /// Get all elements of type T in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <param name="ascending">Specify if order is ascending</param>
        /// <returns>List of selected elements</returns>
        IEnumerable<T> GetPaged<TProperty>(int pageIndex, int pageCount, Expression<Func<T, TProperty>> orderByExpression, bool ascending);
        
        /// <summary>
        /// Get  elements of type T in repository
        /// </summary>
        /// <param name="filter">Filter that each element do match</param>
        /// <returns>List of selected elements</returns>
        IEnumerable<T> GetFiltered(Expression<Func<T, bool>> filter);
    }
}
