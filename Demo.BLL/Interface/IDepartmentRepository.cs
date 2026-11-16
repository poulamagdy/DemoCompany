using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interface
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        public IQueryable<Department> Search(string name);
    }
}
