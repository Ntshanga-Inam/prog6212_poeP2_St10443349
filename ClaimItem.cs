using System;

namespace CMCS.Models
{
    public class ClaimItem
    {
        public int ClaimItemId { get; set; }
        public int ClaimId { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public string Module { get; set; }
        public string Description { get; set; }
    }
}