using Demo.BLL.Interface;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repository
{
    public class DepartmentRespository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly DemoCompanyContext _context;

        public DepartmentRespository(DemoCompanyContext context) : base(context) 
        {
            _context = context;
        }

        public IQueryable<Department> Search(string name)
            => _context.Departments.Where(d => d.Name.Trim().ToLower().Contains(name.Trim().ToLower()));
        
    }
}
