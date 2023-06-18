using System;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;

#nullable disable

namespace Infrastructure.Context
{
    public partial class ZermosContext : DbContext
    {
        public ZermosContext()
        {
        }

        public ZermosContext(DbContextOptions<ZermosContext> options) : base(options)
        {
        }

        public virtual DbSet<user> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string server = Environment.GetEnvironmentVariable("DB_HOST");
                string port = Environment.GetEnvironmentVariable("DB_PORT");
                string user = Environment.GetEnvironmentVariable("DB_USER");
                string password = Environment.GetEnvironmentVariable("DB_PASSWORD");
                string database = Environment.GetEnvironmentVariable("DB_NAME");


                optionsBuilder.UseMySQL(
                    $"server={server};user={user};password={password};database={database};port={port};Connect Timeout=5;");
            }
        }
    }
}