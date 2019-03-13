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
    public class ProcessController : Controller
    {
        private readonly PolicyWorkflowContext _context;

        public ProcessController(PolicyWorkflowContext context)
        {
            _context = context;
        }

        // GET: Process
        public async Task<IActionResult> Index()
        {
            var policyWorkflowContext = _context.Processes.Include(p => p.Procedure);
            return View(await policyWorkflowContext.ToListAsync());
        }

        // GET: Process/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _context.Processes
                .Include(p => p.Procedure)
                .SingleOrDefaultAsync(m => m.ProcessId == id);
            if (process == null)
            {
                return NotFound();
            }

            return View(process);
        }

        // GET: Process/Create
        public IActionResult Create()
        {
            //ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId");
            PopulateProcedureDropdownList();
            return View();
        }

        // POST: Process/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcessId,ProcTitle,ProcDesc,ProcedureId")] Process process)
        {
            if (ModelState.IsValid)
            {
                _context.Add(process);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateProcedureDropdownList(process.ProcedureId);
           // ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId", process.ProcedureId);
            return View(process);
        }

        // GET: Process/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _context.Processes.SingleOrDefaultAsync(m => m.ProcessId == id);
            if (process == null)
            {
                return NotFound();
            }
            //ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId", process.ProcedureId);
            PopulateProcedureDropdownList(process.ProcedureId);
            return View(process);
        }

        // POST: Process/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProcessId,ProcTitle,ProcDesc,ProcedureId")] Process process)
        {
            if (id != process.ProcessId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(process);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessExists(process.ProcessId))
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
            //ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId", process.ProcedureId);
            PopulateProcedureDropdownList(process.ProcedureId);
            return View(process);
        }

        // GET: Process/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _context.Processes
                .Include(p => p.Procedure)
                .SingleOrDefaultAsync(m => m.ProcessId == id);
            if (process == null)
            {
                return NotFound();
            }

            return View(process);
        }

        // POST: Process/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var process = await _context.Processes.SingleOrDefaultAsync(m => m.ProcessId == id);
            _context.Processes.Remove(process);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcessExists(int id)
        {
            return _context.Processes.Any(e => e.ProcessId == id);
        }

        private void PopulateProcedureDropdownList(object selectedProcedure = null)
        {
            var proceduresQuery = from proc in _context.Procedures orderby proc.PrTitle select proc;

            ViewBag.ProcedureId = new SelectList(proceduresQuery.AsNoTracking(), "ProcedureId", "PrTitle", selectedProcedure);
        }
    }
}
