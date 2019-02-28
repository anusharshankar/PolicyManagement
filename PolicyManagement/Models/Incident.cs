using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyManagement.Models
{
    public class Incident
    {
        [Key]
        public int IncidentId { get; set; }
        [Display(Name = "Incident Title")]
        public string Title { get; set; }
        [Display(Name = "Filed By")]
        public string FiledBy { get; set; }
        [Display(Name = "Filed On")]
        [DataType(DataType.DateTime)]
        public DateTime FiledOn { get; set; }
        public string Issues { get; set; }
        [Display(Name = "Steps Taken")]
        public string StepsTaken { get; set; }
        [Display(Name = "Expected Outcome")]
        public string ExpectedOutcome { get; set; }
        [Display(Name = "Actual Outcome")]
        public string ActualOutcome { get; set; }
        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }
        [Display(Name = "Changed Expectation")]
        public string ChangedExpectation { get; set; }
        [Display(Name = "Necessary Fix")]
        public string NecessaryFix { get; set; }
        [Display(Name = "Fix Status")]
        public string FixStatus { get; set; }
        [Display(Name = "Filer Feedback")]
        public string FilerFeedback { get; set; }
    }
}
