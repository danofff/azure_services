using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MyFaceIdApp
{
    public class GenericRepository : DbContext 
    {
        public GenericRepository() : base("DbConnection")
        {


        }

        public DbSet<UserModel> Users { get; set; }
    }
}
