using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QuanLyGhiChu.Models
{
    public partial class QuanLyGhiChuContext : DbContext
    {
        public QuanLyGhiChuContext()
        {
        }

        public QuanLyGhiChuContext(DbContextOptions<QuanLyGhiChuContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ghichu> Ghichu { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=103.207.37.8;Database=QuanLyGhiChu;Integrated Security=False;User Id=sa;Password=tmdt@123456;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ghichu>(entity =>
            {
                entity.ToTable("GHICHU");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.HashCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.HienAn).HasDefaultValueSql("((1))");

                entity.Property(e => e.Password).HasMaxLength(30);

                entity.Property(e => e.TimeCreated).HasColumnType("datetime");

                entity.Property(e => e.TimeUpdated).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}
