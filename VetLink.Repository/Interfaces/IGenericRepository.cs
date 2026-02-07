using System;
using System.Collections.Generic;
using System.Text;
using VetLink.Repository.Specifications;

namespace VetLink.Repository.Interfaces
{
    public interface IGenericRepository<TEntity,Tkey> where TEntity:class
    {
		// Basic CRUD operations
		Task<TEntity?> GetByIdAsync(Tkey id);
		Task<TEntity?> GetByIdWithSpecAsync(ISpecification<TEntity> spec);
		Task<IReadOnlyList<TEntity>> ListAllAsync();
		Task<IReadOnlyList<TEntity>> ListAllWithSpecAsync(ISpecification<TEntity> spec );
		Task<TEntity> AddAsync(TEntity entity);
		Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);
		void Update(TEntity entity);
		void Delete(TEntity entity);

		// Utility methods
		Task<int> CountAsync();
		Task<int> CountWithSpecAsync(ISpecification<TEntity> spec );
		Task<bool> ExistsAsync(Tkey id  );
		Task<string> GenerateOrderNumberAsync();
	}
}
