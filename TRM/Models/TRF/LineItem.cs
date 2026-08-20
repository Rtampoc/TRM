using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TRM.Models.TRF
{
    /// <summary>
    /// Line Item model representing cost details for CNC and Knife Fabrication
    /// Maps to TRMS_LineItem table
    /// </summary>
    public class LineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TRFId { get; set; }
        [ForeignKey("TRFId")]
        public virtual ToolingRequestForm? ToolingRequestForm { get; set; }

        // Line item details
        public int? LineNumber { get; set; }

        [MaxLength(50)]
        public string JONumber { get; set; } = string.Empty;

        [MaxLength(255)]
        public string ToolDescriptor { get; set; } = string.Empty;

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public decimal? TotalKgs { get; set; }

        public decimal? EstMachineCost { get; set; }
        public decimal? MaterialCost { get; set; }
        public decimal? MachiningCostPHP { get; set; }
        public decimal? TestingCost { get; set; }
        public decimal? OtherCost { get; set; }

        public decimal? TotalCostPHP { get; set; }

        [MaxLength(50)]
        public string MouldSelling { get; set; } = string.Empty;

        public decimal? GPRate { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ToolType { get; set; } = "CNC"; // CNC or Knife

        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
    }
}
