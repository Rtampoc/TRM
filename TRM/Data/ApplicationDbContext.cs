using Microsoft.EntityFrameworkCore;
using TRM.Models;
using TRM.Models.TRF;

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

        // Tooling Request Form entities
        public DbSet<ToolingRequestForm> ToolingRequestForms { get; set; } = null!;
        public DbSet<LineItem> LineItems { get; set; } = null!;
        public DbSet<SignatureApproval> SignatureApprovals { get; set; } = null!;

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

            // Configure ToolingRequestForm entity
            modelBuilder.Entity<ToolingRequestForm>(entity =>
            {
                entity.ToTable("TRMS_ToolingRequestForm", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.TRFNo).HasColumnName("TRFNo").HasMaxLength(50).IsRequired();
                entity.Property(e => e.CPIPNumber).HasColumnName("CPIPNumber").HasMaxLength(50);
                entity.Property(e => e.TRFStatus).HasColumnName("TRFStatus").HasMaxLength(50);
                entity.Property(e => e.DateRequested).HasColumnName("DateRequested").HasColumnType("datetime");
                entity.Property(e => e.FASubmission).HasColumnName("FASubmission").HasColumnType("datetime");
                entity.Property(e => e.MouldAvailability).HasColumnName("MouldAvailability").HasColumnType("datetime");
                entity.Property(e => e.Model).HasColumnName("Model").HasMaxLength(100);
                entity.Property(e => e.NoCavities).HasColumnName("NoCavities");
                entity.Property(e => e.FullCavities).HasColumnName("FullCavities");
                entity.Property(e => e.Ownership).HasColumnName("Ownership").HasMaxLength(50);
                entity.Property(e => e.POBasedStatus).HasColumnName("POBasedStatus").HasMaxLength(50);
                entity.Property(e => e.PONumber).HasColumnName("PONumber").HasMaxLength(50);
                entity.Property(e => e.AmortizationStatus).HasColumnName("AmortizationStatus").HasMaxLength(50);
                entity.Property(e => e.NumberOfTrays).HasColumnName("NumberOfTrays");
                entity.Property(e => e.ConfirmedQuotationRefNo).HasColumnName("ConfirmedQuotationRefNo").HasMaxLength(50);
                entity.Property(e => e.OtherToolType).HasColumnName("OtherToolType").HasMaxLength(255);
                entity.Property(e => e.FormStatus).HasColumnName("FormStatus").HasMaxLength(50).HasDefaultValue("Draft");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
                entity.Property(e => e.DateModified).HasColumnName("DateModified").HasColumnType("datetime");

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId);

                entity.HasOne(e => e.ToolType)
                    .WithMany()
                    .HasForeignKey(e => e.ToolTypeId);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId);

                entity.HasOne(e => e.ReasonForRevRep)
                    .WithMany()
                    .HasForeignKey(e => e.ReasonForRevRepId);

                entity.HasOne(e => e.PreparedBy)
                    .WithMany()
                    .HasForeignKey(e => e.PreparedById);

                entity.HasOne(e => e.NotedBy)
                    .WithMany()
                    .HasForeignKey(e => e.NotedById);

                entity.HasOne(e => e.ApprovedByPrimepack)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedByPrimepackId);

                entity.HasOne(e => e.RegisteredBy)
                    .WithMany()
                    .HasForeignKey(e => e.RegisteredById);

                entity.HasOne(e => e.NotedByFinance)
                    .WithMany()
                    .HasForeignKey(e => e.NotedByFinanceId);

                entity.HasMany(e => e.LineItems)
                    .WithOne(li => li.ToolingRequestForm)
                    .HasForeignKey(li => li.TRFId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure LineItem entity
            modelBuilder.Entity<LineItem>(entity =>
            {
                entity.ToTable("TRMS_LineItem", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.TRFId).HasColumnName("TRFId");
                entity.Property(e => e.LineNumber).HasColumnName("LineNumber");
                entity.Property(e => e.JONumber).HasColumnName("JONumber").HasMaxLength(50);
                entity.Property(e => e.ToolDescriptor).HasColumnName("ToolDescriptor").HasMaxLength(255);
                entity.Property(e => e.Length).HasColumnName("Length").HasColumnType("decimal(18,2)");
                entity.Property(e => e.Width).HasColumnName("Width").HasColumnType("decimal(18,2)");
                entity.Property(e => e.Height).HasColumnName("Height").HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalKgs).HasColumnName("TotalKgs").HasColumnType("decimal(18,2)");
                entity.Property(e => e.EstMachineCost).HasColumnName("EstMachineCost").HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaterialCost).HasColumnName("MaterialCost").HasColumnType("decimal(18,2)");
                entity.Property(e => e.MachiningCostPHP).HasColumnName("MachiningCostPHP").HasColumnType("decimal(18,2)");
                entity.Property(e => e.TestingCost).HasColumnName("TestingCost").HasColumnType("decimal(18,2)");
                entity.Property(e => e.OtherCost).HasColumnName("OtherCost").HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCostPHP).HasColumnName("TotalCostPHP").HasColumnType("decimal(18,2)");
                entity.Property(e => e.MouldSelling).HasColumnName("MouldSelling").HasMaxLength(50);
                entity.Property(e => e.GPRate).HasColumnName("GPRate").HasColumnType("decimal(18,4)");
                entity.Property(e => e.Remarks).HasColumnName("Remarks").HasMaxLength(500);
                entity.Property(e => e.ToolType).HasColumnName("ToolType").HasMaxLength(50).HasDefaultValue("CNC");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
                entity.Property(e => e.DateModified).HasColumnName("DateModified").HasColumnType("datetime");
            });

            // Configure SignatureApproval entity
            modelBuilder.Entity<SignatureApproval>(entity =>
            {
                entity.ToTable("TRMS_SignatureApproval", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.TRFId).HasColumnName("TRFId");
                entity.Property(e => e.ApprovalRole).HasColumnName("ApprovalRole").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ApprovedByName).HasColumnName("ApprovedByName").HasMaxLength(255);
                entity.Property(e => e.ApprovalDate).HasColumnName("ApprovalDate").HasColumnType("datetime");
                entity.Property(e => e.ApprovalComments).HasColumnName("ApprovalComments").HasMaxLength(500);
                entity.Property(e => e.ApprovalStatus).HasColumnName("ApprovalStatus").HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime");
                entity.Property(e => e.DateModified).HasColumnName("DateModified").HasColumnType("datetime");
            });
        }
    }
}
