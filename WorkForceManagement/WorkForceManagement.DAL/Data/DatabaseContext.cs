using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.DAL.Data
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<TimeOffRequest> TimeOffRequests { get; set; }
        public DbSet<Approvals> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>().HasOne(t => t.TeamLeader).WithMany(u=>u.Teams);

            modelBuilder.Entity<TimeOffRequest>().HasOne(t => t.Requester);

            modelBuilder.Entity<TimeOffRequest>().HasMany(t => t.Approvers).WithMany(u => u.TimeOffRequestsToApprove).UsingEntity(join => join.ToTable("UserTimeOffRequestsToApprove"));

            modelBuilder.Entity<TimeOffRequest>().HasMany(t => t.AlreadyApproved).WithMany(u => u.TimeOffRequestsApproved).UsingEntity(join => join.ToTable("UserTimeOffRequestsAlreadyApproved"));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
