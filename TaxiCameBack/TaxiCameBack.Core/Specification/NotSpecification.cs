using System;
using System.Linq;
using System.Linq.Expressions;
using TaxiCameBack.Core.Specification.Contract;
using TaxiCameBack.Core.Specification.Implementation;

namespace TaxiCameBack.Core.Specification
{
    /// <summary>
    /// A logic Not Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class NotSpecification<T> : Specification<T> where T : class
    {
        #region Members

        readonly Expression<Func<T, bool>> _originalCriteria;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for NotSpecificaiton
        /// </summary>
        /// <param name="originalSpecification">Original specification</param>
        public NotSpecification(ISpecification<T> originalSpecification)
        {

            if (originalSpecification == null)
                throw new ArgumentNullException(nameof(originalSpecification));

            _originalCriteria = originalSpecification.SatisfiedBy();
        }

        /// <summary>
        /// Constructor for NotSpecification
        /// </summary>
        /// <param name="originalSpecification">Original specificaiton</param>
        public NotSpecification(Expression<Func<T, bool>> originalSpecification)
        {
            if (originalSpecification == null)
                throw new ArgumentNullException(nameof(originalSpecification));

            _originalCriteria = originalSpecification;
        }

        #endregion

        #region Override Specification methods

        /// <summary>
        /// Implementation for method SatidfiedBy
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> SatisfiedBy()
        {

            return Expression.Lambda<Func<T, bool>>(Expression.Not(_originalCriteria.Body),
                                                         _originalCriteria.Parameters.Single());
        }

        #endregion

    }
}
