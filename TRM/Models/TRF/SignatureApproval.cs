using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRM.Models.TRF
{
    /// <summary>
    /// Signature Approval model representing workflow approvals on TRF
    /// Maps to TRMS_SignatureApproval table
    /// </summary>
    public class SignatureApproval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TRFId { get; set; }
        [ForeignKey("TRFId")]
        public virtual ToolingRequestForm? ToolingRequestForm { get; set; }

        [MaxLength(100)]
        [Required]
        public string ApprovalRole { get; set; } = string.Empty; // PreparedBy, NotedBy, ApprovedByPrimepack, RegisteredBy, NotedByFinance

        [MaxLength(255)]
        public string ApprovedByName { get; set; } = string.Empty;

        public DateTime? ApprovalDate { get; set; }

        [MaxLength(500)]
        public string ApprovalComments { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
    }
}
