using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace service.Models
{
    public partial class dsb_bad_dogContext : DbContext
    {
        public dsb_bad_dogContext()
        {
        }

        public dsb_bad_dogContext(DbContextOptions<dsb_bad_dogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Reciept> Reciepts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;database=dsb_bad_dog;uid=root;password=adminadmin", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Reciept>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PRIMARY");

                entity.ToTable("reciepts");

                entity.Property(e => e.OrderId)
                    .ValueGeneratedNever()
                    .HasColumnName("orderId");

                entity.Property(e => e.DepDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("depDateTime");

                entity.Property(e => e.DepSt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("depSt");

                entity.Property(e => e.DesDateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("desDateTime");

                entity.Property(e => e.DesSt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("desSt");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("email");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
