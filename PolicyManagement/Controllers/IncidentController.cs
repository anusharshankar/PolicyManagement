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
    public class IncidentController : Controller
    {
        private readonly PolicyWorkflowContext _context;

        public IncidentController(PolicyWorkflowContext context)
        {
            _context = context;
        }

        // GET: Incident
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            ViewData["currentFilter"] = searchString;

            var incidents = from i in _context.Incidents select i;

            if (!String.IsNullOrEmpty(searchString))
            {
                incidents = incidents.Where(i => i.Title.Contains(searchString) || i.Issues.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    incidents = incidents.OrderByDescending(i => i.Title);
                    break;
                case "Date":
                    incidents = incidents.OrderBy(i => i.FiledOn);
                    break;
                case "date_desc":
                    incidents = incidents.OrderByDescending(i => i.FiledOn);
                    break;
                default:
                    incidents = incidents.OrderBy(i => i.Title);
                    break;
            }

            return View(await incidents.AsNoTracking().ToListAsync());
        }

        // GET: Incident/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.Incidents
                .SingleOrDefaultAsync(m => m.IncidentId == id);
            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // GET: Incident/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Incident/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IncidentId,Title,FiledBy,FiledOn,Issues,StepsTaken,ExpectedOutcome,ActualOutcome,RootCause,ChangedExpectation,NecessaryFix,FixStatus,FilerFeedback")] Incident incident)
        {
            if (ModelState.IsValid)
            {
                _context.Add(incident);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(incident);
        }

        // GET: Incident/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.Incidents.SingleOrDefaultAsync(m => m.IncidentId == id);
            if (incident == null)
            {
                return NotFound();
            }
            return View(incident);
        }

        // POST: Incident/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IncidentId,Title,FiledBy,FiledOn,Issues,StepsTaken,ExpectedOutcome,ActualOutcome,RootCause,ChangedExpectation,NecessaryFix,FixStatus,FilerFeedback")] Incident incident)
        {
            if (id != incident.IncidentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incident);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncidentExists(incident.IncidentId))
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
            return View(incident);
        }

        // GET: Incident/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incident = await _context.Incidents
                .SingleOrDefaultAsync(m => m.IncidentId == id);
            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // POST: Incident/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incident = await _context.Incidents.SingleOrDefaultAsync(m => m.IncidentId == id);
            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncidentExists(int id)
        {
            return _context.Incidents.Any(e => e.IncidentId == id);
        }
    }
}
