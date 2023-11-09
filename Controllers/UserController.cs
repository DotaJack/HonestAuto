﻿using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly MarketplaceContext _context;

    public UserController(UserManager<User> userManager, MarketplaceContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // INDEX (Read/List all)
    public IActionResult Index()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    // CREATE (GET)
    public IActionResult Create()
    {
        return View();
    }

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // EDIT (GET)
    public async Task<IActionResult> Edit(int? id)
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

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("UserID,FirstName,LastName,Email,PhoneNumber,Password,Address")] User user)
    {
        if (id != user.UserID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.UserID == user.UserID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // DELETE (GET)
    public async Task<IActionResult> Delete(int? id)
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

    // DELETE (POST/Confirmed)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // CheckRoleAndRedirect function
    public async Task<IActionResult> CheckRoleAndRedirect()
    {
        var user = await _userManager.GetUserAsync(User);

        if (await _userManager.IsInRoleAsync(user, "Buyer"))
        {
            // User is a buyer, redirect them to the buyer dashboard.
            return RedirectToAction("BuyerDashboard", "Buyer");
        }
        else if (await _userManager.IsInRoleAsync(user, "Seller"))
        {
            // User is a seller, redirect them to the seller dashboard.
            return RedirectToAction("SellerDashboard", "Seller");
        }
        else
        {
            // Handle other roles or unauthorized access as needed.
            return RedirectToAction("AccessDenied", "Home");
        }
    }
}