using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Application.DTOs.Task;
using MiniProjectManager.Application.Interfaces;

namespace MiniProjectManager.Api.Controllers;
[Authorize]
[ApiController]
[Route("api")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _svc;
    public TasksController(ITaskService svc) => _svc = svc;
    private string Username => User.Identity?.Name ?? throw new Exception("No user");

    [HttpPost("projects/{projectId}/tasks")]
    public async Task<IActionResult> Create(int projectId, TaskCreateDto dto)
    {
        try
        {
            var t = await _svc.CreateTaskAsync(Username, projectId, dto);
            return Ok(t);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("tasks/{taskId}")]
    public async Task<IActionResult> Update(int taskId, TaskCreateDto dto)
    {
        try
        {
            var t = await _svc.UpdateTaskAsync(Username, taskId, dto);
            return Ok(t);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("tasks/{taskId}")]
    public async Task<IActionResult> Delete(int taskId)
    {
        try
        {
            await _svc.DeleteTaskAsync(Username, taskId);
            return NoContent();
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}
