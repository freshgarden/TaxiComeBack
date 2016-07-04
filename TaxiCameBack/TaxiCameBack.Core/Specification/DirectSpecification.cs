using System;
using System.Linq.Expressions;
using TaxiCameBack.Core.Specification.Implementation;

namespace TaxiCameBack.Core.Specification
{
    /// <summary>
    /// A Direct Specification is a simple implementation
    /// of specification that acquire this from a lambda expression
    /// in  constructor 
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class DirectSpecification<T> : Specification<T> where T : class
    {
        #region Members

        readonly Expression<Func<T, bool>> _matchingCriteria;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for Direct Specification
        /// </summary>
        /// <param name="matchingCriteria">A Matching Criteria</param>
        public DirectSpecification(Expression<Func<T, bool>> matchingCriteria)
        {
            if (matchingCriteria == null)
                throw new ArgumentNullException(nameof(matchingCriteria));

            _matchingCriteria = matchingCriteria;
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            return _matchingCriteria;
        }

        #endregion

    }
}
