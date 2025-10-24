using System.ComponentModel.DataAnnotations;

namespace MiniProjectManager.Application.DTOs.Task;
public class TaskCreateDto
{
    [Required, MinLength(1)]
    public string Title { get; set; } = null!;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; } = false;
}
