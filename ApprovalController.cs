using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;
using System.Diagnostics;

namespace CMCS.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IDataService _dataService;
        private readonly ILogger<ApprovalController> _logger;

        public ApprovalController(IDataService dataService, ILogger<ApprovalController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var userRole = "Coordinator"; // Simulate logged-in user role
                var claims = _dataService.GetClaims();

                var pendingClaims = userRole switch
                {
                    "Coordinator" => claims.Where(c => c.Status == "Submitted" || c.Status == "With Coordinator").ToList(),
                    "Manager" => claims.Where(c => c.Status == "With Manager").ToList(),
                    _ => new List<Claim>()
                };

                return View(pendingClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approval queue");
                TempData["Error"] = "Unable to load approval queue.";
                return View(new List<Claim>());
            }
        }

        public IActionResult Review(int id)
        {
            try
            {
                var claim = _dataService.GetClaim(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claim for review");
                TempData["Error"] = "Unable to load claim for review.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string notes)
        {
            try
            {
                var claim = _dataService.GetClaim(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                var userRole = "Coordinator"; // Simulate logged-in user role

                if (userRole == "Coordinator" && (claim.Status == "Submitted" || claim.Status == "With Coordinator"))
                {
                    claim.Status = "With Manager";
                    claim.ApprovedBy = "Coordinator";
                }
                else if (userRole == "Manager" && claim.Status == "With Manager")
                {
                    claim.Status = "Approved";
                    claim.ApprovedBy = "Manager";
                    claim.ApprovedDate = DateTime.Now;
                }

                _dataService.SaveClaim(claim);
                TempData["Success"] = $"Claim #{id} has been approved and moved to next stage.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving claim");
                TempData["Error"] = "Error approving claim. Please try again.";
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string notes)
        {
            try
            {
                var claim = _dataService.GetClaim(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                claim.Status = "Rejected";
                claim.ApprovedBy = HttpContext.User.Identity?.Name ?? "System";
                claim.Notes = $"[REJECTED] {notes}";

                _dataService.SaveClaim(claim);
                TempData["Success"] = $"Claim #{id} has been rejected.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim");
                TempData["Error"] = "Error rejecting claim. Please try again.";
                return RedirectToAction(nameof(Review), new { id });
            }
        }
    }
}