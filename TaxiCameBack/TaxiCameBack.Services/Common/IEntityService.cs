using System;
using System.Collections.Generic;
using TaxiCameBack.Core;

namespace TaxiCameBack.Services.Common
{
    public interface IEntityService<T> : IService where T : BaseEntity
    {
        void Create(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Update(T entity);
    }
}
