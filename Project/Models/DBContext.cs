using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Project.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }

        public virtual DbSet<account> accounts { get; set; }
        public virtual DbSet<booking> bookings { get; set; }
        public virtual DbSet<equipment> equipments { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<room> rooms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<account>()
                .HasMany(e => e.bookings)
                .WithOptional(e => e.account)
                .HasForeignKey(e => e.user_id);

            modelBuilder.Entity<account>()
                .HasMany(e => e.rooms)
                .WithOptional(e => e.account)
                .HasForeignKey(e => e.leader_id);

            modelBuilder.Entity<equipment>()
                .Property(e => e.equipment_number)
                .IsUnicode(false);

            modelBuilder.Entity<role>()
                .Property(e => e.role_name)
                .IsUnicode(false);

            modelBuilder.Entity<role>()
                .HasMany(e => e.accounts)
                .WithOptional(e => e.role)
                .HasForeignKey(e => e.role_id);

            modelBuilder.Entity<room>()
                .HasMany(e => e.bookings)
                .WithOptional(e => e.room)
                .HasForeignKey(e => e.room_id);

            modelBuilder.Entity<room>()
                .HasMany(e => e.equipments)
                .WithOptional(e => e.room)
                .HasForeignKey(e => e.room_id);
        }
    }
}
