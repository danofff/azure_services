using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace code_first_database
{
    class AppDbContext : DbContext
    {
        public AppDbContext():base("name=MyModel")
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Department d1 = new Department { DepartmentId = 1, DepartmentName = "accountant" };
            Employee e1 = new Employee { EmployeeId = 1, EmployeeName = "sam collins", DepartmentId=1 };
            AppDbContext context = new AppDbContext();

            context.Departments.Add(d1);
            context.Employees.Add(e1);

            context.SaveChanges();
        }
    }
}
