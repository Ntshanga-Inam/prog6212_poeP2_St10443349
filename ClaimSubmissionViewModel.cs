using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class ClaimSubmissionViewModel
    {
        [Required]
        [Display(Name = "Claim Month")]
        public DateTime ClaimMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [Required]
        [Range(0.5, 200, ErrorMessage = "Total hours must be between 0.5 and 200")]
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Required]
        [Range(100, 1000, ErrorMessage = "Hourly rate must be between 100 and 1000")]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; } = 250; // Default rate

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [Display(Name = "Supporting Documents")]
        public List<DocumentUpload> Documents { get; set; } = new List<DocumentUpload>();

        // Calculated property for display
        public decimal TotalAmount => TotalHours * HourlyRate;
    }

    public class DocumentUpload
    {
        [Display(Name = "Select File")]
        public IFormFile File { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}