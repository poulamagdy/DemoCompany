using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interface
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Delete(T entity);
        void Update(T entity);
        Task<T> GetByIdAsync(int? id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
