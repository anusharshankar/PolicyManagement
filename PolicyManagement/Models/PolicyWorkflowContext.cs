using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolicyManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PolicyLDAP.Identity;

namespace PolicyManagement.Models
{
    public class PolicyWorkflowContext  :  DbContext
    {

        public PolicyWorkflowContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Policy> Policies { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<Actions> Actions { get; set; }
        public DbSet<Incident> Incidents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Policy>().ToTable("Policy");
            modelBuilder.Entity<Procedure>().ToTable("Procedure");
            modelBuilder.Entity<Process>().ToTable("Process");
            modelBuilder.Entity<Actions>().ToTable("Actions");
            modelBuilder.Entity<Incident>().ToTable("Incident");
        }

        
    }
}
