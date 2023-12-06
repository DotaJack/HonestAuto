using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
            var carEvaluations = await _context.CarEvaluations
                //  .Where(ce => _context.Cars.Any(c => c.CarID == ce.CarID))
                .ToListAsync();

            var groupedCarEvaluations = carEvaluations.GroupBy(ce => ce.EvaluationStatus);
            return View(groupedCarEvaluations);
        }

        // GET: CarEvaluation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carEvaluation = await _context.CarEvaluations
            //    .Include(ce => ce.Car)
                .FirstOrDefaultAsync(m => m.CarEvaluationID == id);

            if (carEvaluation == null)
            {
                return NotFound();
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
                // Fetch the existing car evaluation from the database
                var existingEvaluation = await _context.CarEvaluations.FindAsync(id);
                if (existingEvaluation == null)
                {
                    return NotFound();
                }

                // Update only the specific fields
                existingEvaluation.EvaluationStatus = carEvaluation.EvaluationStatus;
                existingEvaluation.EvaluationSummary = carEvaluation.EvaluationSummary;
                existingEvaluation.CarValue = carEvaluation.CarValue;

                try
                {
                    _context.Update(existingEvaluation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarEvaluationExists(id))
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

        private bool CarEvaluationExists(int id)
        {
            return _context.CarEvaluations.Any(ce => ce.CarEvaluationID == id);
        }

        // Additional action methods (Create, Delete, etc.) as needed
    }
}