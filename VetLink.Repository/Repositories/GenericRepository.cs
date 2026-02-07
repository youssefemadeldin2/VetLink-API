using Microsoft.EntityFrameworkCore;
using VetLink.Data.Contexts;
using VetLink.Repository.Interfaces;
using VetLink.Repository.Specifications;

namespace VetLink.Repository.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        private readonly VetLinkDbContext _context;
        public GenericRepository(VetLinkDbContext context)
        {
            _context = context;
        }
        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec, bool forCount = false)
            => SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), spec);
        public async Task<TEntity?> GetByIdAsync(TKey? id)
            => await _context.Set<TEntity>().FindAsync(id);
        public async Task<TEntity?> GetByIdWithSpecAsync(ISpecification<TEntity> spec)
            => await ApplySpecification(spec).FirstOrDefaultAsync();
        public async Task<string> GenerateOrderNumberAsync()
        {
            var nextVal = await _context.Database
                .SqlQueryRaw<long>("SELECT NEXT VALUE FOR OrderNumberSequence")
                .SingleAsync();

            return $"ORD-{nextVal:D8}";
        }

        public async Task<IReadOnlyList<TEntity>> ListAllAsync()
            => await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        public async Task<IReadOnlyList<TEntity>> ListAllWithSpecAsync(ISpecification<TEntity> spec)
            => await ApplySpecification(spec).ToListAsync();
        public async Task<int> CountAsync()
            => await _context.Set<TEntity>().CountAsync();

        public async Task<int> CountWithSpecAsync(ISpecification<TEntity> spec)
                => await ApplySpecification(spec, true).CountAsync();
        public void Delete(TEntity entity)
            => _context.Remove(entity);
        public void Update(TEntity entity)
            => _context.Set<TEntity>().Update(entity);
        public async Task<bool> ExistsAsync(TKey id)
            => await _context.Set<TEntity>().FindAsync(id) != null;

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }
        public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            return entities;
        }

    }
}