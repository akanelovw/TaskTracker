using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TestWebApp.Models;

namespace TestWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    { 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 

        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployee { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Project>(entity =>
            {
                entity.HasOne(p => p.Manager)
                .WithMany(u => u.ManagedProjects)
                .HasForeignKey(p => p.ManagerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(p => p.Objectives)
                .WithOne(u => u.Project)
                .HasForeignKey(p => p.ProjectId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Employees)
                .WithMany(u => u.Projects)
                .UsingEntity<ProjectEmployee>();

                entity.HasMany(p => p.Documents)
                .WithOne(p => p.Project)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Objective>(entity =>
            {
                entity.HasOne(p => p.Author)
                .WithMany(u => u.Objectives)
                .HasForeignKey(p => p.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.AssignedEmployee)
                .WithMany(u => u.AssignedObjectives)
                .IsRequired(false);
                
            });
        }

    }
}
