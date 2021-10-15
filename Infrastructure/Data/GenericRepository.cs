using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _context;

        public GenericRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _context.Set<T>()
            .ToListAsync();
        }

        public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
        {
           // var query =  _context.Set<T>().Where(spec.Criteria);
           // query = spec.Includes.Aggregate(query, (c,i) => c.Include(i));
            //return await query.FirstOrDefaultAsync();
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }
        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            //var query = _context.Set<T>().AsQueryable();
            // query = spec.Includes.Aggregate(query, (c,i) => c.Include(i));
           // return await query.ToListAsync();
           return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        private IQueryable<T> ApplySpecification (ISpecification<T> spec)
        {
            /*var query = _context.Set<T>().AsQueryable();

            if(spec.Criteria != null)
            {
                query = query.Where(spec.Criteria); 
            }

            query = spec.Includes.Aggregate(query, (c, i) => c.Include(i));
            return query;*/
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }

    }
}