using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
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
            // Retrieve all CarEvaluations from the database
            var carEvaluations = await _context.CarEvaluations.ToListAsync();

            // Group the CarEvaluations by EvaluationStatus
            var groupedCarEvaluations = carEvaluations.GroupBy(ce => ce.EvaluationStatus);

            // Render the view with groupedCarEvaluations
            return View(groupedCarEvaluations);
        }

        // GET: CarEvaluation/MechanicIndex
        [Authorize(Roles = "Mechanic")]
        public async Task<IActionResult> MechanicIndex()
        {
            // Retrieve the ID of the logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve all CarEvaluations where the mechanic ID matches the logged-in user's ID
            var mechanicEvaluations = await _context.CarEvaluations.Where(ce => ce.MechanicID == userId).ToListAsync();

            // Render the view with mechanicEvaluations
            return View(mechanicEvaluations);
        }

        // GET: CarEvaluation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a specific CarEvaluation by its ID from the database
            var carEvaluation = await _context.CarEvaluations.FirstOrDefaultAsync(m => m.CarEvaluationID == id);

            if (carEvaluation == null)
            {
                return NotFound();
            }

            // Render the view with the carEvaluation
            return View(carEvaluation);
        }

        // GET: CarEvaluation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a specific CarEvaluation for editing by its ID from the database
            var carEvaluation = await _context.CarEvaluations.FindAsync(id);

            if (carEvaluation == null)
            {
                return NotFound();
            }

            // Render the view for editing with the carEvaluation
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

                // Update specific fields of the existing car evaluation
                existingEvaluation.EvaluationStatus = carEvaluation.EvaluationStatus;
                existingEvaluation.EvaluationSummary = carEvaluation.EvaluationSummary;
                existingEvaluation.CarValue = carEvaluation.CarValue;

                try
                {
                    // Save the changes to the database
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

                // Redirect to the Index action after successful update
                return RedirectToAction(nameof(Index));
            }

            // If the ModelState is not valid, return to the edit view with the carEvaluation
            return View(carEvaluation);
        }

        private bool CarEvaluationExists(int id)
        {
            // Check if a CarEvaluation with the given ID exists in the database
            return _context.CarEvaluations.Any(ce => ce.CarEvaluationID == id);
        }

        // Additional action methods (Create, Delete, etc.) as needed
    }
}