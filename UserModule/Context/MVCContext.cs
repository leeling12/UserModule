using Microsoft.EntityFrameworkCore;
using UserModule.Models;

namespace UserModule.Context
{
    public class MVCContext: DbContext
    {
        public MVCContext(DbContextOptions<MVCContext> options): base(options)
        {

        }

        //create dbset
        public DbSet<User> Users { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Team> Teams { get; set; }

    }
}
