namespace MiniProjectManager.Application.DTOs.Project;
public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
