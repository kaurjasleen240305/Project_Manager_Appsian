using MiniProjectManager.Application.DTOs.Task;

namespace MiniProjectManager.Application.DTOs.Project;
public class ProjectWithTasksDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<TaskResponseDto> Tasks { get; set; } = new List<TaskResponseDto>();
}
