﻿using HonestAuto.Data;
using HonestAuto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Auto - Generated by Visual Studio and partially edited by me
namespace HonestAuto.Controllers
{
    public class MechanicController : Controller
    {
        private readonly MarketplaceContext _context;

        public MechanicController(MarketplaceContext context)
        {
            _context = context;
            // Constructor for MechanicController that takes an instance of MarketplaceContext.
        }

        // INDEX (Read/List all)
        public IActionResult Index()
        {
            var mechanics = _context.Mechanics.ToList();
            // Retrieve all mechanics from the database and convert them to a list.
            return View(mechanics);
            // Return a view with the list of mechanics.
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
            // Display the Create view for creating a new mechanic.
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mechanic mechanic)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mechanic);
                await _context.SaveChangesAsync();
                // If the model is valid, add the mechanic to the database and save changes.
                return RedirectToAction(nameof(Index));
                // Redirect to the Index action after successfully creating the mechanic.
            }
            return View(mechanic);
            // If the model is not valid, return the Create view with validation errors.
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }
            return View(mechanic);
            // Display the Edit view for editing an existing mechanic.
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MechanicID,FirstName,LastName,Email,Password,Address")] Mechanic mechanic)
        {
            if (id != mechanic.MechanicID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mechanic);
                    await _context.SaveChangesAsync();
                    // If the model is valid, update the mechanic in the database and save changes.
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Mechanics.Any(m => m.MechanicID == mechanic.MechanicID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
                // Redirect to the Index action after successfully editing the mechanic.
            }
            return View(mechanic);
            // If the model is not valid, return the Edit view with validation errors.
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }
            return View(mechanic);
            // Display the Delete view for deleting an existing mechanic.
        }

        // DELETE (POST/Confirmed)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }
            _context.Mechanics.Remove(mechanic);
            await _context.SaveChangesAsync();
            // Remove the mechanic from the database and save changes.
            return RedirectToAction(nameof(Index));
            // Redirect to the Index action after successfully deleting the mechanic.
        }
    }
}