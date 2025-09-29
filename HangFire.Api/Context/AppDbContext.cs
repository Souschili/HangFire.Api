using HangFire.Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace HangFire.Api.Context
{
    public class AppDbContext:DbContext
    {
        public DbSet<DailyReport> DailyReports => Set<DailyReport>(); 
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        
    }
}
