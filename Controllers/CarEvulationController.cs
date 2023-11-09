using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonestAuto.Controllers
{
    public class CarEvaluationController : Controller
    {
        private readonly MarketplaceContext _context;

        public CarEvaluationController(MarketplaceContext context)
        {
            _context = context;
        }

        // INDEX (Read/List all)
        public IActionResult Index()
        {
            var carEvaluations = _context.CarEvaluations.ToList();
            return View(carEvaluations);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarEvaluation carEvaluation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carEvaluation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carEvaluation);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carEvaluation = await _context.CarEvaluations.FindAsync(id);
            if (carEvaluation == null)
            {
                return NotFound();
            }
            return View(carEvaluation);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarEvaluationID,CarID,MechanicID,CarValue,Image,EvaluationStatus,EvaluationSummary")] CarEvaluation carEvaluation)
        {
            if (id != carEvaluation.CarEvaluationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carEvaluation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CarEvaluations.Any(ce => ce.CarEvaluationID == carEvaluation.CarEvaluationID))
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
            return View(carEvaluation);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carEvaluation = await _context.CarEvaluations.FindAsync(id);
            if (carEvaluation == null)
            {
                return NotFound();
            }
            return View(carEvaluation);
        }

        // DELETE (POST/Confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carEvaluation = await _context.CarEvaluations.FindAsync(id);
            if (carEvaluation == null)
            {
                return NotFound();
            }
            _context.CarEvaluations.Remove(carEvaluation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}