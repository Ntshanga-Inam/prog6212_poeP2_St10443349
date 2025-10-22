using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required]
        public int LecturerId { get; set; }

        [Required]
        [Display(Name = "Claim Month")]
        public DateTime ClaimMonth { get; set; }

        [Required]
        [Range(0.5, 200, ErrorMessage = "Total hours must be between 0.5 and 200")]
        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Required]
        [Range(100, 1000, ErrorMessage = "Hourly rate must be between 100 and 1000")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Required]
        [Display(Name = "Total Amount")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [Required]
        public string Status { get; set; } = "Draft";

        public DateTime SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public List<ClaimItem> ClaimItems { get; set; } = new List<ClaimItem>();
        public List<Document> Documents { get; set; } = new List<Document>();

        // Calculated property
        public void CalculateAmount()
        {
            Amount = TotalHours * HourlyRate;
        }
    }
}