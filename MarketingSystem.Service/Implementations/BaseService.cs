using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MarketingSystem.Common.Interfaces;
using MarketingSystem.Model;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingSystem.Service.Implementations
{
    public abstract class BaseService<TEntity> : IService<TEntity>
        where TEntity : BaseEntity 
    {
        private readonly AppDbContext _context;

        public BaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            var entry = _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<TEntity> Get(Guid id)
        {
            var entity = await _context.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IList<TEntity>> GetAll()
        {
            var entities = await _context.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
            return entities;
        }

        public async Task<IList<TEntity>> GetMany(IList<Guid> entityIds)
        {
            var entities = await _context.Set<TEntity>()
                .AsNoTracking()
                .Where(x => entityIds.Contains(x.Id))
                .ToListAsync();
            return entities;
        }

        public async Task Update(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        public async Task Remove(TEntity entity, Guid? updaterId)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMultiple(IList<TEntity> entities, Guid? updaterId)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }

            await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>> filterExpression = null)
        {
            if (filterExpression == null) return _context.Set<TEntity>().AsNoTracking();
            return _context.Set<TEntity>().AsNoTracking().Where(filterExpression);
        }
    }
}