using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    public string? CreatedBy { get; set; }
    
    public string? UpdatedBy { get; set; }
}