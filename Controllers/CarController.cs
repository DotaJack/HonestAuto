﻿using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HonestAuto.Controllers
{
    public class CarController : Controller
    {
        private readonly MarketplaceContext _context;

        public CarController(MarketplaceContext context)
        {
            _context = context;
        }

        // INDEX (Read/List all)
        public async Task<IActionResult> Index()
        {
            // Retrieve a list of all cars from the database asynchronously
            var cars = await _context.Cars.ToListAsync();

            // Return the list of cars to a view
            return View(cars);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Display a form for creating a new car
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Brand,Model,Year,Mileage,History,UserID")] Car car, IFormFile carImageFile)
        {
            // Check if the submitted form data is valid
            if (ModelState.IsValid)
            {
                // Check if a car image file is uploaded
                if (carImageFile != null && carImageFile.Length > 0)
                {
                    // Copy the uploaded car image to a memory stream
                    using (var memoryStream = new MemoryStream())
                    {
                        await carImageFile.CopyToAsync(memoryStream);
                        car.CarImage = memoryStream.ToArray(); // Store the image data as a byte array in the Car object
                    }
                }

                // Add the car to the database context
                _context.Add(car);

                // Save changes to the database to get the CarID generated by the database
                await _context.SaveChangesAsync();

                // Create a new CarEvaluation entity using the newly generated CarID
                var carEvaluation = new CarEvaluation
                {
                    CarID = car.CarID, // The CarID from the newly created Car
                    EvaluationStatus = "Submitted", // Set the EvaluationStatus as "Submitted"
                    EvaluationDate = DateTime.UtcNow, // Set the current UTC date and time
                    MechanicID = 1, // Set the MechanicID to 1 for now
                    CarValue = 20000, // Set the CarValue to 20000 for now
                    EvaluationSummary = "This is a summary of the car evaluation."

                    // Other properties can be initialized here if needed
                };

                // Add the new CarEvaluation to the context
                _context.CarEvaluations.Add(carEvaluation);

                // Save changes to the database to save the CarEvaluation
                await _context.SaveChangesAsync();

                // Redirect to the Index action after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return the view with the car data to display validation errors
            return View(car);
        }

        // DETAILS (Read/View single item)
        public async Task<IActionResult> Details(int? id)
        {
            // Check if the provided ID is null
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a car by ID from the database asynchronously
            var car = await _context.Cars.FirstOrDefaultAsync(m => m.CarID == id);

            // Check if the car with the provided ID exists
            if (car == null)
            {
                return NotFound();
            }

            // Return the car details to a view
            return View(car);
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if the provided ID is null
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a car by ID from the database asynchronously
            var car = await _context.Cars.FindAsync(id);

            // Check if the car with the provided ID exists
            if (car == null)
            {
                return NotFound();
            }

            // Display a form for editing the car
            return View(car);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarID,Brand,Model,Year,Mileage,History,UserID")] Car car)
        {
            // Check if the provided ID matches the car's ID
            if (id != car.CarID)
            {
                return NotFound();
            }

            // Check if the submitted form data is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Update the car in the database context
                    _context.Update(car);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if the car with the provided ID no longer exists
                    if (!CarExists(car.CarID))
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

            // If the model state is not valid, return the view with the car data to display validation errors
            return View(car);
        }

        // GET: Car/EditImage/5
        public async Task<IActionResult> EditImage(int? id)
        {
            if (id == null)
            {
                return NotFound(); // If no ID is provided, return a 404 Not Found result.
            }

            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound(); // If no car with the provided ID is found, return a 404 Not Found result.
            }

            return View(car); // Display the view for updating the image.
        }

        // POST: Car/EditImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(int id, IFormFile imageFile)
        {
            if (id == null)
            {
                return NotFound(); // If no ID is provided, return a 404 Not Found result.
            }

            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound(); // If no car with the provided ID is found, return a 404 Not Found result.
            }

            if (imageFile != null)
            {
                // If a new image file is provided, read it into a memory stream and assign it to the CarImage property.
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    car.CarImage = memoryStream.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car); // Update the Car entity in the database context.
                    await _context.SaveChangesAsync(); // Save changes to the database.
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.CarID))
                    {
                        return NotFound(); // If the car no longer exists, return a 404 Not Found result.
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index)); // Redirect to the Index action after a successful update.
            }

            return View(car); // If the model state is not valid, return the view with the car data to display validation errors.
        }

        // DELETE (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            // Check if the provided ID is null
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve a car by ID from the database asynchronously
            var car = await _context.Cars.FirstOrDefaultAsync(m => m.CarID == id);

            // Check if the car with the provided ID exists
            if (car == null)
            {
                return NotFound();
            }

            // Display a confirmation page for deleting the car
            return View(car);
        }

        // DELETE (POST/Confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve a car by ID from the database
            var car = await _context.Cars.FindAsync(id);

            // Check if the car with the provided ID exists
            if (car != null)
            {
                // Remove the car from the database context
                _context.Cars.Remove(car);

                // Save changes to the database to delete the car
                await _context.SaveChangesAsync();
            }

            // Redirect to the Index action after successful deletion
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            // Check if a car with the provided ID exists in the database
            return _context.Cars.Any(e => e.CarID == id);
        }
    }
}