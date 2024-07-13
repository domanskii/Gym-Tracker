using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTracker.Data;
using GymTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GymTracker.Controllers
{
    [Authorize]
    public class UserrsController : Controller
    {
        private readonly GymTrackerContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserrsController(GymTrackerContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                var allUsers = await _context.Userrs.ToListAsync();
                return View(allUsers);
            }
            else
            {
                var userRecords = await _context.Userrs
                    .Where(u => u.UserId == user.Id)
                    .Include(u => u.User)
                    .ToListAsync();
                return View(userRecords);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userr == null)
            {
                return NotFound();
            }

            return View(userr);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Age,Gender,Height,Weight,RegistrationDate,LastLoginDate")] Userr userr)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                userr.UserId = user.Id;

                if (!userr.RegistrationDate.HasValue)
                    userr.RegistrationDate = DateTime.Now;

                if (!userr.LastLoginDate.HasValue)
                    userr.LastLoginDate = DateTime.Now;

                _context.Add(userr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userr);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetail = await _context.Userrs.FindAsync(id);
            if (userDetail == null)
            {
                return NotFound();
            }

            return View(userDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,Email,Age,Gender,Height,Weight,RegistrationDate,LastLoginDate")] Userr userr)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingUserr = await _context.Userrs.FindAsync(id);

            if (existingUserr == null)
            {
                return NotFound();
            }

            if (existingUserr.UserId != user.Id)
            {
                return Forbid();
            }

            if (!userr.RegistrationDate.HasValue)
                userr.RegistrationDate = DateTime.Now;

            if (!userr.LastLoginDate.HasValue)
                userr.LastLoginDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                existingUserr.FirstName = userr.FirstName;
                existingUserr.LastName = userr.LastName;
                existingUserr.Email = userr.Email;
                existingUserr.Age = userr.Age;
                existingUserr.Gender = userr.Gender;
                existingUserr.Height = userr.Height;
                existingUserr.Weight = userr.Weight;
                existingUserr.RegistrationDate = userr.RegistrationDate;
                existingUserr.LastLoginDate = userr.LastLoginDate;

                _context.Update(existingUserr);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Your profile has been updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userr);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userr == null)
            {
                return NotFound();
            }

            return View(userr);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Userrs == null)
            {
                return Problem("Entity set 'GymTrackerContext.Userr' is null.");
            }
            var userr = await _context.Userrs.FindAsync(id);
            if (userr != null)
            {
                _context.Userrs.Remove(userr);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserrExists(int id)
        {
            return (_context.Userrs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
