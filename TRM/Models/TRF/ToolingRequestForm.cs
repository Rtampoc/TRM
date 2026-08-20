using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TRM.Models;

namespace TRM.Models.TRF
{
    /// <summary>
    /// Tooling Request Form (TRF) model representing the main TRF document
    /// Maps to TRMS_ToolingRequestForm table
    /// </summary>
    public class ToolingRequestForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Document Details
        [Required]
        [MaxLength(50)]
        public string TRFNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CPIPNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TRFStatus { get; set; } = string.Empty;

        public DateTime? DateRequested { get; set; }
        public DateTime? FASubmission { get; set; }
        public DateTime? MouldAvailability { get; set; }

        // Customer & Product
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        public int? NoCavities { get; set; }
        public int? FullCavities { get; set; }

        // Ownership / Payment
        [MaxLength(50)]
        public string Ownership { get; set; } = string.Empty; // Customer Owned, Primepack Owned, etc.

        [MaxLength(50)]
        public string POBasedStatus { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PONumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AmortizationStatus { get; set; } = string.Empty;

        public int? NumberOfTrays { get; set; }

        [MaxLength(50)]
        public string ConfirmedQuotationRefNo { get; set; } = string.Empty;

        // Tool Info
        public int? ToolTypeId { get; set; }
        [ForeignKey("ToolTypeId")]
        public virtual ToolType? ToolType { get; set; }

        [MaxLength(255)]
        public string OtherToolType { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public int? ReasonForRevRepId { get; set; }
        [ForeignKey("ReasonForRevRepId")]
        public virtual ReasonForRevRep? ReasonForRevRep { get; set; }

        // Workflow / Signatures
        public int? PreparedById { get; set; }
        [ForeignKey("PreparedById")]
        public virtual RegisteredBy? PreparedBy { get; set; }

        public int? NotedById { get; set; }
        [ForeignKey("NotedById")]
        public virtual NotedBy? NotedBy { get; set; }

        public int? ReviewedById { get; set; }
        [ForeignKey("ReviewedById")]
        public virtual RegisteredBy? ReviewedBy { get; set; }

        public int? ApprovedByPrimepackId { get; set; }
        [ForeignKey("ApprovedByPrimepackId")]
        public virtual RegisteredBy? ApprovedByPrimepack { get; set; }

        public int? RegisteredById { get; set; }
        [ForeignKey("RegisteredById")]
        public virtual RegisteredBy? RegisteredBy { get; set; }

        public int? NotedByFinanceId { get; set; }
        [ForeignKey("NotedByFinanceId")]
        public virtual RegisteredBy? NotedByFinance { get; set; }

        // Form State
        [MaxLength(50)]
        public string FormStatus { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected

        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }

        // Navigation
        public virtual ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();
    }
}
