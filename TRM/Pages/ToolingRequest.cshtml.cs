using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TRM.Data;
using TRM.Models;
using TRM.Models.TRF;

namespace TRM.Pages
{
    public class ToolingRequestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ToolingRequestModel> _logger;

        [BindProperty]
        public ToolingRequestForm TRF { get; set; } = new();

        [BindProperty]
        public List<LineItem> LineItems { get; set; } = new();

        public ToolingRequestModel(ApplicationDbContext context, ILogger<ToolingRequestModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                // Populate dropdown lists
                await PopulateDropdowns();

                if (id.HasValue)
                {
                    // Load existing TRF
                    TRF = await _context.ToolingRequestForms
                        .Include(t => t.LineItems)
                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (TRF == null)
                    {
                        return NotFound("Tooling Request Form not found.");
                    }

                    LineItems = TRF.LineItems.ToList();
                }
                else
                {
                    // New TRF - initialize with empty values
                    TRF = new ToolingRequestForm
                    {
                        FormStatus = "Draft",
                        DateCreated = DateTime.Now
                    };
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the exception and show the page with an error message so the layout/design is visible
                _logger.LogError(ex, "Error loading ToolingRequest page");
                ModelState.AddModelError(string.Empty, "An error occurred while loading the form. Check logs for details.");
                // Ensure dropdowns are at least empty lists to avoid null reference in the view
                ViewData["CustomerList"] = new List<SelectListItem>();
                ViewData["ToolTypeList"] = new List<SelectListItem>();
                ViewData["CategoryList"] = new List<SelectListItem>();
                ViewData["ReasonForRevRepList"] = new List<SelectListItem>();
                ViewData["RegisteredByList"] = new List<SelectListItem>();
                ViewData["NotedByList"] = new List<SelectListItem>();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateDropdowns();
                    return Page();
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(TRF.TRFNo))
                {
                    ModelState.AddModelError("TRF.TRFNo", "TRF No. is required.");
                    await PopulateDropdowns();
                    return Page();
                }

                if (TRF.CustomerId == null || TRF.CustomerId == 0)
                {
                    ModelState.AddModelError("TRF.CustomerId", "Please select a customer.");
                    await PopulateDropdowns();
                    return Page();
                }

                // Set audit fields
                if (TRF.Id == 0)
                {
                    TRF.DateCreated = DateTime.Now;
                    TRF.IsActive = true;
                }
                else
                {
                    TRF.DateModified = DateTime.Now;
                }

                // Handle line items if provided as JSON
                if (!string.IsNullOrEmpty(Request.Form["LineItems"]))
                {
                    try
                    {
                        // Parse line items from request (implementation depends on your AJAX approach)
                        // For now, we'll use the bound LineItems if available
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error parsing line items");
                    }
                }

                // Save TRF
                if (TRF.Id == 0)
                {
                    _context.ToolingRequestForms.Add(TRF);
                }
                else
                {
                    _context.ToolingRequestForms.Update(TRF);
                }

                await _context.SaveChangesAsync();

                // Handle line items
                if (LineItems.Any())
                {
                    foreach (var item in LineItems)
                    {
                        item.TRFId = TRF.Id;
                        item.ToolType = item.ToolType ?? "CNC";

                        if (item.Id == 0)
                        {
                            item.DateCreated = DateTime.Now;
                            _context.LineItems.Add(item);
                        }
                        else
                        {
                            item.DateModified = DateTime.Now;
                            _context.LineItems.Update(item);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                var message = TRF.FormStatus == "Draft" ? "saved as draft" : "submitted";
                TempData["SuccessMessage"] = $"Tooling Request Form {message} successfully!";

                // Redirect based on form status
                if (TRF.FormStatus == "Submitted")
                {
                    return RedirectToPage("/Index");
                }

                return RedirectToPage("./ToolingRequest", new { id = TRF.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving ToolingRequest");
                ModelState.AddModelError("", "An error occurred while saving the form. Please try again.");
                await PopulateDropdowns();
                return Page();
            }
        }

        private async Task PopulateDropdowns()
        {
            // Populate customers
            var customers = await _context.Customers
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.BPCode} - {c.BPName}"
                })
                .OrderBy(s => s.Text)
                .ToListAsync();
            ViewData["CustomerList"] = customers;

            // Populate tool types
            var toolTypes = await _context.ToolTypes
                .Where(t => t.IsActive)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
                .OrderBy(s => s.Text)
                .ToListAsync();
            ViewData["ToolTypeList"] = toolTypes;

            // Populate categories
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .OrderBy(s => s.Text)
                .ToListAsync();
            ViewData["CategoryList"] = categories;

            // Populate reason for revision/replacement
            var reasons = await _context.ReasonsForRevRep
                .Where(r => r.IsActive)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                })
                .OrderBy(s => s.Text)
                .ToListAsync();
            ViewData["ReasonForRevRepList"] = reasons;

            // Populate registered by (for approvals)
            var registeredBy = await _context.RegisteredByList
                .Where(r => r.IsActive)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                })
                .OrderBy(s => s.Text)
                .ToListAsync();
            ViewData["RegisteredByList"] = registeredBy;

            // Populate noted by
            var notedBy = await _context.NotedByList
                .Where(n => n.IsActive)
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name
                })
                .OrderBy(s => s.Text)
                .ToListAsync();
            ViewData["NotedByList"] = notedBy;
        }
    }
}
