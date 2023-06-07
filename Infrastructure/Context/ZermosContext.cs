using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;

#nullable disable

namespace Infrastructure.Context
{
    public partial class ZermosContext : DbContext
    {
        public ZermosContext() { }
        public ZermosContext(DbContextOptions<ZermosContext> options) : base(options) { }

        public virtual DbSet<user> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("server=REDACTED_IP_ADRESS;user=root;password=REDACTED_DATABASE_PASSWORD;database=REDACTED_DATABASE_NAME;port=32768;Connect Timeout=5;");
            }
        }
    }
}
