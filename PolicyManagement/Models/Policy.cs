using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PolicyManagement.Models
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }
        [Display(Name = "Policy Title")]
        public string PTitle { get; set; }
        [Display(Name = "Policy Description")]
        public string PDescription { get; set; }
        [Display(Name = "Scope and Responsibilities")]
        public string PScope { get; set; }
        public ICollection<Procedure> Procedures { get; set; }
        [Display(Name = "Approval Authority")]
        public string ApprovalAuthority { get; set; }
        [Display(Name = "Advisory Committee")]
        public string AdvisoryCommittee { get; set; }
        public string Administrator { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Next Review Date")]
        public DateTime NxtReviewDate { get; set; }
        [Display(Name = "Original Approval Authority")]
        public string OrigApprAuth { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Approval Date")]
        public DateTime ApprDate { get; set; }
        [Display(Name = "Amendment Authority")]
        public string AmendAuth { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Amendment Date")]
        public DateTime AmendDate { get; set; }
        public string Notes { get; set; }
    }
}
