﻿using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        public DbSet<Category> categories { get; set; }

        public DbSet<CoverType> coverTypes { get; set; }

        public DbSet<Product> products { get; set; }
    }
}
