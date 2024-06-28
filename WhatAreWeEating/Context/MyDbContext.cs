using Microsoft.EntityFrameworkCore;
using WhatAreWeEating.Models;

namespace WhatAreWeEating.Context
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDish> UserDishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints here if needed
            modelBuilder.Entity<UserDish>()
                .HasKey(ud => ud.Id);

            modelBuilder.Entity<UserDish>()
                .HasOne(ud => ud.User)
                .WithMany()
                .HasForeignKey(ud => ud.UserId);

            modelBuilder.Entity<UserDish>()
                .HasOne(ud => ud.Dish)
                .WithMany()
                .HasForeignKey(ud => ud.DishId);
        }
    }
}

