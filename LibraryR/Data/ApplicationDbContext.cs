using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryR.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryR.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>()
                .HasKey(c => new { c.BookId, c.UserId });

            modelBuilder.Entity<Author>()
                .HasKey(a => new { a.Id, a.UserId });

            modelBuilder.Entity<Book>()
                .HasKey(b => new { b.Id, b.UserId });

            modelBuilder.Entity<StatusType>()
                .HasData(new StatusType {Id = 1, Type = "Readed" }, new StatusType {Id=2, Type = "Reading it" }, new StatusType {Id=3, Type = "To read" });
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Record> Records { get; set; }

        public DbSet<StatusType> StatusTypes { get; set; }

        public DbSet<LibraryR.Models.ProjectRole> ProjectRole { get; set; }
    }
}
