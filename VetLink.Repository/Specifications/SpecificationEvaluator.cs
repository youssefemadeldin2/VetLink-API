using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace VetLink.Repository.Specifications
{
    internal class SpecificationEvaluator<T> where T :class
	{
		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
		{
			var query = inputQuery;

			// If tracking is disabled, use AsNoTracking
			if (!specification.IsTracking)
			{
				query = query.AsNoTracking();
			}

			// Filter by criteria
			if (specification.Criteria != null)
			{
				query = query.Where(specification.Criteria);
			}

			// Include related entities
			query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
			query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

			// Apply ordering
			if (specification.OrderBy != null)
			{
				query = query.OrderBy(specification.OrderBy);
			}
			else if (specification.OrderByDescending != null)
			{
				query = query.OrderByDescending(specification.OrderByDescending);
			}

			// Apply then ordering
			var orderedQuery = query as IOrderedQueryable<T>;
			if (orderedQuery != null && specification.ThenBy != null)
			{
				query = orderedQuery.ThenBy(specification.ThenBy);
			}
			else if (orderedQuery != null && specification.ThenByDescending != null)
			{
				query = orderedQuery.ThenByDescending(specification.ThenByDescending);
			}

			// Apply paging
			if (specification.IsPagingEnabled)
			{
				query = query.Skip(specification.Skip).Take(specification.Take);
			}

			return query;
		}
	}
}
