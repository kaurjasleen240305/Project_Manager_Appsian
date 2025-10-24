using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Application.DTOs.Task;
using MiniProjectManager.Application.Interfaces;
using MiniProjectManager.Infrastructure.Data;

namespace MiniProjectManager.Application.Services;
public class TaskService : ITaskService
{
    private readonly AppDbContext _db;
    public TaskService(AppDbContext db) => _db = db;

    public async Task<TaskResponseDto> CreateTaskAsync(string username, int projectId, TaskCreateDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found");
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == user.Id) ?? throw new Exception("Project not found");

        var t = new Domain.Entities.TaskItem { Title = dto.Title, DueDate = dto.DueDate, IsCompleted = dto.IsCompleted, ProjectId = project.Id };
        _db.Tasks.Add(t);
        await _db.SaveChangesAsync();

        return new TaskResponseDto { Id = t.Id, Title = t.Title, DueDate = t.DueDate, IsCompleted = t.IsCompleted, ProjectId = t.ProjectId };
    }

    public async Task DeleteTaskAsync(string username, int taskId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found");
        var task = await _db.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == user.Id) ?? throw new Exception("Task not found");
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(string username, int taskId, TaskCreateDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username) ?? throw new Exception("User not found");
        var task = await _db.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == user.Id) ?? throw new Exception("Task not found");

        task.Title = dto.Title;
        task.DueDate = dto.DueDate;
        task.IsCompleted = dto.IsCompleted;
        await _db.SaveChangesAsync();

        return new TaskResponseDto { Id = task.Id, Title = task.Title, DueDate = task.DueDate, IsCompleted = task.IsCompleted, ProjectId = task.ProjectId };
    }
}
