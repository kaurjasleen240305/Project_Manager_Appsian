using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProjectManager.Application.DTOs.Project;
using MiniProjectManager.Application.Interfaces;

namespace MiniProjectManager.Api.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _svc;
    public ProjectsController(IProjectService svc) => _svc = svc;

    private string Username => User.Identity?.Name ?? throw new Exception("No user");

    [HttpGet]
    public async Task<IActionResult> GetProjects() => Ok(await _svc.GetUserProjectsAsync(Username));

    [HttpPost]
    public async Task<IActionResult> Create(ProjectCreateDto dto)
    {
        try
        {
            var p = await _svc.CreateProjectAsync(Username, dto);
            return Ok(p);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _svc.DeleteProjectAsync(Username, id);
            return NoContent();
        }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}
