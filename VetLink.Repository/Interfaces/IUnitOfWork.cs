using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;

namespace VetLink.Repository.Interfaces
{
    public interface IUnitOfWork: IDisposable
	{
        Task<int> SaveChangesAsync();
        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class;
		Task<IDbContextTransaction> BeginTransactionAsync();
		Task CommitAsync();
		Task RollbackAsync();
	}
}
