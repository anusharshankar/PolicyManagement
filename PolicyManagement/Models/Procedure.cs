using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyManagement.Models
{
    public class Procedure
    {
        [Key]
        public int ProcedureId { get; set; }
        [Display(Name = "Procedure Title")]
        public string PrTitle { get; set; }
        [Display(Name = "Procedure Description")]
        public string PrDesc { get; set; }
        [Display(Name = "Procedure Purpose")]
        public string PrPurpose { get; set; }
        public ICollection<Process> Processes { get; set; }
        public Policy Policy { get; set; }
        public int PolicyId { get; set; }
    }
}
