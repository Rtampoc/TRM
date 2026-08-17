using Microsoft.EntityFrameworkCore;
using TRM.Models;

namespace TRM.Data
{
    /// <summary>
    /// Entity Framework Core database context for the TRM application
    /// Manages connection to PTI_ERP database and models
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ToolType> ToolTypes { get; set; } = null!;
        public DbSet<RegisteredBy> RegisteredByList { get; set; } = null!;
        public DbSet<ReasonForRevRep> ReasonsForRevRep { get; set; } = null!;
        public DbSet<NotedBy> NotedByList { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Users entity to map to the TRMS_Users table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("TRMS_Users", "dbo");

                // Primary key maps to Id (int)
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedOnAdd();

                // Username column in DB is 'UserId' (varchar)
                entity.Property(e => e.UserId)
                    .HasColumnName("UserId")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Password)
                    .HasColumnName("Password")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.EmployeeName)
                    .HasColumnName("EmployeeName")
                    .HasMaxLength(100);

                entity.Property(e => e.Department)
                    .HasColumnName("Department")
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(255);

                entity.Property(e => e.Role)
                    .HasColumnName("Role")
                    .HasMaxLength(100);

                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive")
                    .HasDefaultValue(true);

                entity.Property(e => e.DateCreated)
                    .HasColumnName("DateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastLogin)
                    .HasColumnName("LastLogin")
                    .HasColumnType("datetime");

                // Index on username for faster lookups
                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("IX_TRMS_Users_UserId");
            });

            // Configure ToolType entity
            modelBuilder.Entity<ToolType>(entity =>
            {
                entity.ToTable("TRMS_ToolType", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("ToolType").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
            });

            // Configure RegisteredBy entity
            modelBuilder.Entity<RegisteredBy>(entity =>
            {
                entity.ToTable("TRMS_RegisteredBy", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("RegisteredBy").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
            });

            // Configure ReasonForRevRep entity
            modelBuilder.Entity<ReasonForRevRep>(entity =>
            {
                entity.ToTable("TRMS_ReasonForRevRep", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("ReasonForRevRep").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
            });

            // Configure NotedBy entity
            modelBuilder.Entity<NotedBy>(entity =>
            {
                entity.ToTable("TRMS_NotedBy", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("NotedBy").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("TRMS_Customer", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.BPCode).HasColumnName("BPCode").HasMaxLength(50);
                entity.Property(e => e.BPName).HasColumnName("BPName").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("TRMS_Category", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("Category").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
            });
        }
    }
}
