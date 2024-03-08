using Azure.Messaging;
using HonestAuto.Data;
using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
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

                // Fetch the user email for each car
                foreach (var car in carViewModels)
                {
                    var user = await _userManager.FindByIdAsync(car.UserID);
                    if (user != null)
                    {
                        car.UserEmail = user.Email;
                    }
                }

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

        [Authorize(Roles = "Admin,Seller")]
        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            var brands = _context.Brands.ToList() ?? new List<Brand>(); // Retrieve all brands from the database or initialize an empty list if null
            var models = _context.Models.ToList() ?? new List<Model>(); // Retrieve all models from the database or initialize an empty list if null

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
        [Authorize(Roles = "Admin,Seller")]
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

                    // Other properties can be initialized here if needed
                };

                // Add the new CarEvaluation to the context
                _context.CarEvaluations.Add(carEvaluation);
                var emailSubject = "Welcome to Honest Auto - Your Car is Registered";
                // Call the method to send the email
                var emailContent = $@"
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Your Car is Added to Our System</title>
    <style>
        body {{ background-color: #f2f2f2; font-family: Arial, sans-serif; margin: 0; padding: 0; }}
        .container {{ background-color: #ffffff; max-width: 600px; margin: auto; padding: 20px; }}
        .content {{ padding: 20px; text-align: left; color: #333333; }}
        .footer {{ padding: 20px; text-align: center; }}
        a {{ color: #1A55E8; text-decoration: none; }}
        .button {{ background-color: #004aad; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
        .logo {{ height: 100px; vertical-align: middle; margin-right: 10px; }}
        .type {{ height: 100px; vertical-align: middle; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""content"">
            <h2>Hello,</h2>
            <p>We have successfully added your car to our system! We're excited to get you started with our premium auto services.</p>
            <p>Please bring your car to the following address:</p>
            <p><strong>Ship St, Penrose Quay, Victorian Quarter, Cork, T23 TR7</strong></p>
            <p>If you have any questions or need further assistance, don't hesitate to contact us. You can also click the button below for directions:</p>
            <p style=""text-align: center;"">
                <a href=""https://www.google.com/maps/place/Ship+St+Penrose+Quay+Victorian+Quarter+Cork+T23+TR7/"" target=""_blank"" class=""button"">Get Directions</a>
            </p>
        </div>
        <div class=""footer"">
            <p>Thank you for choosing Honest Auto!</p>
            <img src=""https://i.ibb.co/m6cXxB1/Honest-Auto-Logo.png"" alt=""Honest Auto Logo"" class=""logo"" />
            <img src=""https://i.ibb.co/6RzfjLM/Honest-Auto-Type.png"" alt=""Honest Auto Type"" class=""type"" />
        </div>
    </div>
</body>
</html>

";
                await _emailService.SendEmailAsync(userEmail, emailSubject, emailContent, true);
                // Save changes to the database to save the CarEvaluation
                await _context.SaveChangesAsync();

                // Redirect based on the user's role
                if (User.IsInRole("Admin"))
                {
                    // If the user is an admin, redirect them to the index page
                    return RedirectToAction("Index");
                }
                else if (User.IsInRole("Seller"))
                {
                    // If the user is a seller, redirect them to the UserCars page
                    return RedirectToAction("UserCars");
                }
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

            // Fetch the car entity by ID
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarID == id);

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

            // Get the car evaluation and its ID
            var carEvaluation = await _context.CarEvaluations.FirstOrDefaultAsync(ce => ce.CarID == car.CarID);

            // Fetch the user email for the current car
            var user = await _userManager.FindByIdAsync(car.UserID);
            var userEmail = user != null ? user.Email : "Unknown User Email";

            // Pass brand name, model name, evaluation ID, and other details to the view
            ViewData["CurrentUserId"] = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            ViewData["BrandName"] = brandName;
            ViewData["ModelName"] = modelName;
            ViewData["ReferrerId"] = referrerId;
            ViewData["CarEvaluationId"] = carEvaluation != null ? carEvaluation.CarEvaluationID : 0;
            ViewData["UserEmail"] = userEmail;

            // Create the view model and populate its properties
            var carViewModel = new CarViewModel
            {
                UserID = car.UserID, // Populate UserID
                CarID = car.CarID,
                BrandName = brandName,
                ModelName = modelName,
                Year = car.Year,
                Mileage = car.Mileage,
                History = car.History,
                CarImage = car.CarImage,
                Registration = car.Registration,
                Status = car.Status,
                Colour = car.Colour,
                IsEvaluationComplete = carEvaluation != null && carEvaluation.EvaluationStatus == "Completed",
                CarValue = carEvaluation != null ? (double?)carEvaluation.CarValue : null,

                CarEvaluationID = carEvaluation != null ? carEvaluation.CarEvaluationID : 0, // Include CarEvaluationId
                UserEmail = userEmail // Include UserEmail
            };

            return View(carViewModel);
        }

        [Authorize(Roles = "Admin,Seller")]
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

        [Authorize(Roles = "Admin,Seller")]
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

        [Authorize(Roles = "Admin,Seller")]
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

        [Authorize(Roles = "Admin,Seller")]
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
        [Authorize(Roles = "Admin,Seller")]
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
        [Authorize(Roles = "Admin,Seller")]
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
        [Authorize(Roles = "Admin,Seller")]
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
        [Authorize(Roles = "Admin,Seller")]
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
                .Where(c => c.Status == "Visible")
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

            foreach (var car in cars)
            {
                var user = await _userManager.FindByIdAsync(car.UserID);
                if (user != null)
                {
                    car.UserEmail = user.Email;
                }
            }

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
                    brand => brand.BrandId.ToString(),
                    (car, brand) => new { Car = car, Brand = brand })
                .Join(
                    _context.Models,
                    joinResult => joinResult.Car.ModelId,
                    model => model.ModelId.ToString(),
                    (joinResult, model) => new { Car = joinResult.Car, Brand = joinResult.Brand, Model = model })
                .Join(
                    _context.Users,
                    joinResult => joinResult.Car.UserID,
                    user => user.Id,
                    (joinResult, user) => new { Car = joinResult.Car, Brand = joinResult.Brand, Model = joinResult.Model, User = user })
                .Select(result => new CarViewModel
                {
                    CarID = result.Car.CarID,
                    BrandName = result.Brand.Name ?? "Unknown Brand",
                    ModelName = result.Model.Name ?? "Unknown Model",
                    Year = result.Car.Year,
                    Mileage = result.Car.Mileage,
                    History = result.Car.History,
                    Colour = result.Car.Colour,
                    CarValue = _context.CarEvaluations
                                .Where(ce => ce.CarID == result.Car.CarID)
                                .OrderByDescending(ce => ce.EvaluationDate)
                                .Select(ce => ce.CarValue)
                                .FirstOrDefault(),

                    UserEmail = result.User.Email // Include user email
                })
                .ToListAsync();

            model.SearchResults = carViewModels;

            return View(model);
        }

        [Authorize]
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

            var userEmail = user.Email;

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

            var tupleModel = new Tuple<string, IEnumerable<CarViewModel>>(userEmail, carViewModels);

            return View(tupleModel);
        }

        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> UserCars()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle the case where user is not logged in
                return RedirectToAction("Login", "Account");
            }

            // Fetch the user's email using their ID
            var userEmail = await _context.Users
                .Where(u => u.Id == user.Id)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (userEmail == null)
            {
                // Handle the case where user's email is not found
                // You can log an error or return an appropriate response
                return BadRequest("User email not found.");
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
                    (joinResult, model) => new { Car = joinResult.Car, Brand = joinResult.Brand, Model = model })
                .Join(
                    _context.CarEvaluations,
                    joinResult => joinResult.Car.CarID,
                    evaluation => evaluation.CarID,
                    (joinResult, evaluation) => new CarViewModel
                    {
                        CarID = joinResult.Car.CarID,
                        BrandName = joinResult.Brand.Name ?? "Unknown Brand",
                        ModelName = joinResult.Model.Name ?? "Unknown Model",
                        Year = joinResult.Car.Year,
                        Mileage = joinResult.Car.Mileage,
                        History = joinResult.Car.History,
                        UserID = joinResult.Car.UserID,
                        CarImage = joinResult.Car.CarImage,
                        Registration = joinResult.Car.Registration,
                        Status = joinResult.Car.Status,
                        Colour = joinResult.Car.Colour,
                        IsEvaluationComplete = evaluation.EvaluationStatus == "Completed",
                        UserEmail = userEmail // Assign the user's email to the UserEmail property
                    })
                .ToListAsync();

            return View(carViewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCarCountByBrandAndModel()
        {
            var carCounts = await _context.Cars
          .Where(c => !string.IsNullOrEmpty(c.BrandId) && !string.IsNullOrEmpty(c.ModelId))
          .GroupBy(c => new { BrandId = c.BrandId, ModelId = c.ModelId })
          .Select(g => new
          {
              BrandId = g.Key.BrandId,
              ModelId = g.Key.ModelId,
              CarCount = g.Count()
          })
          .ToListAsync();

            var carViewModels = new List<CarViewModel>();
            foreach (var count in carCounts)
            {
                // Retrieve brand and model names using their IDs
                var brandName = await _context.Brands
           .Where(b => b.BrandId.ToString() == count.BrandId)
           .Select(b => b.Name)
           .FirstOrDefaultAsync();

                var modelName = await _context.Models
                    .Where(m => m.ModelId.ToString() == count.ModelId)
                    .Select(m => m.Name)
                    .FirstOrDefaultAsync();

                carViewModels.Add(new CarViewModel
                {
                    BrandId = count.BrandId,
                    ModelId = count.ModelId,
                    BrandName = brandName,
                    ModelName = modelName,
                    CarCount = count.CarCount
                });
            }

            return View(carViewModels);
        }

        private bool CarExists(int id)
        {
            // Check if a car with the provided ID exists in the database
            return _context.Cars.Any(e => e.CarID == id);
        }
    }
}