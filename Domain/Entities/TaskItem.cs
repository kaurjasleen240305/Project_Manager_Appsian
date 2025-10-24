namespace MiniProjectManager.Domain.Entities;
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
