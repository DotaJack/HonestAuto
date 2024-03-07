using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Retrieve all CarEvaluations from the database
            var carEvaluations = await _context.CarEvaluations.ToListAsync();

            // Group the CarEvaluations by EvaluationStatus
            var groupedCarEvaluations = carEvaluations.GroupBy(ce => ce.EvaluationStatus);

            // Render the view with groupedCarEvaluations
            return View(groupedCarEvaluations);
        }

        // GET: CarEvaluation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a specific CarEvaluation by its CarID or CarEvaluationID from the database
            var carEvaluation = await _context.CarEvaluations.FirstOrDefaultAsync(m => m.CarID == id || m.CarEvaluationID == id);

            if (carEvaluation == null)
            {
                return NotFound();
            }

            // Render the view with the carEvaluation
            return View(carEvaluation);
        }

        [Authorize(Roles = "Admin,Mechanic")]
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

        // GET: CarEvaluation/MyEvaluations
        [Authorize(Roles = "Mechanic")]
        public async Task<IActionResult> MyEvaluations()
        {
            // Get the current mechanic's ID
            var mechanicId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve car evaluations assigned to the current mechanic
            var carEvaluations = await _context.CarEvaluations
                .Where(ce => ce.MechanicID == mechanicId)
                .ToListAsync();

            // Render the view with car evaluations assigned to the current mechanic
            return View(carEvaluations);
        }

        [Authorize(Roles = "Admin,Mechanic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarEvaluationID, CarID, MechanicID, EvaluationStatus, EvaluationSummary, EvaluationDate, CarValue")] CarEvaluation carEvaluation)
        {
            if (id != carEvaluation.CarEvaluationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing car evaluation from the database
                    var existingEvaluation = await _context.CarEvaluations.FindAsync(id);

                    if (existingEvaluation == null)
                    {
                        return NotFound();
                    }

                    // Update all fields of the existing car evaluation
                    existingEvaluation.CarID = carEvaluation.CarID;
                    existingEvaluation.MechanicID = carEvaluation.MechanicID;
                    existingEvaluation.EvaluationStatus = carEvaluation.EvaluationStatus;
                    existingEvaluation.EvaluationSummary = carEvaluation.EvaluationSummary;
                    existingEvaluation.EvaluationDate = carEvaluation.EvaluationDate;
                    existingEvaluation.CarValue = carEvaluation.CarValue;

                    // Save the changes to the database
                    _context.Update(existingEvaluation);
                    await _context.SaveChangesAsync();

                    // Redirect to the Index action after successful update
                    return RedirectToAction(nameof(Index));
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