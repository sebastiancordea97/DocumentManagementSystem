using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataAccessLibrary.Models
{
    public partial class LicentaTestContext : DbContext
    {
        public LicentaTestContext()
        {
        }

        public LicentaTestContext(DbContextOptions<LicentaTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<DocumentsSigners> DocumentsSigners { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LicentaTest;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepId)
                    .HasName("PK__Departme__DB9CAA5F6FE52165");

                entity.Property(e => e.DepName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.SupervisorNavigation)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => d.Supervisor)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Departmen__Super__2B3F6F97");
            });

            modelBuilder.Entity<Documents>(entity =>
            {
                entity.HasKey(e => e.DocId)
                    .HasName("PK__Document__3EF188AD21D3F539");

                entity.Property(e => e.DocumentPath)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Status)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('PENDING')");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Documents__UserI__2E1BDC42");
            });

            modelBuilder.Entity<DocumentsSigners>(entity =>
            {
                entity.HasKey(e => e.SignerId)
                    .HasName("PK__Document__E5C65A98DCE45E36");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentsSigners)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Documents__Docum__5AEE82B9");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__1788CC4C364ADC42");

                entity.HasIndex(e => e.Email)
                    .HasName("UniqueEmail")
                    .IsUnique();

                entity.HasIndex(e => e.UserFunction)
                    .HasName("UQ__Users__459DE536BE7D2652")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Salt)
                    .HasColumnName("salt")
                    .HasMaxLength(128);

                entity.Property(e => e.SupervisorId).HasColumnName("Supervisor_id");

                entity.Property(e => e.UserFunction)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('not set')");

                entity.Property(e => e.Userrank)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('not set')");

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Users__Departmen__3C69FB99");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleId__286302EC");

                entity.HasOne(d => d.Supervisor)
                    .WithMany(p => p.InverseSupervisor)
                    .HasForeignKey(d => d.SupervisorId)
                    .HasConstraintName("FK__Users__Superviso__3D5E1FD2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
