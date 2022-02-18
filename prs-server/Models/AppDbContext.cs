using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prs_server.Models {
    public class AppDbContext : DbContext {

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {

            builder.Entity<User>(a => { // makes username unique
                a.HasIndex(p => p.Username)
                        .IsUnique(true);
            });

            builder.Entity<Vendor>(c => { // makes code unique
                c.HasIndex(d => d.Code)
                        .IsUnique(true);
            });
            
            builder.Entity<Product>(v => {
                v.HasIndex(o => o.PartNbr)
                        .IsUnique(true);
            });
        }
    }
}
