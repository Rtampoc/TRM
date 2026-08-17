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

        public string? SelectedCategory { get; set; }
        public string SelectedCategoryTitle { get; set; } = string.Empty;

        public PrerequisitesModel(ApplicationDbContext context, ILogger<PrerequisitesModel> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
