using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication8.Models;

namespace WebApplication8.Data
{

    public class FaunaContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public FaunaContext(DbContextOptions<FaunaContext> options)
            : base(options) { }

        public DbSet<Species> Species { get; set; }
        public DbSet<Animals> Animals { get; set; }
        public DbSet<Enclosures> Enclosures { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Feeding> Feeding { get; set; }
        public DbSet<MedicalCheckups> MedicalCheckups { get; set; }
        public DbSet<Visitors> Visitors { get; set; }
        public DbSet<Tickets> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // обязательно сначала вызываем базовый метод для Identity

            // Первичные ключи твоих таблиц
            modelBuilder.Entity<Animals>().HasKey(a => a.Animal_ID);
            modelBuilder.Entity<Species>().HasKey(s => s.Species_ID);
            modelBuilder.Entity<Enclosures>().HasKey(e => e.Enclosure_ID);
            modelBuilder.Entity<Employees>().HasKey(emp => emp.Employee_ID);
            modelBuilder.Entity<Feeding>().HasKey(f => f.Feeding_ID);
            modelBuilder.Entity<MedicalCheckups>().HasKey(m => m.Checkup_ID);
            modelBuilder.Entity<Visitors>().HasKey(v => v.Visitor_ID);
            modelBuilder.Entity<Tickets>().HasKey(t => t.Ticket_ID);

            // Связи
            modelBuilder.Entity<Animals>()
                .HasOne(a => a.Enclosure)
                .WithMany(e => e.Animals)
                .HasForeignKey(a => a.Enclosure_ID);

            modelBuilder.Entity<Animals>()
                .HasOne(a => a.Species)
                .WithMany(s => s.Animals)
                .HasForeignKey(a => a.Species_ID);

            modelBuilder.Entity<Employees>()
                .HasOne(e => e.Enclosure)
                .WithMany(en => en.Employees)
                .HasForeignKey(e => e.Enclosure_ID);

            modelBuilder.Entity<Feeding>()
                .HasOne(f => f.Animal)
                .WithMany(a => a.Feeding)
                .HasForeignKey(f => f.Animal_ID);

            modelBuilder.Entity<MedicalCheckups>()
                .HasOne(m => m.Animal)
                .WithMany(a => a.MedicalCheckups)
                .HasForeignKey(m => m.Animal_ID);

            modelBuilder.Entity<MedicalCheckups>()
                .HasOne(m => m.Employee)
                .WithMany(e => e.MedicalCheckups)
                .HasForeignKey(m => m.Employee_ID);

            modelBuilder.Entity<Tickets>()
                .HasOne(t => t.Visitor)
                .WithMany(v => v.Tickets)
                .HasForeignKey(t => t.Visitor_ID);
        }
    }
}
