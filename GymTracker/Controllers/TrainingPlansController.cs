using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTracker.Data;
using GymTracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymTracker.Controllers
{
    [Authorize]
    public class TrainingPlansController : Controller
    {
        private readonly GymTrackerContext _context;

        public TrainingPlansController(GymTrackerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            if (User.IsInRole("Admin"))
            {
                var trainingPlans = await _context.TrainingPlans
                    .Include(t => t.ExercisePlans)
                    .ThenInclude(ep => ep.Exercise)
                    .ToListAsync();

                var users = await _context.Users.ToDictionaryAsync(u => u.Id, u => u);

                ViewBag.Users = users;

                return View(trainingPlans);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var userTrainingPlans = await _context.TrainingPlans
                    .Include(t => t.ExercisePlans)
                    .ThenInclude(ep => ep.Exercise)
                    .Where(t => t.UserId == userId)
                    .ToListAsync();
                return View(userTrainingPlans);
            }

            return NotFound();
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingPlan = await _context.TrainingPlans
                .Include(t => t.ExercisePlans)
                .ThenInclude(e => e.Exercise)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainingPlan == null)
            {
                return NotFound();
            }

            return View(trainingPlan);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.Users, "Id", "Email");
            ViewBag.Exercises = new SelectList(_context.Exercises, "Id", "Name");
            return View(new TrainingPlan());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id, UserId, Name, ExercisePlans")] TrainingPlan trainingPlan)
        {
            ModelState.Keys.Where(k => k.Contains("Exercise")).ToList().ForEach(key => ModelState.Remove(key));

            if (ModelState.IsValid)
            {
                _context.Add(trainingPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Count > 0)
                {
                    Console.WriteLine(
                        $"{entry.Key}: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            ViewBag.Users = new SelectList(_context.Users, "Id", "Email");
            ViewBag.Exercises = new SelectList(_context.Exercises, "Id", "Name");
            return View(trainingPlan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingPlan = await _context.TrainingPlans
                .Include(tp => tp.ExercisePlans)
                .ThenInclude(ep => ep.Exercise)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainingPlan == null)
            {
                return NotFound();
            }

            ViewBag.Users = new SelectList(_context.Users, "Id", "Email", trainingPlan.UserId);
            ViewBag.Exercises = new SelectList(_context.Exercises, "Id", "Name");

            return View(trainingPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,ExercisePlans")] TrainingPlan trainingPlan, [FromForm] List<int> RemovedExerciseIds)
        {
            if (id != trainingPlan.Id)
            {
                return NotFound();
            }

            ModelState.Keys.Where(k => k.Contains("Exercise")).ToList().ForEach(key => ModelState.Remove(key));

            if (ModelState.IsValid)
            {
                try
                {
                    // Pobierz oryginalny plan treningowy z bazy danych
                    var originalPlan = await _context.TrainingPlans
                        .Include(tp => tp.ExercisePlans)
                        .FirstOrDefaultAsync(tp => tp.Id == id);

                    if (originalPlan == null)
                    {
                        return NotFound();
                    }

                    // Aktualizuj właściwości planu treningowego
                    originalPlan.UserId = trainingPlan.UserId;
                    originalPlan.Name = trainingPlan.Name;

                    // Usuń ćwiczenia
                    if (RemovedExerciseIds != null && RemovedExerciseIds.Any())
                    {
                        var exercisesToRemove = originalPlan.ExercisePlans
                            .Where(ep => RemovedExerciseIds.Contains(ep.Id))
                            .ToList();
                        _context.ExercisePlans.RemoveRange(exercisesToRemove);
                    }

                    // Aktualizuj ćwiczenia
                    foreach (var exercisePlan in trainingPlan.ExercisePlans)
                    {
                        var existingExercisePlan = originalPlan.ExercisePlans
                            .FirstOrDefault(ep => ep.Id == exercisePlan.Id);

                        if (existingExercisePlan != null)
                        {
                            existingExercisePlan.ExerciseId = exercisePlan.ExerciseId;
                            existingExercisePlan.Sets = exercisePlan.Sets;
                            existingExercisePlan.Repetitions = exercisePlan.Repetitions;
                            existingExercisePlan.Weight = exercisePlan.Weight;
                        }
                        else
                        {
                            originalPlan.ExercisePlans.Add(exercisePlan);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingPlanExists(trainingPlan.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Users = new SelectList(_context.Users, "Id", "Email", trainingPlan.UserId);
            ViewBag.Exercises = new SelectList(_context.Exercises, "Id", "Name");

            return View(trainingPlan);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingPlan = await _context.TrainingPlans
                .Include(t => t.ExercisePlans)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainingPlan == null)
            {
                return NotFound();
            }

            return View(trainingPlan);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainingPlan = await _context.TrainingPlans.FindAsync(id);
            _context.TrainingPlans.Remove(trainingPlan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingPlanExists(int id)
        {
            return _context.TrainingPlans.Any(e => e.Id == id);
        }

        public async Task<PartialViewResult> GetUserTrainingPlan()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var trainingPlan = await _context.TrainingPlans
                    .Include(t => t.ExercisePlans)
                    .ThenInclude(ep => ep.Exercise)
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                if (trainingPlan == null)
                {
                    Console.WriteLine("No training plan found for user: " + userId);
                }

                return PartialView("TrainingPlanPanel", trainingPlan);
            }

            Console.WriteLine("User ID is null or empty");
            return PartialView("TrainingPlanPanel", null);
        }
    }
}
