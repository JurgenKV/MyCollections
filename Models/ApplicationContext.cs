using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MyCollections.Models
{
    
    public class ApplicationContext : IdentityDbContext<User>
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ItemComment>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> User { get; set; }
        public DbSet<UserCollection> UserCollections { get; set; }
        public DbSet<CollectionItem> CollectionItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<ItemComment> ItemComments { get; set; }
        public DbSet<ItemLike> ItemLikes { get; set; }

    }
}
