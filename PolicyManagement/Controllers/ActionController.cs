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
    public class ActionController : Controller
    {
        private readonly PolicyWorkflowContext _context;

        public ActionController(PolicyWorkflowContext context)
        {
            _context = context;
        }

        // GET: Action
        public async Task<IActionResult> Index()
        {
            var policyWorkflowContext = _context.Actions.Include(a => a.Process);
            return View(await policyWorkflowContext.ToListAsync());
        }

        // GET: Action/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actions = await _context.Actions
                .Include(a => a.Process)
                .SingleOrDefaultAsync(m => m.ActionId == id);
            if (actions == null)
            {
                return NotFound();
            }

            return View(actions);
        }

        // GET: Action/Create
        public IActionResult Create()
        {
            ViewData["ProcessId"] = new SelectList(_context.Processes, "ProcessId", "ProcessId");
            return View();
        }

        // POST: Action/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActionId,ATitle,ADesc,Inputs,Outputs,Departments,IsSRSAffected,ProcessId")] Actions actions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProcessId"] = new SelectList(_context.Processes, "ProcessId", "ProcessId", actions.ProcessId);
            return View(actions);
        }

        // GET: Action/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actions = await _context.Actions.SingleOrDefaultAsync(m => m.ActionId == id);
            if (actions == null)
            {
                return NotFound();
            }
            ViewData["ProcessId"] = new SelectList(_context.Processes, "ProcessId", "ProcessId", actions.ProcessId);
            return View(actions);
        }

        // POST: Action/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActionId,ATitle,ADesc,Inputs,Outputs,Departments,IsSRSAffected,ProcessId")] Actions actions)
        {
            if (id != actions.ActionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionsExists(actions.ActionId))
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
            ViewData["ProcessId"] = new SelectList(_context.Processes, "ProcessId", "ProcessId", actions.ProcessId);
            return View(actions);
        }

        // GET: Action/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actions = await _context.Actions
                .Include(a => a.Process)
                .SingleOrDefaultAsync(m => m.ActionId == id);
            if (actions == null)
            {
                return NotFound();
            }

            return View(actions);
        }

        // POST: Action/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actions = await _context.Actions.SingleOrDefaultAsync(m => m.ActionId == id);
            _context.Actions.Remove(actions);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActionsExists(int id)
        {
            return _context.Actions.Any(e => e.ActionId == id);
        }
    }
}
