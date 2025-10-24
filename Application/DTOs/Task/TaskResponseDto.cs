namespace MiniProjectManager.Application.DTOs.Task;
public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public int ProjectId { get; set; }
}
