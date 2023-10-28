using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        // 30, LINQ on DbSet<> -> SQL statements
        // 30, Add connectionString inside appsettings.json
        // 30, Connect the ApplicationDbContext with connectionString. That way it knows where is the SQL database and we need to add dependency injection
        // 30, goto Program.cs
        // 30, Pass connectionString to DbContext. DbContextOptions is Injected from AddDbContext<AppDbContext>()
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Villa> Villas { get; set; }
    }
}
