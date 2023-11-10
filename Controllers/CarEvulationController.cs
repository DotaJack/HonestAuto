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
            // Constructor for CarEvaluationController that takes an instance of MarketplaceContext.
        }

        // INDEX (Read/List all)
        public IActionResult Index()
        {
            var carEvaluations = _context.CarEvaluations.ToList();
            // Retrieve all car evaluations from the database and convert them to a list.
            return View(carEvaluations);
            // Return a view with the list of car evaluations.
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
            // Display the Create view for creating a new car evaluation.
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
                // If the model is valid, add the car evaluation to the database and save changes.
                return RedirectToAction(nameof(Index));
                // Redirect to the Index action after successfully creating the car evaluation.
            }
            return View(carEvaluation);
            // If the model is not valid, return the Create view with validation errors.
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
            // Display the Edit view for editing an existing car evaluation.
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
                    // If the model is valid, update the car evaluation in the database and save changes.
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
                // Redirect to the Index action after successfully editing the car evaluation.
            }
            return View(carEvaluation);
            // If the model is not valid, return the Edit view with validation errors.
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
            // Display the Delete view for deleting an existing car evaluation.
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
            // Remove the car evaluation from the database and save changes.
            return RedirectToAction(nameof(Index));
            // Redirect to the Index action after successfully deleting the car evaluation.
        }
    }
}