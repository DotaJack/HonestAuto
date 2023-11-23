using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Import the logger namespace

namespace HonestAuto.Controllers
{
    public class UserController : Controller
    {
        private readonly MarketplaceContext _context;
        private readonly ILogger<UserController> _logger; // Create a logger instance

        public UserController(MarketplaceContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger
        }

        // INDEX (Read/List all users)
        public async Task<IActionResult> Index()
        {
            try
            {
                // Retrieve a list of all users from the database asynchronously
                var users = await _context.Users.ToListAsync();
                return View(users); // Display the list of users
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error occurred while getting users");

                // Display an error view or handle the error gracefully
                return View("Error");
            }
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Display the user creation form
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,Password,Address,Role")] User user, IFormFile profileImageFile)
        {
            if (ModelState.IsValid)
            {
                if (profileImageFile != null && profileImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await profileImageFile.CopyToAsync(memoryStream);
                        user.ProfileImage = memoryStream.ToArray(); // Upload and store the profile image
                    }
                }

                _context.Add(user); // Add the user to the database
                await _context.SaveChangesAsync(); // Save changes to the database
                return RedirectToAction(nameof(Index)); // Redirect to the user list after creation
            }
            return View(user); // Return to the create user form with validation errors if any
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound(); // Return a not found view if no user ID is provided
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(); // Return a not found view if the user doesn't exist
                }
                return View(user); // Display the user edit form
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"Error occurred while editing user with ID {id}");

                // Consider a dedicated error view or error handling logic
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,FirstName,LastName,Email,PhoneNumber,Password,Address,Role")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound(); // Return not found if the user ID doesn't match
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _context.Users.FindAsync(id);
                    if (userToUpdate == null)
                    {
                        return NotFound(); // Return not found if the user doesn't exist
                    }

                    // Update user properties
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.Email = user.Email;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                    userToUpdate.Password = user.Password;
                    userToUpdate.Address = user.Address;
                    userToUpdate.Role = user.Role;

                    _context.Update(userToUpdate); // Update the user in the database
                    await _context.SaveChangesAsync(); // Save changes to the database
                    return RedirectToAction(nameof(Index)); // Redirect to the user list after editing
                }
                catch (DbUpdateException ex)
                {
                    // Log the error
                    _logger.LogError(ex, $"Error occurred while updating user with ID {id}");

                    // Handle specific database update errors
                }
            }
            return View(user); // Return to the edit user form with validation errors if any
        }

        // DELETE (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound(); // Return not found if no user ID is provided
                }

                var user = await _context.Users.FirstOrDefaultAsync(m => m.UserID == id);
                if (user == null)
                {
                    return NotFound(); // Return not found if the user doesn't exist
                }
                return View(user); // Display the user delete confirmation page
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"Error occurred while deleting user with ID {id}");

                // Consider a dedicated error view or error handling logic
                return View("Error");
            }
        }

        // DELETE (POST/Confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user); // Remove the user from the database
                    await _context.SaveChangesAsync(); // Save changes to the database
                }
                return RedirectToAction(nameof(Index)); // Redirect to the user list after deletion
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"Error occurred while confirming deletion of user with ID {id}");

                // Display an error view or handle the error gracefully
                return View("Error");
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}