using HonestAuto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonestAuto.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult StartChat(string id)
        {
            // Perform any necessary logic to set up the chat, e.g., create a conversation
            // and navigate to the chat view.
            // You can also pass the user's ID to the chat view if needed.
            return RedirectToAction("ChatWithUser", new { receiverId = id });
        }

        // GET: /User
        public IActionResult Index()
        {
            // Retrieve all users
            var users = _userManager.Users;
            return View(users);
        }

        // GET: /User/Create
        public IActionResult Create()
        {
            // Get all available roles from the RoleManager as a list of role names
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Pass roles to the view using ViewBag
            ViewBag.Roles = roles;

            return View();
        }

        // POST: /User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string selectedRole)
        {
            if (ModelState.IsValid)
            {
                // Create the user with the provided password
                var result = await _userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                {
                    // Check if a role was selected and add the user to that role
                    if (!string.IsNullOrEmpty(selectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, selectedRole);
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Get all available roles from the RoleManager
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Pass roles and selected role to the view
            ViewBag.Roles = new SelectList(roles, selectedRole);

            // If the model is not valid or if there are errors, return to the Create view
            return View(user);
        }

        // GET: /User/Edit/1
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Get all available roles from the RoleManager as a list of role names
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Fetch the user's current role
            var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            // Pass roles to the view using ViewBag
            ViewBag.Roles = roles;

            return View(user);
        }

        // POST: /User/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string userName, string email, string selectedRole, string currentRole)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Update user's properties
            user.UserName = userName;
            user.Email = email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Update user's role if it has changed
                await UpdateUserRole(user, selectedRole, currentRole);

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Get all available roles for role selection
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Pass roles to the view using ViewBag as SelectList
            ViewBag.Roles = new SelectList(roles, selectedRole);

            return View(user);
        }

        // Helper method to update user role and handle role changes
        private async Task UpdateUserRole(User user, string newRole, string currentRole)
        {
            if (newRole != currentRole)
            {
                // Remove the user from the current role
                await _userManager.RemoveFromRoleAsync(user, currentRole);

                // Add the user to the new role
                await _userManager.AddToRoleAsync(user, newRole);
            }
        }

        // GET: /User/Details/1
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Additional action to change a user's role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Remove the user from all current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add the user to the new role
                await _userManager.AddToRoleAsync(user, newRole);

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}