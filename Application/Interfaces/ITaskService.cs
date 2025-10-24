using MiniProjectManager.Application.DTOs.Task;

namespace MiniProjectManager.Application.Interfaces;
public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(string username, int projectId, TaskCreateDto dto);
    Task<TaskResponseDto> UpdateTaskAsync(string username, int taskId, TaskCreateDto dto);
    Task DeleteTaskAsync(string username, int taskId);
}
