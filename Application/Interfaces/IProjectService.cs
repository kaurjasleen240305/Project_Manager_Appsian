using MiniProjectManager.Application.DTOs.Project;

namespace MiniProjectManager.Application.Interfaces;
public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(string username);
    Task<ProjectWithTasksDto> GetProjectByIdAsync(string username, int projectId);
    Task<ProjectResponseDto> CreateProjectAsync(string username, ProjectCreateDto dto);
    Task DeleteProjectAsync(string username, int projectId);
}
