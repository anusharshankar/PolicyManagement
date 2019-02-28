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
    public class ProcedureController : Controller
    {
        private readonly PolicyWorkflowContext _context;

        public ProcedureController(PolicyWorkflowContext context)
        {
            _context = context;
        }

        // GET: Procedure
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            //ViewData["PolicySortParm"] = String.IsNullOrEmpty(sortOrder) ? "policy_desc" : "Number";

            var procedures = from pr in _context.Procedures.Include(q => q.Policy)  select pr;

            switch (sortOrder)
            {
                case "title_desc": procedures =  procedures.OrderByDescending(p => p.PrTitle);
                    break;
                //case "policy_desc": procedures = procedures.OrderByDescending(p => p.PolicyId);
                //    break;
                default:
                    procedures = procedures.OrderBy(p => p.PrTitle);
                    break;
            }
            
            //var policyWorkflowContext = _context.Procedures.Include(p => p.Policy);
            return View(await procedures.AsNoTracking().ToListAsync());
        }

        // GET: Procedure/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures
                .Include(p => p.Policy)
                .SingleOrDefaultAsync(m => m.ProcedureId == id);
            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        // GET: Procedure/Create
        public IActionResult Create()
        {
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyId");
            return View();
        }

        // POST: Procedure/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcedureId,PrTitle,PrDesc,PrPurpose,PolicyId")] Procedure procedure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(procedure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyId", procedure.PolicyId);
            return View(procedure);
        }

        // GET: Procedure/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures.SingleOrDefaultAsync(m => m.ProcedureId == id);
            if (procedure == null)
            {
                return NotFound();
            }
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyId", procedure.PolicyId);
            return View(procedure);
        }

        // POST: Procedure/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProcedureId,PrTitle,PrDesc,PrPurpose,PolicyId")] Procedure procedure)
        {
            if (id != procedure.ProcedureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(procedure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcedureExists(procedure.ProcedureId))
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
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyId", procedure.PolicyId);
            return View(procedure);
        }

        // GET: Procedure/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedure = await _context.Procedures
                .Include(p => p.Policy)
                .SingleOrDefaultAsync(m => m.ProcedureId == id);
            if (procedure == null)
            {
                return NotFound();
            }

            return View(procedure);
        }

        // POST: Procedure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedure = await _context.Procedures.SingleOrDefaultAsync(m => m.ProcedureId == id);
            _context.Procedures.Remove(procedure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcedureExists(int id)
        {
            return _context.Procedures.Any(e => e.ProcedureId == id);
        }
    }
}
