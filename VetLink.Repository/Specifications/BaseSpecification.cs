using System.Linq.Expressions;

namespace VetLink.Repository.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>>? criteria = null)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>>? Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public Expression<Func<T, object>>? ThenBy { get; private set; }
        public Expression<Func<T, object>>? ThenByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;
        public bool IsTracking { get; private set; } = false;
        public bool IsDistinct { get; private set; }

		
		protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }
        protected virtual void ApplyThenBy(Expression<Func<T, object>> thenByExpression)
        {
            ThenBy = thenByExpression;
        }
        protected virtual void ApplyThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
        {
            ThenByDescending = thenByDescendingExpression;
        }
        protected virtual void EnableTracking(bool isTracking)
        {
            IsTracking = isTracking;
        }
        protected void ApplyDistinct()
        {
            IsDistinct = true;
        }
    }
}
