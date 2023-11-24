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

        // GET: CarEvaluation/Index
        public async Task<IActionResult> Index()
        {
            // Retrieve a list of all car evaluations with a valid CarID from the database asynchronously
            var carEvaluations = await _context.CarEvaluations
                .Where(ce => _context.Cars.Any(c => c.CarID == ce.CarID))
                .ToListAsync();

            // Group car evaluations by their EvaluationStatus
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

            // Retrieve a car evaluation by ID from the database asynchronously
            var carEvaluation = await _context.CarEvaluations
                .Include(ce => ce.Car) // Include the associated Car
                .FirstOrDefaultAsync(ce => ce.CarEvaluationID == id);

            if (carEvaluation == null)
            {
                return NotFound();
            }

            // Return the car evaluation details along with the associated car to a view
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
                // Set the evaluation date to the current UTC date and time
                carEvaluation.EvaluationDate = DateTime.UtcNow;

                // Add the car evaluation to the database context
                _context.Add(carEvaluation);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return the view with the car evaluation data to display validation errors
            return View(carEvaluation);
        }

        // GET: CarEvaluation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a car evaluation by ID from the database asynchronously
            var carEvaluation = await _context.CarEvaluations.FindAsync(id);

            if (carEvaluation == null)
            {
                return NotFound();
            }

            // Display a form for editing the car evaluation
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
                    // Find the existing car evaluation by ID
                    var existingCarEvaluation = await _context.CarEvaluations.FindAsync(id);

                    if (existingCarEvaluation == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing car evaluation with the values from carEvaluation
                    existingCarEvaluation.EvaluationStatus = carEvaluation.EvaluationStatus;
                    existingCarEvaluation.EvaluationSummary = carEvaluation.EvaluationSummary;
                    existingCarEvaluation.CarValue = carEvaluation.CarValue;

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

                // Redirect to the Index action after successful editing
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return the view with the car evaluation data to display validation errors
            return View(carEvaluation);
        }

        // POST: CarEvaluation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the car evaluation by ID and remove it from the database
            var carEvaluation = await _context.CarEvaluations.FindAsync(id);

            if (carEvaluation != null)
            {
                _context.CarEvaluations.Remove(carEvaluation);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Handle the case when carEvaluation is null if necessary
                // This could be logging the error or returning a user-friendly message
            }

            // Redirect to the Index action after successful deletion
            return RedirectToAction(nameof(Index));
        }

        private bool CarEvaluationExists(int id)
        {
            // Check if a car evaluation with the provided ID exists in the database
            return _context.CarEvaluations.Any(ce => ce.CarEvaluationID == id);
        }
    }
}