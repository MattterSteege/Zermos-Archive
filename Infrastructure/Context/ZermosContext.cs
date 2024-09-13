using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;

namespace Infrastructure.Context
{
    public class ZermosContext : DbContext
    {
        public ZermosContext()
        {
        }

        public ZermosContext(DbContextOptions<ZermosContext> options) : base(options)
        {
        }

        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<share> shares { get; set; }
        public DbSet<custom_appointment> custom_appointments { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //
        //     // // Configuratie voor de relatie tussen user en CustomAppointment
        //     // modelBuilder.Entity<custom_appointment>()
        //     //     .HasOne(ca => ca.User)
        //     //     .WithMany(u => u.custom_appointments)
        //     //     .HasForeignKey(ca => ca.UserEmail);
        // }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#if RELEASE && BETA
//                optionsBuilder.UseMySQL("server=REDACTED_IP_ADRESS;user=root;password=REDACTED_DATABASE_PASSWORD;database=zermos-beta-database;port=3306;Connect Timeout=5;");
//#elif RELEASE && !BETA
                optionsBuilder.UseMySQL("server=REDACTED_IP_ADRESS;user=root;password=REDACTED_DATABASE_PASSWORD;database=REDACTED_DATABASE_NAME;port=3306;Connect Timeout=5;");
//#else
//                optionsBuilder.UseMySQL("server=REDACTED_IP_ADRESS;user=root;password=REDACTED_DATABASE_PASSWORD;database=zermos-beta-database;port=3306;Connect Timeout=5;");
//#endif
                
            }
        }
    }
}