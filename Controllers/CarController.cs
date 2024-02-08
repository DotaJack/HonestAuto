using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HonestAuto.Controllers
{
    public class CarController : Controller
    {
        private readonly MarketplaceContext _context;

        // Inject UserManager into your controller
        private readonly UserManager<User> _userManager;

        public CarController(MarketplaceContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? searchYear)
        {
            // Retrieve a list of all cars from the database asynchronously
            var cars = _context.Cars.AsQueryable(); // Start with all cars

            if (searchYear.HasValue)
            {
                // If a search year is provided, filter cars by that year
                cars = cars.Where(car => car.Year == searchYear.Value);
            }

            // Execute the query and retrieve the filtered cars
            var filteredCars = await cars.ToListAsync();

            // Return the list of filtered cars to a view
            return View(filteredCars);
        }

        private async Task<User> GetRandomMechanic()
        {
            // Get all users who have the "Mechanic" role
            var mechanics = await _userManager.GetUsersInRoleAsync("Mechanic");

            // If there are no mechanics, return null
            if (!mechanics.Any())
            {
                return null;
            }

            // Choose a random index
            var random = new Random();
            var index = random.Next(0, mechanics.Count);

            // Return the randomly chosen mechanic
            return mechanics.ElementAt(index);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            var brands = _context.Brands.ToList() ?? new List<Brand>(); // Retrieve all brands from the database or initialize an empty list if null
            var models = _context.Models.ToList() ?? new List<Model>(); // Retrieve all models from the database or initialize an empty list if null

            ViewBag.Brands = brands;
            ViewBag.Models = models;

            ViewBag.UserID = User.FindFirstValue(ClaimTypes.NameIdentifier); // Set the user ID
            // Display a form for creating a new car
            return View();
        }

        [HttpGet]
        public IActionResult GetAllBrands()
        {
            var allBrands = _context.Brands
                .Select(b => new { BrandID = b.BrandId, BrandName = b.Name })
                .ToList();

            return Json(allBrands);
        }

        [HttpGet]
        public async Task<IActionResult> GetModelsByBrand(int selectedBrandId)
        {
            // Retrieve models corresponding to the selected brand from the database
            var models = await _context.Models
                .Where(m => m.BrandId == selectedBrandId)
                .Select(m => new { ModelId = m.ModelId, ModelName = m.Name })
                .ToListAsync();

            return Json(models);
        }

        [HttpGet]
        public IActionResult GetBrandsByName(string searchTerm)
        {
            // Perform a case-insensitive search for brands containing the searchTerm
            var matchingBrands = _context.Brands
                .Where(b => b.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(b => new { BrandID = b.BrandId, BrandName = b.Name })
                .ToList();

            return Json(matchingBrands);
        }

        [HttpGet]
        public IActionResult GetModelsBySelectedBrand(int selectedBrandId)
        {
            // Retrieve models corresponding to the selected brand from the database
            var models = _context.Models
                .Where(m => m.BrandId == selectedBrandId)
                .Select(m => new { ModelId = m.ModelId, ModelName = m.Name })
                .ToList();

            return Json(models);
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrandId,ModelId,Year,Mileage,History,UserID,Registration,Status,Colour")] Car car, IFormFile carImageFile)
        {
            // Check if the submitted form data is valid
            if (ModelState.IsValid)
            {
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

        public async Task<IActionResult> Details(int? id, int? referrerId = 0)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            // Fetch brand name
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == int.Parse(car.BrandId));
            var brandName = brand != null ? brand.Name : "Unknown Brand";

            // Fetch model name
            var model = await _context.Models.FirstOrDefaultAsync(m => m.ModelId == int.Parse(car.ModelId));
            var modelName = model != null ? model.Name : "Unknown Model";

            // Pass brand name and model name to the view
            ViewData["BrandName"] = brandName;
            ViewData["ModelName"] = modelName;
            ViewData["ReferrerId"] = referrerId;

            return View(car);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car); // Assuming you have a view named "ChangeStatus" that matches this action
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int carId, string Status) // Notice the parameter name matches the form field name
        {
            if (string.IsNullOrEmpty(Status))
            {
                ModelState.AddModelError("", "Status is required.");
                return View(await _context.Cars.FindAsync(carId));
            }

            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                return NotFound();
            }

            car.Status = Status;

            try
            {
                _context.Update(car);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Car status updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(carId))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while updating the status. Please try again.");
                    return View(car);
                }
            }
        }

        // EDIT (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Edit(int id, [Bind("CarID,BrandId,ModelId,Year,Mileage,History,UserID,Registration,Status,Colour")] Car car)
        {
            // Check if the provided ID matches the car's ID
            if (id != car.CarID)
            {
                return NotFound();
            }

            // Retrieve the existing car data from the database, including the current image
            var existingCar = await _context.Cars.FindAsync(id);

            // Check if the submitted form data is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Retain the existing image without updating it
                    // car.CarImage = existingCar.CarImage;
                    car.UserID = existingCar.UserID;
                    // Update other car properties in the database context
                    existingCar.BrandId = car.BrandId;
                    existingCar.ModelId = car.ModelId;
                    existingCar.Year = car.Year;
                    existingCar.Mileage = car.Mileage;
                    existingCar.History = car.History;
                    existingCar.Registration = car.Registration;
                    existingCar.Status = car.Status;
                    existingCar.Colour = car.Colour;

                    // Update the car in the database context
                    _context.Update(existingCar);

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
            return View(existingCar);
        }

        // GET: Car/EditImage/5
        // EDIT (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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