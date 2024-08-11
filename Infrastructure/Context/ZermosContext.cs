using System;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using MySql.Data.MySqlClient;

namespace Infrastructure.Context
{
    public class ZermosContext : DbContext
    {
        public ZermosContext() { }
        public ZermosContext(DbContextOptions<ZermosContext> options) : base(options) {}

        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<share> shares { get; set; }
    }
}