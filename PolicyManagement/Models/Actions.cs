using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyManagement.Models
{
    public class Actions
    {
        [Key]
        public int ActionId { get; set; }
        [Display(Name = "Action Title")]
        public string ATitle { get; set; }
        [Display(Name = "Action Decription")]
        public string ADesc { get; set; }
        public string Inputs { get; set; }
        public string Outputs { get; set; }
        public string Departments { get; set; }
        public bool IsSRSAffected { get; set; }
        public Process Process { get; set; }
        public int ProcessId { get; set; }
    }
}
