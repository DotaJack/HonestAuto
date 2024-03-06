using HonestAuto.Data;
using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace HonestAuto.Controllers
{
    public class CarController : Controller
    {
        private readonly MarketplaceContext _context;
        private readonly EmailService _emailService;

        // Inject UserManager into your controller
        private readonly UserManager<User> _userManager;

        public CarController(MarketplaceContext context, UserManager<User> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            // Ensure there's a user authenticated and email claim exists
            var userEmail = User?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                // If no user authenticated or email claim doesn't exist, handle accordingly
                return RedirectToAction("Login", "Account"); // Redirect to login or handle unauthorized access
            }

            try
            {
                // Fetch car view models from the database
                var carViewModels = await _context.Cars
                    .Join(
                        _context.Brands,
                        car => car.BrandId,
                        brand => brand.BrandId.ToString(),
                        (car, brand) => new { Car = car, Brand = brand })
                    .Join(
                        _context.Models,
                        joinResult => joinResult.Car.ModelId,
                        model => model.ModelId.ToString(),
                        (joinResult, model) => new CarViewModel
                        {
                            CarID = joinResult.Car.CarID,
                            BrandName = joinResult.Brand.Name ?? "Unknown Brand",
                            ModelName = model.Name ?? "Unknown Model",
                            Year = joinResult.Car.Year,
                            Mileage = joinResult.Car.Mileage,
                            History = joinResult.Car.History,
                            UserID = joinResult.Car.UserID,
                            CarImage = joinResult.Car.CarImage,
                            Registration = joinResult.Car.Registration,
                            Status = joinResult.Car.Status,
                            Colour = joinResult.Car.Colour
                        })
                    .ToListAsync();

                // Send email to the user
                await _emailService.SendEmailAsync(userEmail, "Car Created Successfully", "Your car has been successfully added to our system.");

                // Return the car view models to the view
                return View(carViewModels);
            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"An error occurred: {ex.Message}");
                return RedirectToAction("Error", "Home"); // Redirect to error page or handle the error
            }
        }

        private async Task<string> GetRandomMechanicId()
        {
            // Get all users who have the "Mechanic" role
            var mechanics = await _userManager.GetUsersInRoleAsync("Mechanic");

            // If there are no mechanics, return null or throw an exception, depending on your requirement
            if (!mechanics.Any())
            {
                return null; // or throw new Exception("No mechanics found.");
            }

            // Choose a random index
            var random = new Random();
            var index = random.Next(0, mechanics.Count);

            // Retrieve the ID of the randomly chosen mechanic
            var mechanicIdString = mechanics.ElementAt(index).Id;

            return mechanicIdString;
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            var brands = _context.Brands.ToList() ?? new List<Brand>(); // Retrieve all brands from the database or initialize an empty list if null
            var models = _context.Models.ToList() ?? new List<Model>(); // Retrieve all models from the database or initialize an empty list if null
                                                                        // Retrieve the email of the currently logged-in user
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
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
                // Retrieve the email of the currently logged-in user
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                // Ensure userEmail is not null before proceeding
                if (userEmail == null)
                {
                    // Log or handle the case where the user's email is not available
                    // For now, let's log a message and return a bad request response
                    Console.WriteLine("User email not found.");
                    return BadRequest("User email not found.");
                }

                // Add the car to the database context
                _context.Add(car);

                // Save changes to the database to get the CarID generated by the database
                await _context.SaveChangesAsync();

                // Retrieve a random mechanic's ID
                var mechanicId = await GetRandomMechanicId();

                // Create a new CarEvaluation entity using the newly generated CarID and the random mechanic's ID
                var carEvaluation = new CarEvaluation
                {
                    CarID = car.CarID, // The CarID from the newly created Car
                    EvaluationStatus = "Submitted", // Set the EvaluationStatus as "Submitted"
                    EvaluationDate = DateTime.UtcNow, // Set the current UTC date and time
                    MechanicID = mechanicId, // Set the MechanicID to the ID of the random mechanic
                    CarValue = 20000, // Set the CarValue to 20000 for now
                    EvaluationSummary = "This is a summary of the car evaluation."
                    // Other properties can be initialized here if needed
                };

                // Add the new CarEvaluation to the context
                _context.CarEvaluations.Add(carEvaluation);
                // Send email to the user

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

        public async Task<IActionResult> ViewAds()
        {
            var cars = await _context.Cars
                .Where(c => c.Status == "Visible") // Filter ads by status
                .Join(
                    _context.Brands,
                    car => car.BrandId,
                    brand => brand.BrandId.ToString(),
                    (car, brand) => new { Car = car, Brand = brand })
                .Join(
                    _context.Models,
                    joinResult => joinResult.Car.ModelId,
                    model => model.ModelId.ToString(),
                    (joinResult, model) => new CarViewModel
                    {
                        CarID = joinResult.Car.CarID,
                        BrandName = joinResult.Brand.Name ?? "Unknown Brand",
                        ModelName = model.Name ?? "Unknown Model",
                        Year = joinResult.Car.Year,
                        Mileage = joinResult.Car.Mileage,
                        History = joinResult.Car.History,
                        UserID = joinResult.Car.UserID,
                        Registration = joinResult.Car.Registration,
                        Status = joinResult.Car.Status,
                        Colour = joinResult.Car.Colour,
                        CarImage = joinResult.Car.CarImage
                    })
                .ToListAsync();

            if (cars == null || !cars.Any())
            {
                return NotFound();
            }

            return View(cars);
        }

        public async Task<IActionResult> SearchCars(SearchViewModel model)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(model.SelectedBrand))
            {
                query = query.Where(c => c.BrandId == model.SelectedBrand);
            }

            if (!string.IsNullOrEmpty(model.SelectedModel))
            {
                query = query.Where(c => c.ModelId == model.SelectedModel);
            }

            if (model.SelectedYear.HasValue)
            {
                query = query.Where(c => c.Year == model.SelectedYear.Value);
            }

            if (!string.IsNullOrEmpty(model.SelectedColour))
            {
                query = query.Where(c => c.Colour == model.SelectedColour);
            }

            if (model.MinMileage.HasValue)
            {
                query = query.Where(c => c.Mileage >= model.MinMileage.Value);
            }

            if (model.MaxMileage.HasValue)
            {
                query = query.Where(c => c.Mileage <= model.MaxMileage.Value);
            }

            var carViewModels = await query
                .Join(
                    _context.Brands,
                    car => car.BrandId,
                    brand => brand.BrandId.ToString(), // Convert BrandId to string for comparison
                    (car, brand) => new { Car = car, Brand = brand })
                .Join(
                    _context.Models,
                    joinResult => joinResult.Car.ModelId,
                    model => model.ModelId.ToString(), // Convert ModelId to string for comparison
                    (joinResult, model) => new CarViewModel
                    {
                        CarID = joinResult.Car.CarID,
                        BrandName = joinResult.Brand.Name ?? "Unknown Brand",
                        ModelName = model.Name ?? "Unknown Model",
                        Year = joinResult.Car.Year,
                        Mileage = joinResult.Car.Mileage,
                        History = joinResult.Car.History,
                        Colour = joinResult.Car.Colour
                    })
                .ToListAsync();

            model.SearchResults = carViewModels;

            return View(model);
        }

        public async Task<IActionResult> ViewUserCars(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound(); // Handle invalid user ID
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); // Handle user not found
            }

            var carViewModels = await _context.Cars
                .Where(car => car.UserID == userId) // Filter by user ID
                .Join(
                    _context.Brands,
                    car => car.BrandId,
                    brand => brand.BrandId.ToString(), // Convert BrandId to string for comparison
                    (car, brand) => new { Car = car, Brand = brand })
                .Join(
                    _context.Models,
                    joinResult => joinResult.Car.ModelId,
                    model => model.ModelId.ToString(), // Convert ModelId to string for comparison
                    (joinResult, model) => new CarViewModel
                    {
                        CarID = joinResult.Car.CarID,
                        BrandName = joinResult.Brand.Name ?? "Unknown Brand",
                        ModelName = model.Name ?? "Unknown Model",
                        Year = joinResult.Car.Year,
                        Mileage = joinResult.Car.Mileage,
                        History = joinResult.Car.History,
                        UserID = joinResult.Car.UserID,
                        CarImage = joinResult.Car.CarImage,
                        Registration = joinResult.Car.Registration,
                        Status = joinResult.Car.Status,
                        Colour = joinResult.Car.Colour
                    })
                .ToListAsync();

            var userEmail = user.Email;
            var tupleModel = new Tuple<string, IEnumerable<CarViewModel>>(userEmail, carViewModels);

            return View(tupleModel);
        }

        public async Task<IActionResult> UserCars()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle the case where user is not logged in
                return RedirectToAction("Login", "Account");
            }

            var carViewModels = await _context.Cars
                .Where(car => car.UserID == user.Id) // Filter by user ID
                .Join(
                    _context.Brands,
                    car => car.BrandId,
                    brand => brand.BrandId.ToString(), // Convert BrandId to string for comparison
                    (car, brand) => new { Car = car, Brand = brand })
                .Join(
                    _context.Models,
                    joinResult => joinResult.Car.ModelId,
                    model => model.ModelId.ToString(), // Convert ModelId to string for comparison
                    (joinResult, model) => new CarViewModel
                    {
                        CarID = joinResult.Car.CarID,
                        BrandName = joinResult.Brand.Name ?? "Unknown Brand",
                        ModelName = model.Name ?? "Unknown Model",
                        Year = joinResult.Car.Year,
                        Mileage = joinResult.Car.Mileage,
                        History = joinResult.Car.History,
                        UserID = joinResult.Car.UserID,
                        CarImage = joinResult.Car.CarImage,
                        Registration = joinResult.Car.Registration,
                        Status = joinResult.Car.Status,
                        Colour = joinResult.Car.Colour
                    })
                .ToListAsync();

            return View(carViewModels);
        }

        private bool CarExists(int id)
        {
            // Check if a car with the provided ID exists in the database
            return _context.Cars.Any(e => e.CarID == id);
        }
    }
}