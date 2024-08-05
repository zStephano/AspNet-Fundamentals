using FundamentosAspNetProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FundamentosAspNetProject.Data
{
    public class DataContext : DbContext
    {
        public DbSet<TodoModel> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");
    }
}
