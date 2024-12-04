using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace OlsaMarketing.Models
{
    public class MarketingDepartment : DbContext
    {
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Task> Tasks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Campaign)
                .WithMany()
                .HasForeignKey(t => t.CampaignId);
            modelBuilder.Entity<Task>()
                .Property(t => t.Photos)
                .HasConversion(
                    v => string.Join(";", v),  // Преобразуем список строк в строку через ";"
                    v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList() // Разбиваем обратно в список строк
                );

        }

        public MarketingDepartment(DbContextOptions<MarketingDepartment> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
