using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TRM.Data;
using TRM.Models;

namespace TRM.Pages
{
    public class PrerequisitesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrerequisitesModel> _logger;

        public List<Category> Categories { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();
        public List<ToolType> ToolTypes { get; set; } = new();
        public List<ReasonForRevRep> ReasonsForRevRep { get; set; } = new();
        public List<NotedBy> NotedByList { get; set; } = new();
        public List<RegisteredBy> RegisteredByList { get; set; } = new();

        // Summary counts used by the category cards — keep separate from loaded lists so counts are always accurate
        public int CustomersCount { get; set; }
        public int ToolTypesCount { get; set; }
        public int CategoriesCount { get; set; }
        public int ReasonsForRevRepCount { get; set; }
        public int NotedByCount { get; set; }
        public int RegisteredByCount { get; set; }

        public string? SelectedCategory { get; set; }
        public string SelectedCategoryTitle { get; set; } = string.Empty;

        public PrerequisitesModel(ApplicationDbContext context, ILogger<PrerequisitesModel> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Handler to add a new record to a selected prerequisite category
        public async Task<IActionResult> OnPostAddAsync(string category, string? bpCode, string? bpName, string? name, string? status)
        {
            if (string.IsNullOrEmpty(category))
            {
                TempData["ErrorMessage"] = "Category is required.";
                return RedirectToPage();
            }

            // Status select posts "Active" / "Inactive"; default to Active when not supplied.
            bool isActive = !string.Equals(status, "Inactive", StringComparison.OrdinalIgnoreCase);

            try
            {
                switch (category.ToLower())
                {
                    case "customers":
                        if (string.IsNullOrWhiteSpace(bpCode) || string.IsNullOrWhiteSpace(bpName))
                        {
                            TempData["ErrorMessage"] = "Customer code and name are required.";
                            return RedirectToPage(new { category = "customers" });
                        }

                        _context.Customers.Add(new Customer
                        {
                            BPCode = bpCode!.Trim(),
                            BPName = bpName!.Trim(),
                            IsActive = isActive,
                            DateCreated = DateTime.Now
                        });
                        break;

                    case "tool-types":
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            TempData["ErrorMessage"] = "Tool type name is required.";
                            return RedirectToPage(new { category = "tool-types" });
                        }

                        _context.ToolTypes.Add(new ToolType
                        {
                            Name = name!.Trim(),
                            IsActive = isActive,
                            DateCreated = DateTime.Now
                        });
                        break;

                    case "category":
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            TempData["ErrorMessage"] = "Category name is required.";
                            return RedirectToPage(new { category = "category" });
                        }

                        _context.Categories.Add(new Category
                        {
                            Name = name!.Trim(),
                            IsActive = isActive,
                            DateCreated = DateTime.Now
                        });
                        break;

                    case "reasons-for-revision":
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            TempData["ErrorMessage"] = "Reason name is required.";
                            return RedirectToPage(new { category = "reasons-for-revision" });
                        }

                        _context.ReasonsForRevRep.Add(new ReasonForRevRep
                        {
                            Name = name!.Trim(),
                            IsActive = isActive,
                            DateCreated = DateTime.Now
                        });
                        break;

                    case "noted-by":
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            TempData["ErrorMessage"] = "Name is required.";
                            return RedirectToPage(new { category = "noted-by" });
                        }

                        _context.NotedByList.Add(new NotedBy
                        {
                            Name = name!.Trim(),
                            IsActive = isActive,
                            DateCreated = DateTime.Now
                        });
                        break;

                    case "registered-by":
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            TempData["ErrorMessage"] = "Name is required.";
                            return RedirectToPage(new { category = "registered-by" });
                        }

                        _context.RegisteredByList.Add(new RegisteredBy
                        {
                            Name = name!.Trim(),
                            IsActive = isActive,
                            DateCreated = DateTime.Now
                        });
                        break;

                    default:
                        TempData["ErrorMessage"] = "Unknown category.";
                        return RedirectToPage();
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Record added successfully.";
                return RedirectToPage(new { category = category });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding prerequisite record");
                TempData["ErrorMessage"] = "An error occurred while adding the record.";
                return RedirectToPage(new { category = category });
            }
        }

        public async Task OnGetAsync(string? category)
        {
            // Check if user is authenticated
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                RedirectToPage("/Auth/Login");
                return;
            }

            try
            {
                // Ensure default categories exist
                await EnsureDefaultCategoriesAsync();

                // Load all categories
                Categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                // Populate summary counts for the cards so they always show accurate numbers
                CustomersCount = await _context.Customers.Where(c => c.IsActive).CountAsync();
                ToolTypesCount = await _context.ToolTypes.Where(t => t.IsActive).CountAsync();
                CategoriesCount = await _context.Categories.Where(c => c.IsActive).CountAsync();
                ReasonsForRevRepCount = await _context.ReasonsForRevRep.Where(r => r.IsActive).CountAsync();
                NotedByCount = await _context.NotedByList.Where(n => n.IsActive).CountAsync();
                RegisteredByCount = await _context.RegisteredByList.Where(r => r.IsActive).CountAsync();

                // Set selected category based on URL parameter
                SelectedCategory = category?.ToLower() ?? "customers";

                // Load data for selected category
                await LoadCategoryDataAsync(SelectedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading prerequisites data");
            }
        }

        // Return a single record as JSON for editing modal
        public async Task<IActionResult> OnGetRecordAsync(int id, string category)
        {
            try
            {
                switch (category?.ToLower())
                {
                    case "customers":
                        var c = await _context.Customers.FindAsync(id);
                        if (c == null) return new JsonResult(null);
                        return new JsonResult(new { id = c.Id, bpCode = c.BPCode, bpName = c.BPName, isActive = c.IsActive });
                    case "tool-types":
                        var t = await _context.ToolTypes.FindAsync(id);
                        if (t == null) return new JsonResult(null);
                        return new JsonResult(new { id = t.Id, name = t.Name, isActive = t.IsActive });
                    case "category":
                        var cat = await _context.Categories.FindAsync(id);
                        if (cat == null) return new JsonResult(null);
                        return new JsonResult(new { id = cat.Id, name = cat.Name, isActive = cat.IsActive });
                    case "reasons-for-revision":
                        var r = await _context.ReasonsForRevRep.FindAsync(id);
                        if (r == null) return new JsonResult(null);
                        return new JsonResult(new { id = r.Id, name = r.Name, isActive = r.IsActive });
                    case "noted-by":
                        var n = await _context.NotedByList.FindAsync(id);
                        if (n == null) return new JsonResult(null);
                        return new JsonResult(new { id = n.Id, name = n.Name, isActive = n.IsActive });
                    case "registered-by":
                        var rb = await _context.RegisteredByList.FindAsync(id);
                        if (rb == null) return new JsonResult(null);
                        return new JsonResult(new { id = rb.Id, name = rb.Name, isActive = rb.IsActive });
                    default:
                        return new JsonResult(null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching record for edit");
                return StatusCode(500);
            }
        }

        // Edit a record
        public async Task<IActionResult> OnPostEditAsync(int id, string category, string? bpCode, string? bpName, string? name, string? status)
        {
            if (string.IsNullOrEmpty(category))
            {
                TempData["ErrorMessage"] = "Category is required.";
                return RedirectToPage();
            }

            // Status select posts "Active" / "Inactive"; only apply it if it was actually submitted.
            bool? isActive = string.IsNullOrEmpty(status)
                ? null
                : string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase);

            try
            {
                switch (category.ToLower())
                {
                    case "customers":
                        var c = await _context.Customers.FindAsync(id);
                        if (c == null)
                        {
                            TempData["ErrorMessage"] = "Customer not found.";
                            return RedirectToPage(new { category = "customers" });
                        }
                        if (!string.IsNullOrWhiteSpace(bpCode)) c.BPCode = bpCode.Trim();
                        if (!string.IsNullOrWhiteSpace(bpName)) c.BPName = bpName.Trim();
                        if (isActive.HasValue) c.IsActive = isActive.Value;
                        break;
                    case "tool-types":
                        var t = await _context.ToolTypes.FindAsync(id);
                        if (t == null)
                        {
                            TempData["ErrorMessage"] = "Tool type not found.";
                            return RedirectToPage(new { category = "tool-types" });
                        }
                        if (!string.IsNullOrWhiteSpace(name)) t.Name = name.Trim();
                        if (isActive.HasValue) t.IsActive = isActive.Value;
                        break;
                    case "category":
                        var cat = await _context.Categories.FindAsync(id);
                        if (cat == null)
                        {
                            TempData["ErrorMessage"] = "Category not found.";
                            return RedirectToPage(new { category = "category" });
                        }
                        if (!string.IsNullOrWhiteSpace(name)) cat.Name = name.Trim();
                        if (isActive.HasValue) cat.IsActive = isActive.Value;
                        break;
                    case "reasons-for-revision":
                        var r = await _context.ReasonsForRevRep.FindAsync(id);
                        if (r == null)
                        {
                            TempData["ErrorMessage"] = "Reason not found.";
                            return RedirectToPage(new { category = "reasons-for-revision" });
                        }
                        if (!string.IsNullOrWhiteSpace(name)) r.Name = name.Trim();
                        if (isActive.HasValue) r.IsActive = isActive.Value;
                        break;
                    case "noted-by":
                        var n = await _context.NotedByList.FindAsync(id);
                        if (n == null)
                        {
                            TempData["ErrorMessage"] = "Record not found.";
                            return RedirectToPage(new { category = "noted-by" });
                        }
                        if (!string.IsNullOrWhiteSpace(name)) n.Name = name.Trim();
                        if (isActive.HasValue) n.IsActive = isActive.Value;
                        break;
                    case "registered-by":
                        var rb = await _context.RegisteredByList.FindAsync(id);
                        if (rb == null)
                        {
                            TempData["ErrorMessage"] = "Record not found.";
                            return RedirectToPage(new { category = "registered-by" });
                        }
                        if (!string.IsNullOrWhiteSpace(name)) rb.Name = name.Trim();
                        if (isActive.HasValue) rb.IsActive = isActive.Value;
                        break;
                    default:
                        TempData["ErrorMessage"] = "Unknown category.";
                        return RedirectToPage();
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Record updated successfully.";
                return RedirectToPage(new { category = category });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing prerequisite record");
                TempData["ErrorMessage"] = "An error occurred while updating the record.";
                return RedirectToPage(new { category = category });
            }
        }

        // Soft-delete a record (mark IsActive = false)
        public async Task<IActionResult> OnPostDeleteAsync(int id, string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                TempData["ErrorMessage"] = "Category is required.";
                return RedirectToPage();
            }

            try
            {
                switch (category.ToLower())
                {
                    case "customers":
                        var c = await _context.Customers.FindAsync(id);
                        if (c != null) c.IsActive = false;
                        break;
                    case "tool-types":
                        var t = await _context.ToolTypes.FindAsync(id);
                        if (t != null) t.IsActive = false;
                        break;
                    case "category":
                        var cat = await _context.Categories.FindAsync(id);
                        if (cat != null) cat.IsActive = false;
                        break;
                    case "reasons-for-revision":
                        var r = await _context.ReasonsForRevRep.FindAsync(id);
                        if (r != null) r.IsActive = false;
                        break;
                    case "noted-by":
                        var n = await _context.NotedByList.FindAsync(id);
                        if (n != null) n.IsActive = false;
                        break;
                    case "registered-by":
                        var rb = await _context.RegisteredByList.FindAsync(id);
                        if (rb != null) rb.IsActive = false;
                        break;
                    default:
                        TempData["ErrorMessage"] = "Unknown category.";
                        return RedirectToPage();
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Record deleted successfully.";
                return RedirectToPage(new { category = category });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prerequisite record");
                TempData["ErrorMessage"] = "An error occurred while deleting the record.";
                return RedirectToPage(new { category = category });
            }
        }

        private async Task EnsureDefaultCategoriesAsync()
        {
            var defaultCategories = new[] { "Customers", "Tool Types", "Reasons for Revision", "Noted By", "Registered By" };

            foreach (var categoryName in defaultCategories)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Name == categoryName);

                if (!categoryExists)
                {
                    _context.Categories.Add(new Category
                    {
                        Name = categoryName,
                        IsActive = true,
                        DateCreated = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task LoadCategoryDataAsync(string categoryKey)
        {
            try
            {
                switch (categoryKey)
                {
                    case "customers":
                        Customers = await _context.Customers
                            .Where(c => c.IsActive)
                            .OrderBy(c => c.BPName)
                            .ToListAsync();
                        SelectedCategoryTitle = "Customers";
                        break;

                    case "tool-types":
                        ToolTypes = await _context.ToolTypes
                            .Where(t => t.IsActive)
                            .OrderBy(t => t.Name)
                            .ToListAsync();
                        SelectedCategoryTitle = "Tool Types";
                        break;

                    case "category":
                        Categories = await _context.Categories
                            .Where(c => c.IsActive)
                            .OrderBy(c => c.Name)
                            .ToListAsync();
                        SelectedCategoryTitle = "Category";
                        break;

                    case "reasons-for-revision":
                        ReasonsForRevRep = await _context.ReasonsForRevRep
                            .Where(r => r.IsActive)
                            .OrderBy(r => r.Name)
                            .ToListAsync();
                        SelectedCategoryTitle = "Reasons for Revision";
                        break;

                    case "noted-by":
                        NotedByList = await _context.NotedByList
                            .Where(n => n.IsActive)
                            .OrderBy(n => n.Name)
                            .ToListAsync();
                        SelectedCategoryTitle = "Noted By";
                        break;

                    case "registered-by":
                        RegisteredByList = await _context.RegisteredByList
                            .Where(r => r.IsActive)
                            .OrderBy(r => r.Name)
                            .ToListAsync();
                        SelectedCategoryTitle = "Registered By";
                        break;

                    default:
                        Customers = await _context.Customers
                            .Where(c => c.IsActive)
                            .OrderBy(c => c.BPName)
                            .ToListAsync();
                        SelectedCategoryTitle = "Customers";
                        SelectedCategory = "customers";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading data for category: {categoryKey}");
            }
        }
    }
}
