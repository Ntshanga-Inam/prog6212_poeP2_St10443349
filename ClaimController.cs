using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;
using System.Diagnostics;

namespace CMCS.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(IDataService dataService, IWebHostEnvironment environment, ILogger<ClaimController> logger)
        {
            _dataService = dataService;
            _environment = environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var claims = _dataService.GetClaims()
                    .Where(c => c.LecturerId == 1) // Simulate logged-in lecturer
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToList();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims");
                TempData["Error"] = "Unable to load claims. Please try again.";
                return View(new List<Claim>());
            }
        }

        public IActionResult Create()
        {
            var model = new ClaimSubmissionViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimSubmissionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var claim = new Claim
                {
                    ClaimId = GenerateClaimId(),
                    LecturerId = 1, // Simulate logged-in user
                    ClaimMonth = model.ClaimMonth,
                    TotalHours = model.TotalHours,
                    HourlyRate = model.HourlyRate,
                    Notes = model.Notes,
                    Status = "Submitted",
                    SubmittedDate = DateTime.Now
                };
                claim.CalculateAmount();

                // Handle file uploads
                if (model.Documents != null && model.Documents.Any(d => d.File != null))
                {
                    claim.Documents = new List<Document>();
                    foreach (var doc in model.Documents.Where(d => d.File != null))
                    {
                        var document = await SaveUploadedFile(doc.File, claim.ClaimId);
                        if (document != null)
                        {
                            claim.Documents.Add(document);
                        }
                    }
                }

                _dataService.SaveClaim(claim);
                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim");
                TempData["Error"] = "Error submitting claim. Please try again.";
                return View(model);
            }
        }

        public IActionResult Details(int id)
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
                _logger.LogError(ex, "Error retrieving claim details");
                TempData["Error"] = "Unable to load claim details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(int claimId, IFormFile file, string description)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Please select a file to upload." });
                }

                // Validate file size (5MB limit)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size must be less than 5MB." });
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Only PDF, DOCX, XLSX, JPG, and PNG files are allowed." });
                }

                var document = await SaveUploadedFile(file, claimId, description);
                if (document != null)
                {
                    _dataService.SaveDocument(document);
                    return Json(new { success = true, message = "File uploaded successfully!", fileName = document.FileName });
                }

                return Json(new { success = false, message = "Error uploading file." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return Json(new { success = false, message = "Error uploading file. Please try again." });
            }
        }

        private async Task<Document> SaveUploadedFile(IFormFile file, int claimId, string description = null)
        {
            try
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "documents");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new Document
                {
                    DocumentId = GenerateDocumentId(),
                    ClaimId = claimId,
                    FileName = file.FileName,
                    FilePath = $"/uploads/documents/{fileName}",
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    UploadedDate = DateTime.Now
                };
            }
            catch
            {
                return null;
            }
        }

        private int GenerateClaimId()
        {
            var claims = _dataService.GetClaims();
            return claims.Any() ? claims.Max(c => c.ClaimId) + 1 : 1;
        }

        private int GenerateDocumentId()
        {
            var claims = _dataService.GetClaims();
            var allDocuments = claims.SelectMany(c => c.Documents ?? new List<Document>());
            return allDocuments.Any() ? allDocuments.Max(d => d.DocumentId) + 1 : 1;
        }
    }
}