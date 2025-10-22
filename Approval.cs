using System;

namespace CMCS.Models
{
    public class Approval
    {
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public int ApproverId { get; set; }
        public string ApprovedByRole { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } // "Approved", "Rejected"
    }
}