using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HonestAuto.Models; // Make sure to include the correct namespace for your models
using System;
using System.Linq;
using System.Threading.Tasks;
using HonestAuto.Data;

namespace HonestAuto.Controllers
{
    public class CarEvaluationController : Controller
    {
        private readonly MarketplaceContext _context;

        public CarEvaluationController(MarketplaceContext context)
        {
            _context = context;
        }

        // GET: CarEvaluation
        public async Task<IActionResult> Index()
        {
            var carEvaluation = await _context.CarEvaluations

                .ToListAsync();

            return View(carEvaluation);
        }

        // GET: CarEvaluation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carEvaluation = await _context.CarEvaluations

                .FirstOrDefaultAsync(ce => ce.CarEvaluationID == id);

            if (carEvaluation == null)
            {
                return NotFound();
            }

            return View(carEvaluation);
        }

        // GET: CarEvaluation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarEvaluation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarID,MechanicID,EvaluationStatus,EvaluationSummary,CarValue")] CarEvaluation carEvaluation)
        {
            if (ModelState.IsValid)
            {
                carEvaluation.EvaluationDate = DateTime.UtcNow;
                _context.Add(carEvaluation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carEvaluation);
        }

        // GET: CarEvaluation/Edit/5
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

        // POST: CarEvaluation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarEvaluationID,EvaluationStatus,EvaluationSummary,CarValue")] CarEvaluation carEvaluation)
        {
            if (id != carEvaluation.CarEvaluationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCarEvaluation = await _context.CarEvaluations.FindAsync(id);

                    if (existingCarEvaluation == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existingCarEvaluation with the values from carEvaluation
                    existingCarEvaluation.EvaluationStatus = carEvaluation.EvaluationStatus;
                    existingCarEvaluation.EvaluationSummary = carEvaluation.EvaluationSummary;
                    existingCarEvaluation.CarValue = carEvaluation.CarValue;

                    // You may also need to attach the related Car entity to the context if it's being modified.
                    // Example: _context.Attach(existingCarEvaluation.Car);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarEvaluationExists(carEvaluation.CarEvaluationID))
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

        // POST: CarEvaluation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carEvaluation = await _context.CarEvaluations.FindAsync(id);
            _context.CarEvaluations.Remove(carEvaluation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarEvaluationExists(int id)
        {
            return _context.CarEvaluations.Any(ce => ce.CarEvaluationID == id);
        }
    }
}