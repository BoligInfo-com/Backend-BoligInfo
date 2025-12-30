
using BoligInfo.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BoligInfo.Database
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Loan> Loans { get; set; }
    }
}