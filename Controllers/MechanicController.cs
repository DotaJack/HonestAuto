using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonestAuto.Controllers
{
    public class MechanicController : Controller
    {
        private readonly MarketplaceContext _context;

        public MechanicController(MarketplaceContext context)
        {
            _context = context;
        }

        // INDEX (Read/List all)
        public IActionResult Index()
        {
            var mechanics = _context.Mechanics.ToList();
            return View(mechanics);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mechanic mechanic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mechanic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mechanic);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }
            return View(mechanic);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MechanicID,FirstName,LastName,Email,Password,Address")] Mechanic mechanic)
        {
            if (id != mechanic.MechanicID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mechanic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Mechanics.Any(m => m.MechanicID == mechanic.MechanicID))
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
            return View(mechanic);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }
            return View(mechanic);
        }

        // DELETE (POST/Confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }
            _context.Mechanics.Remove(mechanic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}