using System;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using MySql.Data.MySqlClient;

namespace Infrastructure.Context
{
    public class ZermosContext : DbContext
    {
        public bool DatabaseAvailable { get; set; } = false;

        public ZermosContext()
        {
        }

        public ZermosContext(DbContextOptions<ZermosContext> options) : base(options)
        {
        }

        public virtual DbSet<user> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // if (!optionsBuilder.IsConfigured)
            // {
            //     var server = Environment.GetEnvironmentVariable("DB_HOST");
            //     var port = Environment.GetEnvironmentVariable("DB_PORT");
            //     var user = Environment.GetEnvironmentVariable("DB_USER");
            //     var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            //     var database = Environment.GetEnvironmentVariable("DB_NAME");
            //
            //     var connectionString = $"server={server};user={user};password={password};database={database};port={port};Connect Timeout=5;";
            //
            //     if (IsDatabaseConnectionValid(connectionString))
            //     {
            //         optionsBuilder.UseMySQL(connectionString);
            //     }
            // }

            // "DB_PASSWORD": "REDACTED_DATABASE_PASSWORD",
            // "DB_NAME": "REDACTED_DATABASE_NAME",
            // "DB_PORT": "3306",
            // "DB_HOST": "REDACTED_IP_ADRESS",
            // "DB_USER": "root"
             optionsBuilder.UseMySQL("server=REDACTED_IP_ADRESS;user=root;password=REDACTED_DATABASE_PASSWORD;database=REDACTED_DATABASE_NAME;port=3306;Connect Timeout=5;");
        }

        private bool IsDatabaseConnectionValid(string connectionString)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                DatabaseAvailable = true;
                return true;
            }
            catch
            {
                DatabaseAvailable = false;
                return false;
            }
        }
    }
}