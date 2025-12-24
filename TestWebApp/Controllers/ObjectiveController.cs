using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;
using TaskTracker.ViewModels.Objectives;
using TaskTracker.Data;
using System.Security.Claims;
using TaskTracker.ViewModels.Project;
using Microsoft.AspNetCore.Authorization;

namespace TaskTracker.Controllers
{
    public class ObjectiveController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ObjectiveController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> Create(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            var project = await _context.Projects.Include(p => p.Manager).FirstOrDefaultAsync(p => p.Id == id);
            bool hasAccess = isAdmin ||
                     project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            ViewBag.AssignedEmployeeId = new SelectList(_userManager.Users, "Id", "FullName");
            ViewBag.StatusList = Enum.GetValues(typeof(ObjectiveStatus))
                              .Cast<ObjectiveStatus>()
                              .Select(s => new SelectListItem
                              {
                                  Value = s.ToString(),
                                  Text = s.ToString()
                              }).ToList();
            var response = new ObjectiveCreateViewModel { 
                ProjectId = id
            };
            return View(response);
        }
        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, ObjectiveCreateViewModel objectiveCreateViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            var project = await _context.Projects.Include(p => p.Manager).FirstOrDefaultAsync(p => p.Id == id);
            bool hasAccess = isAdmin ||
                     project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                Objective objective = new Objective
                {
                    Title = objectiveCreateViewModel.Title,
                    Comment = objectiveCreateViewModel.Comment,
                    ProjectId = id,
                    Status = objectiveCreateViewModel.Status,
                    Priority = objectiveCreateViewModel.Priority,
                    AuthorId = new string(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    AssignedEmployeeId = objectiveCreateViewModel.AssignedEmployeeId
                };

                _context.Add(objective);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Project", new { id = objective.ProjectId });
            }
            ViewBag.StatusList = Enum.GetValues(typeof(ObjectiveStatus))
                              .Cast<ObjectiveStatus>()
                              .Select(s => new SelectListItem
                              {
                                  Value = s.ToString(),
                                  Text = s.ToString()
                              }).ToList();
            ViewBag.AssignedEmployeeId = new SelectList(_userManager.Users, "Id", "FullName");
            return View(objectiveCreateViewModel);
        }
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objective = await _context.Objectives
                .Include(p => p.AssignedEmployee)
                .Include(p => p.Project)
                    .ThenInclude(p => p.Manager)
                .FirstOrDefaultAsync(p => p.Id == id);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            bool hasAccess = isAdmin ||
                     objective.Project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            if (objective == null)
            {
                return NotFound();
            }
            var model = new ObjectiveUpdateViewModel
            {
                Id = objective.Id,
                Title = objective.Title,
                Comment = objective.Comment,
                Status = objective.Status,
                AssignedEmployeeId = objective.AssignedEmployeeId,
                Priority = objective.Priority
            };

            ViewBag.AssignedEmployeeId = new SelectList(_userManager.Users, "Id", "FullName");
            ViewBag.StatusList = Enum.GetValues(typeof(ObjectiveStatus))
                              .Cast<ObjectiveStatus>()
                              .Select(s => new SelectListItem
                              {
                                  Value = s.ToString(),
                                  Text = s.ToString()
                              }).ToList();
            return View(model);
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ObjectiveUpdateViewModel objectiveUpdateViewModel)
        {
            if (id != objectiveUpdateViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var objective = await _context.Objectives
                    .Include(p => p.AssignedEmployee)
                    .Include(p => p.Project)
                        .ThenInclude(p => p.Manager)
                    .FirstOrDefaultAsync(p => p.Id == id);
                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
                bool hasAccess = isAdmin ||
                         objective.Project.ManagerId == currentUser.Id;

                if (!hasAccess)
                {
                    return Forbid();
                }

                objective.Title = objectiveUpdateViewModel.Title;
                objective.Comment = objectiveUpdateViewModel.Comment;
                objective.Status = objectiveUpdateViewModel.Status;
                objective.AssignedEmployeeId = objectiveUpdateViewModel.AssignedEmployeeId;
                objective.Priority = objectiveUpdateViewModel.Priority;

                _context.Update(objective);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = objective.Id });
            }
            ViewBag.AssignedEmployeeId = new SelectList(_userManager.Users, "Id", "FullName");
            ViewBag.StatusList = Enum.GetValues(typeof(ObjectiveStatus))
                              .Cast<ObjectiveStatus>()
                              .Select(s => new SelectListItem
                              {
                                  Value = s.ToString(),
                                  Text = s.ToString()
                              }).ToList();
            return View(objectiveUpdateViewModel);
        }
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objective = await _context.Objectives
                .Include(o => o.AssignedEmployee)
                .Include(p => p.Project)
                    .ThenInclude(p => p.Manager)
                .FirstOrDefaultAsync(p => p.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            bool hasAccess = isAdmin ||
                     objective.Project.ManagerId == currentUser.Id ||
                     objective.AssignedEmployee.Id == currentUser.Id;
                    

            if (!hasAccess)
            {
                return Forbid();
            }

            if (objective == null)
            {
                return NotFound();
            }
            var model = new ChangeStatusViewModel
            {
                Id = objective.Id,
                Status = objective.Status
            };

            ViewBag.StatusList = Enum.GetValues(typeof(ObjectiveStatus))
                              .Cast<ObjectiveStatus>()
                              .Select(s => new SelectListItem
                              {
                                  Value = s.ToString(),
                                  Text = s.ToString()
                              }).ToList();
            return View(model);
        }

        [HttpPost, ActionName("ChangeStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, ChangeStatusViewModel changeStatusViewModel)
        {
            if (id != changeStatusViewModel.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                var objective = await _context.Objectives
                    .Include(o => o.AssignedEmployee)
                    .Include(p => p.Project)
                        .ThenInclude(p => p.Manager)
                    .FirstOrDefaultAsync(p => p.Id == id);

                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
                bool hasAccess = isAdmin ||
                         objective.Project.ManagerId == currentUser.Id ||
                         objective.AssignedEmployee.Id == currentUser.Id;


                if (!hasAccess)
                {
                    return Forbid();
                }

                objective.Status = changeStatusViewModel.Status;

                _context.Update(objective);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = objective.Id });
            }
            ViewBag.StatusList = Enum.GetValues(typeof(ObjectiveStatus))
                              .Cast<ObjectiveStatus>()
                              .Select(s => new SelectListItem
                              {
                                  Value = s.ToString(),
                                  Text = s.ToString()
                              }).ToList();
            return View(changeStatusViewModel);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var objective = await _context.Objectives
                .Include(p => p.AssignedEmployee)
                .Include(p => p.Author)
                .Include(p => p.Project)
                    .ThenInclude(p => p.Manager)
                .Include(p => p.Project)
                    .ThenInclude(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (objective == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            bool hasAccess = isAdmin ||
                     objective.Project.ManagerId == currentUser.Id ||
                     objective.Project.Employees.Any(e => e.Id == currentUser.Id);


            if (!hasAccess)
            {
                return Forbid();
            }

            var response = new ObjectiveDetailsViewModel()
            {
                Id = objective.Id,
                Author = objective.Author,
                Title = objective.Title,
                AssignedEmployee = objective.AssignedEmployee,
                Comment = objective.Comment,
                Status = objective.Status,
                Priority = objective.Priority,
                ProjectId = objective.ProjectId
            };
            return View(response);
        }
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objective = await _context.Objectives
                .Include(p => p.AssignedEmployee)
                .Include(p => p.Author)
                .Include(p => p.Project)
                    .ThenInclude(p => p.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            bool hasAccess = isAdmin ||
                     objective.Project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            if (objective == null)
            {
                return NotFound();
            }

            return View(objective);
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objective = await _context.Objectives
                .Include(p => p.AssignedEmployee)
                .Include(p => p.Author)
                .Include(p => p.Project)
                    .ThenInclude(p => p.Manager)
                .FirstOrDefaultAsync(p => p.Id == id);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            bool hasAccess = isAdmin ||
                     objective.Project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }

            if (objective != null)
            {
                _context.Objectives.Remove(objective);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Project", new { id = objective.ProjectId });
        }
    }
}
