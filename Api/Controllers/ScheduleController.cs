using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MiniProjectManager.Controllers
{
    [ApiController]
    [Route("api/v1/projects/{projectId}/[controller]")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        public class ScheduleTaskDto
        {
            public string Title { get; set; } = string.Empty;
            public int EstimatedHours { get; set; }
            public DateTime? DueDate { get; set; }
            public List<string> Dependencies { get; set; } = new();
        }

        public class ScheduleRequest
        {
            public List<ScheduleTaskDto> Tasks { get; set; } = new();
        }

        public class ScheduledTask
        {
            public string Title { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int EstimatedHours { get; set; }
        }

        public class ScheduleResponse
        {
            public List<string> RecommendedOrder { get; set; } = new();
            public List<ScheduledTask> ScheduledTasks { get; set; } = new();
            public bool IsSchedulable { get; set; } = true;
            public List<string> Warnings { get; set; } = new();
        }

        [HttpPost]
        public ActionResult<ScheduleResponse> ScheduleProjectTasks(int projectId, [FromBody] ScheduleRequest request)
        {
            // Validate input
            if (request?.Tasks == null || request.Tasks.Count == 0)
                return BadRequest("No tasks provided.");

            // Validate estimated hours for all tasks
            foreach (var task in request.Tasks)
            {
                if (task.EstimatedHours <= 0)
                    return BadRequest($"Task '{task.Title}' has invalid estimated hours: {task.EstimatedHours}. Must be greater than 0.");
                
                if (task.EstimatedHours > 10000) // Reasonable upper limit
                    return BadRequest($"Task '{task.Title}' has excessive estimated hours: {task.EstimatedHours}. Maximum allowed is 10000 hours.");
            }

            var taskMap = request.Tasks.ToDictionary(t => t.Title, t => t);
            var inDegree = new Dictionary<string, int>();
            var adj = new Dictionary<string, List<string>>();
            var warnings = new List<string>();

            // Initialize all tasks in inDegree
            foreach (var task in request.Tasks)
            {
                inDegree[task.Title] = 0;
                adj[task.Title] = new List<string>();
            }

            // Build adjacency list and calculate in-degrees
            foreach (var task in request.Tasks)
            {
                foreach (var dep in task.Dependencies)
                {
                    if (!taskMap.ContainsKey(dep))
                    {
                        warnings.Add($"Dependency '{dep}' for task '{task.Title}' not found in task list.");
                        continue;
                    }

                    if (!adj.ContainsKey(dep))
                        adj[dep] = new List<string>();

                    adj[dep].Add(task.Title);
                    inDegree[task.Title]++;
                }
            }

            // Topological sort with time scheduling
            var queue = new Queue<string>(inDegree.Where(x => x.Value == 0).Select(x => x.Key));
            var order = new List<string>();
            var scheduledTasks = new List<ScheduledTask>();
            var taskCompletionTimes = new Dictionary<string, DateTime>();
            var currentTime = DateTime.Today.AddHours(9); // Start at 9 AM today

            // Assume 8 working hours per day
            const int workingHoursPerDay = 8;

            // Check for overdue tasks and add warnings
            foreach (var task in request.Tasks)
            {
                if (task.DueDate.HasValue && task.DueDate.Value.Date < DateTime.Today)
                {
                    warnings.Add($"Task '{task.Title}' has a due date in the past: {task.DueDate.Value:yyyy-MM-dd}");
                }
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentTask = taskMap[current];
                order.Add(current);

                // Calculate start time (after all dependencies are complete)
                var startTime = currentTime;
                foreach (var dep in currentTask.Dependencies)
                {
                    if (taskCompletionTimes.ContainsKey(dep))
                    {
                        var depCompletionTime = taskCompletionTimes[dep];
                        // Ensure we don't have DateTime overflow when comparing
                        if (depCompletionTime > startTime)
                        {
                            startTime = depCompletionTime;
                        }
                    }
                }

                // Ensure start time is within valid DateTime range
                if (startTime > DateTime.MaxValue.AddDays(-365))
                {
                    warnings.Add($"Task '{current}' start time exceeds reasonable date limits.");
                    startTime = DateTime.MaxValue.Date.AddDays(-365);
                }

                // Calculate end time based on estimated hours
                var totalHours = currentTask.EstimatedHours;
                var endTime = startTime;
                
                // Ensure start time is within working hours (9 AM to 5 PM)
                if (endTime.Hour < 9)
                {
                    endTime = endTime.Date.AddHours(9); // Move to 9 AM
                }
                else if (endTime.Hour >= 17)
                {
                    // Move to next day if start time is after 5 PM
                    try
                    {
                        endTime = endTime.Date.AddDays(1).AddHours(9); // Start at 9 AM next day
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        warnings.Add($"Task '{current}' scheduling resulted in date overflow.");
                        endTime = DateTime.MaxValue.Date.AddDays(-1);
                    }
                }
                
                // Add hours while respecting working day limits (9 AM to 5 PM)
                var hoursToAdd = totalHours;
                var maxIterations = 1000; // Prevent infinite loops
                var iterations = 0;
                
                while (hoursToAdd > 0 && iterations < maxIterations)
                {
                    iterations++;
                    var currentHour = endTime.Hour;
                    
                    // If we're past working hours, move to next working day
                    if (currentHour >= 17)
                    {
                        try
                        {
                            endTime = endTime.Date.AddDays(1).AddHours(9); // Start at 9 AM next day
                            currentHour = 9;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            warnings.Add($"Task '{current}' scheduling resulted in date overflow.");
                            endTime = DateTime.MaxValue.Date.AddDays(-1);
                            break;
                        }
                        continue;
                    }
                    
                    // Calculate remaining hours in current working day (9 AM to 5 PM = 8 hours max)
                    var remainingHoursInDay = Math.Min(17 - currentHour, 8); // Never more than 8 hours per day
                    var hoursThisSession = Math.Min(hoursToAdd, remainingHoursInDay);
                    
                    try
                    {
                        endTime = endTime.AddHours(hoursThisSession);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        warnings.Add($"Task '{current}' scheduling resulted in date overflow.");
                        endTime = DateTime.MaxValue.Date.AddDays(-1);
                        break;
                    }
                    
                    hoursToAdd -= hoursThisSession;
                }
                
                if (iterations >= maxIterations)
                {
                    warnings.Add($"Task '{current}' scheduling exceeded maximum iterations. Task may require too many hours.");
                    // Calculate a simple end time based on estimated days needed
                    var daysNeeded = Math.Ceiling((double)totalHours / workingHoursPerDay);
                    try
                    {
                        endTime = startTime.AddDays(daysNeeded);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        endTime = DateTime.MaxValue.Date.AddDays(-1);
                        warnings.Add($"Task '{current}' requires too many days and exceeds date limits.");
                    }
                }

                // Check if task can be completed before due date
                if (currentTask.DueDate.HasValue && endTime > currentTask.DueDate.Value)
                {
                    warnings.Add($"Task '{current}' cannot be completed by due date {currentTask.DueDate.Value:yyyy-MM-dd}. " +
                               $"Estimated completion: {endTime:yyyy-MM-dd}");
                }

                taskCompletionTimes[current] = endTime;
                scheduledTasks.Add(new ScheduledTask
                {
                    Title = current,
                    StartDate = startTime,
                    EndDate = endTime,
                    EstimatedHours = currentTask.EstimatedHours
                });

                // Update current time for next available task, but ensure it doesn't overflow
                try
                {
                    currentTime = endTime;
                }
                catch (ArgumentOutOfRangeException)
                {
                    currentTime = DateTime.MaxValue.Date.AddDays(-1);
                    warnings.Add("Schedule calculation reached maximum date limit. Some tasks may not be properly scheduled.");
                }

                // Process dependent tasks
                foreach (var next in adj[current])
                {
                    inDegree[next]--;
                    if (inDegree[next] == 0)
                        queue.Enqueue(next);
                }
            }

            if (order.Count != inDegree.Count)
                return BadRequest("Cycle detected in dependencies.");

            // Sort scheduled tasks by start date for better visualization
            scheduledTasks = scheduledTasks.OrderBy(t => t.StartDate).ToList();

            return Ok(new ScheduleResponse 
            { 
                RecommendedOrder = order,
                ScheduledTasks = scheduledTasks,
                IsSchedulable = warnings.Count == 0,
                Warnings = warnings
            });
        }
    }
}
