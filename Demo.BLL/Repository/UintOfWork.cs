using Demo.BLL.Interface;
using Demo.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repository
{
    public class UintOfWork : IUintOfWork
    {
        private readonly DemoCompanyContext _context;
        public IEmployeeRepository EmployeeRepository { get; set; }
        public IDepartmentRepository DepartmentRepository { get; set ; }

        public UintOfWork(DemoCompanyContext context)
        {
            EmployeeRepository = new EmployeeRepositor(context);
            DepartmentRepository = new DepartmentRespository(context);
            _context = context;
        }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();
        public void Dispose()
            => _context.Dispose();
    }
}
