using Demo.BLL.Interface;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly DemoCompanyContext _context;

        public GenericRepository(DemoCompanyContext context) // CLR
        {
            _context = context;
        }
        public async Task AddAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);

        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Employee))
            {
                return (IEnumerable<T>) await _context.Set<Employee>().Include(d => d.Department).ToListAsync();
            }        
            return await _context.Set<T>().ToListAsync();

        }
        public async Task<T> GetByIdAsync(int? id)
        {
            if (id == null)
                return null;
            return await _context.Set<T>().FindAsync(id.Value);
        }

        public void Update(T entity)
            => _context.Set<T>().Update(entity);
    }
}
