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

        public class ScheduleResponse
        {
            public List<string> RecommendedOrder { get; set; } = new();
        }

        [HttpPost]
        public ActionResult<ScheduleResponse> ScheduleProjectTasks(Guid projectId, [FromBody] ScheduleRequest request)
        {
            // Validate input
            if (request?.Tasks == null || request.Tasks.Count == 0)
                return BadRequest("No tasks provided.");

            var taskMap = request.Tasks.ToDictionary(t => t.Title, t => t);
            var inDegree = new Dictionary<string, int>();
            var adj = new Dictionary<string, List<string>>();

            // Build adjacency list
            foreach (var task in request.Tasks)
            {
                if (!adj.ContainsKey(task.Title))
                    adj[task.Title] = new List<string>();

                if (!inDegree.ContainsKey(task.Title))
                    inDegree[task.Title] = 0;

                foreach (var dep in task.Dependencies)
                {
                    if (!adj.ContainsKey(dep))
                        adj[dep] = new List<string>();

                    adj[dep].Add(task.Title);

                    if (!inDegree.ContainsKey(task.Title))
                        inDegree[task.Title] = 0;

                    inDegree[task.Title]++;
                }
            }

            // Topological sort (Kahn’s Algorithm)
            var queue = new Queue<string>(inDegree.Where(x => x.Value == 0).Select(x => x.Key));
            var order = new List<string>();

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                order.Add(current);

                foreach (var next in adj[current])
                {
                    inDegree[next]--;
                    if (inDegree[next] == 0)
                        queue.Enqueue(next);
                }
            }

            if (order.Count != inDegree.Count)
                return BadRequest("Cycle detected in dependencies.");

            return Ok(new ScheduleResponse { RecommendedOrder = order });
        }
    }
}
