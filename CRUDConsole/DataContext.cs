using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CRUDConsole
{
    public class DataContext : DbContext
    {
        public DbSet<Good> Goods { get; set; }  
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Good>().HasData(
                new Good() { Id = 1, Name = "Стол", Price = 25m, Count = 5 },
                new Good() { Id = 2, Name = "Диван", Price = 55m, Count = 3 },
                new Good() { Id = 3, Name = "Кресло", Price = 45m, Count = 2 }
            );
        }
    }
}
