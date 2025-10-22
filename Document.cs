using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int ClaimId { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        public string FilePath { get; set; }

        [Display(Name = "File Size")]
        public long FileSize { get; set; }

        [Display(Name = "Uploaded Date")]
        public DateTime UploadedDate { get; set; }

        public string ContentType { get; set; }
    }
}