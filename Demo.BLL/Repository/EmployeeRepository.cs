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
    public class EmployeeRepositor : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly DemoCompanyContext _context;

        public EmployeeRepositor(DemoCompanyContext context) : base(context) 
        {
            _context = context;
        }

        public IQueryable<Employee> Search(string gmail)
            => _context.Employees.Where(e => e.Email.Trim().ToLower().Contains(gmail.Trim().ToLower()));
    }
}
