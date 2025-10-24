using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Application.DTOs.Project;
using MiniProjectManager.Application.Interfaces;
using MiniProjectManager.Infrastructure.Data;

namespace MiniProjectManager.Application.Services;
public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;
    public ProjectService(AppDbContext db) => _db = db;

    public async Task<ProjectResponseDto> CreateProjectAsync(string username, ProjectCreateDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found");
        var p = new Domain.Entities.Project { Title = dto.Title, Description = dto.Description, UserId = user.Id };
        _db.Projects.Add(p);
        await _db.SaveChangesAsync();
        return new ProjectResponseDto { Id = p.Id, Title = p.Title, Description = p.Description, CreatedAt = p.CreatedAt };
    }

    public async Task DeleteProjectAsync(string username, int projectId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found");
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == user.Id) ?? throw new Exception("Project not found");
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(string username)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found");
        return await _db.Projects.Where(p => p.UserId == user.Id)
            .Select(p => new ProjectResponseDto { Id = p.Id, Title = p.Title, Description = p.Description, CreatedAt = p.CreatedAt })
            .ToListAsync();
    }
}
