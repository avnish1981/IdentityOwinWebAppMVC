using IdentityOwinWebApp.Demo.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityOwinWebApp.Demo.EntityFramework
{
    public class ExtendedUserDbContext:IdentityDbContext<ExtendingUser>
    {
        public ExtendedUserDbContext(string connectionString) :base(connectionString )
        {

        }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var address = modelBuilder.Entity<Address>();
            address.ToTable("AspNetUserAddress");
            address.HasKey(x => x.Id);

            var user = modelBuilder.Entity<ExtendingUser>();
            user.Property(x => x.UserName  ).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("FullNameIndex")));
            user.HasMany(x => x.Addresses).WithRequired().HasForeignKey(x => x.UserId);


        }
    }
}