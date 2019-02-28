using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PolicyManagement.Models;

namespace PolicyManagement.Controllers
{
    public class PolicyController : Controller
    {
        private readonly PolicyWorkflowContext _context;

        public PolicyController(PolicyWorkflowContext context)
        {
            _context = context;
        }

        // GET: Policy
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter) //int? page
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["ApprovalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "app_desc" : "";
            ViewData["ReviewSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            //if(searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}

            ViewData["CurrentFilter"] = searchString;

            var policies = from p in _context.Policies select p;

            if(!String.IsNullOrEmpty(searchString))
            {
                policies = policies.Where(p => p.PTitle.Contains(searchString) || p.PDescription.Contains(searchString));
            }

            switch(sortOrder)
            {
                case "title_desc": policies = policies.OrderByDescending(p => p.PTitle);
                    break;
                case "app_desc": policies = policies.OrderByDescending(p => p.ApprovalAuthority);
                    break;
                case "date_desc": policies = policies.OrderByDescending(p => p.NxtReviewDate);
                    break;
                default:
                    policies = policies.OrderBy(p => p.PTitle);
                    break;

            }
            //return View(await PaginatedList<Policy>.CreateAsync(policies.AsNoTracking(), page ?? 1, pageSize));
            return View(await policies.AsNoTracking().ToListAsync());
        }

        // GET: Policy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policies
                .SingleOrDefaultAsync(m => m.PolicyId == id);
            if (policy == null)
            {
                return NotFound();
            }

            return View(policy);
        }

        // GET: Policy/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Policy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PolicyId,PTitle,PDescription,PScope,ApprovalAuthority,AdvisoryCommittee,Administrator,NxtReviewDate,OrigApprAuth,ApprDate,AmendAuth,AmendDate,Notes")] Policy policy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(policy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(policy);
        }

        // GET: Policy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policies.SingleOrDefaultAsync(m => m.PolicyId == id);
            if (policy == null)
            {
                return NotFound();
            }
            return View(policy);
        }

        // POST: Policy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PolicyId,PTitle,PDescription,PScope,ApprovalAuthority,AdvisoryCommittee,Administrator,NxtReviewDate,OrigApprAuth,ApprDate,AmendAuth,AmendDate,Notes")] Policy policy)
        {
            if (id != policy.PolicyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(policy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PolicyExists(policy.PolicyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(policy);
        }

        // GET: Policy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policies
                .SingleOrDefaultAsync(m => m.PolicyId == id);
            if (policy == null)
            {
                return NotFound();
            }

            return View(policy);
        }

        // POST: Policy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policy = await _context.Policies.SingleOrDefaultAsync(m => m.PolicyId == id);
            _context.Policies.Remove(policy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyExists(int id)
        {
            return _context.Policies.Any(e => e.PolicyId == id);
        }
    }
}
