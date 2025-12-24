using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestWebApp.Data;
using TestWebApp.Models;
using TestWebApp.ViewModels.Project;
using static System.Net.Mime.MediaTypeNames;

namespace TestWebApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ProjectController(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize()]
        public async Task<IActionResult> Index(DateTime? startDate, int? priority, string? title, string sortField = "Title")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            IQueryable<Project> projects;

            if (isAdmin)
            {
                projects = _context.Projects.AsQueryable();
            }
            else
            {
                projects = _context.Projects
                    .Where(p => p.ManagerId == currentUser.Id || p.Employees.Any(e => e.Id == currentUser.Id))
                    .AsQueryable();
            }
            if (!string.IsNullOrEmpty(title))
            {
                projects = projects.Where(p => p.Title.Contains(title));
            }

            if (startDate.HasValue)
            {
                projects = projects.Where(p => p.StartTime >= startDate.Value);
            }
                
            if (priority.HasValue)
            {
                projects = projects.Where(p => p.Priority == priority.Value);
            }

            projects = sortField switch
            {
                "Title" => projects.OrderBy(p => p.Title),
                "StartDate" => projects.OrderBy(p => p.StartTime),
                "Priority" => projects.OrderBy(p => p.Priority),
                _ => projects.OrderBy(p => p.Title)
            };

            return View(await projects.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");

            var project = await _context.Projects
                .Include(p => p.Documents)
                .Include(p => p.Employees)
                .Include(p => p.Manager)
                .Include(p => p.Objectives)
                .ThenInclude(p => p.AssignedEmployee)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            bool hasAccess = isAdmin ||
                     project.ManagerId == currentUser.Id ||
                     project.Employees.Any(e => e.Id == currentUser.Id);

            if (!hasAccess)
            {
                return Forbid();
            }

            if (project == null)
            {
                return NotFound();
            }
            var response = new ProjectDetailsViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                CustomerCompany = project.CustomerCompany,
                ExecutorCompany = project.ExecutorCompany,
                Manager = project.Manager,
                ManagerId = project.ManagerId,
                Objectives = project.Objectives,
                Employees = project.Employees,
                Priority = project.Priority,
                StartTime = project.StartTime,
                EndTime = project.EndTime,
                Documents = project.Documents
            };
            return View(response);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Manager = new SelectList(_userManager.Users, "Id","FullName");
            ViewBag.Employees = await _userManager.Users.ToListAsync();
            var response = new ProjectCreateViewModel();
            return View(response);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel projectCreateViewModel)
        {

            if (ModelState.IsValid)
            {
                Project project = new Project 
                { 
                    Title = projectCreateViewModel.Title, 
                    CustomerCompany = projectCreateViewModel.CustomerCompany,
                    ExecutorCompany = projectCreateViewModel.ExecutorCompany,
                    StartTime = projectCreateViewModel.StartTime,
                    EndTime = projectCreateViewModel.EndTime,
                    Priority = projectCreateViewModel.Priority,
                    ManagerId = projectCreateViewModel.ManagerId
                    
                };

                _context.Add(project);
                await _context.SaveChangesAsync();

                foreach (var employeeId in projectCreateViewModel.EmployeesId)
                {
                    _context.ProjectEmployee.Add(new ProjectEmployee
                    {
                        ProjectId = project.Id,
                        EmployeeId = employeeId
                    });
                }
                if (projectCreateViewModel.Documents != null && projectCreateViewModel.Documents.Count > 0)
                {
                    foreach (var file in projectCreateViewModel.Documents)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", project.Id.ToString());
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var document = new Document
                        {
                            FileName = file.FileName,
                            FilePath = $"/uploads/{project.Id.ToString()}/{file.FileName}",
                            ProjectId = project.Id
                        };

                        _context.Documents.Add(document);
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Manager = new SelectList(_userManager.Users, "Id", "FullName");
            ViewBag.Employees = await _userManager.Users.ToListAsync();
            return View(projectCreateViewModel);
        }

        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            if (id == null)
            {
                return NotFound();
            }
            var project = await _context.Projects.Include(p => p.Employees).Include(p => p.Manager).FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }
            bool hasAccess = isAdmin ||
                     project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }

            var model = new ProjectUpdateViewModel
            {
                Id = project.Id,
                Title = project.Title,
                CustomerCompany = project.CustomerCompany,
                ExecutorCompany = project.ExecutorCompany,
                StartTime = project.StartTime,
                EndTime = project.EndTime,
                Priority = project.Priority,
                ManagerId = project.ManagerId,
                EmployeesId = project.ProjectEmployees.Select(pe => pe.EmployeeId).ToList()
            };

            ViewBag.Manager = new SelectList(_userManager.Users, "Id", "FullName");
            ViewBag.Employees = await _userManager.Users.ToListAsync();
            return View(model);
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectUpdateViewModel projectUpdateViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");

            if (id != projectUpdateViewModel.Id)
            {
                return NotFound();
            }

            bool hasAccess = isAdmin ||
                     projectUpdateViewModel.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.Include(p => p.Documents).Include(p => p.Employees).Include(p => p.Manager).FirstOrDefaultAsync(p => p.Id == id);

                project.Title = projectUpdateViewModel.Title;
                project.CustomerCompany = projectUpdateViewModel.CustomerCompany;
                project.ExecutorCompany = projectUpdateViewModel.ExecutorCompany;
                project.StartTime = projectUpdateViewModel.StartTime;
                project.EndTime = projectUpdateViewModel.EndTime;
                project.Priority = projectUpdateViewModel.Priority;
                project.ManagerId = projectUpdateViewModel.ManagerId;

                var existingEmployeeIds = project.ProjectEmployees.Select(pe => pe.EmployeeId).ToList();

                var employeesToRemove = project.ProjectEmployees
                    .Where(pe => !projectUpdateViewModel.EmployeesId.Contains(pe.EmployeeId))
                    .ToList();

                foreach (var employeeToRemove in employeesToRemove)
                {
                    project.ProjectEmployees.Remove(employeeToRemove);
                }

                var newEmployeeIds = projectUpdateViewModel.EmployeesId.Except(existingEmployeeIds).ToList();
                foreach (var newEmployeeId in newEmployeeIds)
                {
                    project.ProjectEmployees.Add(new ProjectEmployee { ProjectId = project.Id, EmployeeId = newEmployeeId });
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", project.Id.ToString());
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var existingDocument in project.Documents.ToList())
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingDocument.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    _context.Documents.Remove(existingDocument);
                }
                foreach (var document in projectUpdateViewModel.Documents)
                {
                    if (document.Length > 0)
                    {
                        var filePath = Path.Combine(uploadsFolder, document.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await document.CopyToAsync(stream);
                        }

                        project.Documents.Add(new Document
                        {
                            FileName = document.FileName,
                            FilePath = $"/uploads/{project.Id}/{document.FileName}"
                        });
                    }
                }

                _context.Update(project);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = project.Id });
            }
            ViewBag.Manager = new SelectList(_userManager.Users, "Id", "FullName");
            ViewBag.Employees = await _userManager.Users.ToListAsync();
            return View(projectUpdateViewModel);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");

            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);

            bool hasAccess = isAdmin ||
                     project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            
            bool hasAccess = isAdmin ||
                     project.ManagerId == currentUser.Id;

            if (!hasAccess)
            {
                return Forbid();
            }
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
