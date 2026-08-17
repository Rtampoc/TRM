namespace TRM.Models
{
    /// <summary>
    /// Tool Type model representing records in TRMS_ToolType table
    /// </summary>
    public class ToolType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
    }

    /// <summary>
    /// Registered By model representing records in TRMS_RegisteredBy table
    /// </summary>
    public class RegisteredBy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
    }

    /// <summary>
    /// Reason For Revision/Replacement model
    /// </summary>
    public class ReasonForRevRep
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
    }

    /// <summary>
    /// Noted By model representing records in TRMS_NotedBy table
    /// </summary>
    public class NotedBy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
    }

    /// <summary>
    /// Customer model representing records in TRMS_Customer table
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string BPCode { get; set; } = string.Empty;
        public string BPName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
    }

    /// <summary>
    /// Category model representing records in TRMS_Category table
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
    }
}
