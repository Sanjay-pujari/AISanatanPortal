using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ProfilePicture { get; set; }

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsEmailVerified { get; set; } = false;

    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public UserPreference? Preference { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<BookReview> BookReviews { get; set; } = new List<BookReview>();
    public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public ICollection<UserAssessmentResult> AssessmentResults { get; set; } = new List<UserAssessmentResult>();
}

public enum UserRole
{
    User = 1,
    Author = 2,
    Vendor = 3,
    Admin = 4,
    SuperAdmin = 5
}

public class UserPreference : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [MaxLength(10)]
    public string Language { get; set; } = "en";

    [MaxLength(10)]
    public string Theme { get; set; } = "light";

    public bool NotificationsEnabled { get; set; } = true;

    public bool EmailUpdatesEnabled { get; set; } = true;

    public bool NewsletterSubscription { get; set; } = false;

    [MaxLength(100)]
    public string TimeZone { get; set; } = "UTC";

    // Navigation property
    public User User { get; set; } = null!;
}