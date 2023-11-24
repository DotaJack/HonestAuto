using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HonestAuto.Controllers
{
    public class UserController : Controller
    {
        private readonly MarketplaceContext _context;

        public UserController(MarketplaceContext context)
        {
            _context = context;
        }

        // INDEX (Read/List all users)
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
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
                        user.ProfileImage = memoryStream.ToArray();
                    }
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,FirstName,LastName,Email,PhoneNumber,Password,Address,Role")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _context.Users.FindAsync(id);
                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }

                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.Email = user.Email;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                    userToUpdate.Password = user.Password;
                    userToUpdate.Address = user.Address;
                    userToUpdate.Role = user.Role;

                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Handle specific database update errors if needed
                }
            }
            return View(user);
        }

        // DELETE (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.FirstOrDefaultAsync(m => m.UserID == id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch (Exception ex)
            {
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
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}