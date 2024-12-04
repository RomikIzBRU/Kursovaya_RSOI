using Microsoft.AspNetCore.Mvc;
using OlsaMarketing.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;

namespace OlsaMarketing.Controllers
{
    public class HomeController : Controller
    {
        private readonly MarketingDepartment db;

        public HomeController(MarketingDepartment context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Employee()
        {
            return View(db.Employees.ToList());
        }


        public IActionResult Report()
        {
            var taskViews = db.Tasks
                .Include(t => t.Employee)
                .Include(t => t.Campaign)
                .Select(t => new TaskView
                {
                    Id = t.Id,
                    Description = t.Description,
                    CampaignId = t.CampaignId,
                    EmployeeId = t.EmployeeId,
                    DueTo = t.DueTo,
                    Employee = t.Employee,
                    Campaign = t.Campaign
                })
                .ToList();

            var report = new ReportView
            {
                Title = "Отчет о задачах",
                Tasks = taskViews
            };

            return View("Report", report);
        }
        [HttpGet]
        public IActionResult PrintReport(int reportId)
        {
            var reportData = db.Tasks
                .Include(t => t.Employee)
                .Include(t => t.Campaign)
                .ToList();

            var reportViewData = new List<TaskView>();

            foreach (var task in reportData)
            {
                TaskView tv = new TaskView
                {
                    Report = task.Report,
                    ReportId = task.ReportId,
                    Description = task.Description,
                    Campaign = task.Campaign,
                    Employee = task.Employee,
                    Id = task.Id,
                    EmployeeId = task.EmployeeId,
                    CampaignId = task.CampaignId,
                    DueTo = task.DueTo,
                    Photos = task.Photos.ToList() 
                };
                reportViewData.Add(tv);
            }

            var report = new ReportView
            {
                Title = "Отчет о задачах",
                Tasks = reportViewData,
            };

            return new ViewAsPdf("PrintReport", report)
            {
                FileName = "Report " + DateTime.Now.ToString("dd.MM.yyyy") + ".pdf"
            };
        }

        [HttpGet]
        public IActionResult Task(string? name)
        {
            var tasks = db.Tasks
                .Include(t => t.Employee)
                .Include(t => t.Campaign)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                tasks = tasks.Where(t => t.Employee.Name.Contains(name));
            }

            var sortedTasks = tasks
                .OrderBy(t => t.DueTo) 
                .ToList();

            return View(sortedTasks);
        }


        [HttpGet]
        public IActionResult AddTask()
        {
            PopulateDropdowns();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddTask(OlsaMarketing.Models.Task task)
        {
           
                await db.Tasks.AddAsync(task);
                await db.SaveChangesAsync();
                return RedirectToAction("Task");
            
        }

        [HttpGet]
        public IActionResult EditTask(int id)
        {
            var task = db.Tasks.Include(t => t.Employee).Include(t => t.Campaign).FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            PopulateDropdowns(task.EmployeeId, task.CampaignId);

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(OlsaMarketing.Models.Task task, List<IFormFile> TaskPhotos)
        { 
            var existingTask = db.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existingTask == null)
            {
                return NotFound();
            }
            existingTask.Description = task.Description;
            existingTask.CampaignId = task.CampaignId;
            existingTask.EmployeeId = task.EmployeeId;
            existingTask.DueTo = task.DueTo;
            if (TaskPhotos != null && TaskPhotos.Any())
            {
                foreach (var photo in TaskPhotos)
                {
                    if (photo.Length > 0)
                    {
                        var fileName = Path.GetFileName(photo.FileName);
                        var filePath = Path.Combine("wwwroot/uploads", fileName);
                        if (!Directory.Exists("wwwroot/uploads"))
                        {
                            Directory.CreateDirectory("wwwroot/uploads");
                        }
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        }
                        existingTask.Photos.Add($"/uploads/{fileName}");
                    }
                }
            }

            db.Tasks.Update(existingTask);
            await db.SaveChangesAsync();

            return RedirectToAction("Task");
        }

        private void PopulateDropdowns(int? selectedEmployeeId = null, int? selectedCampaignId = null)
        {
            var employees = db.Employees.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name", selectedEmployeeId);

            var campaigns = db.Campaigns.ToList();
            ViewBag.Campaigns = new SelectList(campaigns, "Id", "CampaignName", selectedCampaignId);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task != null)
            {
                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
                return RedirectToAction("Task");
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Back_Task()
        {
            return RedirectToAction("Task");
        }

        [HttpGet]
        public IActionResult Index(string Name)
        {
            List<Employee> list;

            if (string.IsNullOrEmpty(Name))
            {
                list = db.Employees.ToList();
            }
            else
            {
                list = db.Employees
                         .Where(p => p.Name.ToLower().Contains(Name.ToLower()))
                         .ToList();
            }

            var employeeView = list.Select(employee => new EmployeeView
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Telephone = employee.Telephone,
                Postion = employee.Postion
            }).ToList();
            return View(employeeView);
        }
        [HttpGet]
        public IActionResult Back()
        {
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult ChangeEmployee(int id)
        {
            var employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(new EmployeeView
            {
                Id = employee.Id,
                Name = employee.Name,
                Postion = employee.Postion,
                Email = employee.Email,
                Telephone = employee.Telephone
            });
        }
        [HttpPost]
        public IActionResult ChangeEmployee(EmployeeView employee_)
        {
            if (ModelState.IsValid)
            {
                var employee = db.Employees.Find(employee_.Id);
                if (employee != null)
                {
                    employee.Name = employee_.Name;
                    employee.Postion = employee_.Postion;
                    employee.Email = employee_.Email;
                    employee.Telephone = employee_.Telephone;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return View(employee_);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int? id)
        {
            if (id != null)
            {
                Employee user = await db.Employees.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    db.Employees.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Campaign(string? name)
        {
            var campaigns = db.Campaigns.AsQueryable(); 
            if (!string.IsNullOrWhiteSpace(name))
            {
                campaigns = campaigns.Where(c => c.CampaignName.Contains(name));
            }

            var sortedCampaigns = campaigns
                .OrderBy(c => c.EndDate) 
                .ToList();

            return View(sortedCampaigns);
        }

        public IActionResult AddCampaign()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCampaign(Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Campaign");
            }

            return View(campaign);
        }
        [HttpGet]
        public IActionResult EditCampaign(int id)
        {
            var campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return NotFound();
            }
            return View(campaign);
        }

        [HttpPost]
        public IActionResult EditCampaign(Campaign campaign)
        {
            
                db.Campaigns.Update(campaign);
                db.SaveChanges();
                return RedirectToAction("Campaign");
            
        }

        [HttpPost]
        public IActionResult DeleteCampaign(int id)
        {
            var campaign = db.Campaigns.FirstOrDefault(t => t.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            db.Campaigns.Remove(campaign);
            db.SaveChanges();
            return RedirectToAction("Campaign");
        }
        [HttpGet]
        public IActionResult Back_Campaign()
        {
            return RedirectToAction("Campaign");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}