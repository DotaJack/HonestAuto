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
                .Include(ce => ce.Car)
                .Where(ce => _context.Cars.Any(c => c.CarID == ce.CarID))
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
                .Include(ce => ce.Car)
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

        // POST: CarEvaluation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarEvaluationID,EvaluationStatus,EvaluationSummary,CarValue")] CarEvaluation carEvaluation)
        {
            // Check if the provided ID matches the car evaluation's ID
            if (id != carEvaluation.CarEvaluationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the car in the database context
                    _context.Update(carEvaluation);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if the car with the provided ID no longer exists
                    if (!CarEvaluationExists(carEvaluation.CarEvaluationID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the Index action after successful editing
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return the view with the car evaluation data to display validation errors
            return View(carEvaluation);
        }

        private bool CarEvaluationExists(int id)
        {
            return _context.CarEvaluations.Any(ce => ce.CarEvaluationID == id);
        }

        // Additional action methods (Create, Delete, etc.) as needed
    }
}