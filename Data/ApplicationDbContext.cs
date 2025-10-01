using Microsoft.EntityFrameworkCore;
using PTSDProject.Models;

namespace PTSDProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CMSUser> CMSUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CMSUser>(entity =>
            {
                entity.ToTable("CMSUser");
                entity.HasKey(e => e.UserId);
            });
        }
    }
}
