using System.Collections;
using Microsoft.EntityFrameworkCore.Storage;
using VetLink.Data.Contexts;
using VetLink.Repository.Interfaces;

namespace VetLink.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VetLinkDbContext _context;
        private Hashtable _repositories;
        private bool _disposed = false;
        private IDbContextTransaction _transaction;
        public UnitOfWork(VetLinkDbContext context) => _context = context;

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var entityKey = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(entityKey))
            {
                // FIX: Use the concrete implementation, not the interface
                var repositoryType = typeof(GenericRepository<,>);

                // Create instance of GenericRepository<TEntity, TKey>
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(typeof(TEntity), typeof(TKey)),
                    _context);

                _repositories.Add(entityKey, repositoryInstance);
            }

            return (IGenericRepository<TEntity, TKey>)_repositories[entityKey];
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task RollbackAsync()
        {
			if (_transaction == null)
			{
				throw new InvalidOperationException("No active transaction to rollback");
			}

			await _transaction.RollbackAsync();
			await _transaction.DisposeAsync();
			_transaction = null;
		}
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
			if (_transaction != null)
			{
				throw new InvalidOperationException("A transaction is already in progress");
			}
			_transaction = await _context.Database.BeginTransactionAsync();
			return _transaction;
		}

        public async Task CommitAsync()
        {
            //await _context.SaveChangesAsync();
            //await _transaction.CommitAsync();
			try
			{
				if (_transaction == null)
				{
					throw new InvalidOperationException("No active transaction. Call BeginTransaction first.");
				}

				await _context.SaveChangesAsync();
				await _transaction.CommitAsync();
			}
			catch
			{
				await RollbackAsync();
				throw;
			}
			finally
			{
				if (_transaction != null)
				{
					await _transaction.DisposeAsync();
					_transaction = null;
				}
			}
		}

        public void Dispose()
        {
            Dispose(true);
            _context.Dispose();
        }
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed && disposing)
			{
				_transaction?.Dispose();
				_context?.Dispose();
			}
			_disposed = true;
		}
	}
}