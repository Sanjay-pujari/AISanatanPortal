using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

// Event-related Models
public class Event : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritTitle { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public EventType Type { get; set; }

    public EventCategory Category { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(100)]
    public string? StartTime { get; set; }

    [MaxLength(100)]
    public string? EndTime { get; set; }

    public bool IsAllDay { get; set; } = false;

    public bool IsRecurring { get; set; } = false;

    public RecurrencePattern? RecurrencePattern { get; set; }

    // Location details
    [MaxLength(200)]
    public string? VenueName { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsOnline { get; set; } = false;

    [MaxLength(500)]
    public string? OnlineUrl { get; set; }

    // Registration details
    public bool RequiresRegistration { get; set; } = false;

    public int? MaxAttendees { get; set; }

    public int CurrentAttendees { get; set; } = 0;

    [Range(0, 99999.99)]
    public decimal? RegistrationFee { get; set; }

    public DateTime? RegistrationDeadline { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Upcoming;

    public string? BannerImageUrl { get; set; }

    public string? ImageUrls { get; set; } // JSON array of image URLs

    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    [MaxLength(255)]
    public string? ContactEmail { get; set; }

    public bool IsFeatured { get; set; } = false;

    [MaxLength(1000)]
    public string? Tags { get; set; } // Comma-separated tags

    // Navigation properties
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}

public enum EventType
{
    Festival = 1,
    Ceremony = 2,
    Workshop = 3,
    Lecture = 4,
    Pilgrimage = 5,
    Celebration = 6,
    Prayer = 7,
    Meditation = 8,
    Cultural = 9,
    Educational = 10
}

public enum EventCategory
{
    Religious = 1,
    Cultural = 2,
    Educational = 3,
    Spiritual = 4,
    Community = 5,
    Charitable = 6,
    Youth = 7,
    Family = 8,
    Devotional = 9,
    Traditional = 10
}

public enum RecurrencePattern
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4,
    Custom = 5
}

public enum EventStatus
{
    Draft = 1,
    Upcoming = 2,
    Ongoing = 3,
    Completed = 4,
    Cancelled = 5,
    Postponed = 6
}

public class EventRegistration : BaseEntity
{
    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Registered;

    [Required]
    [MaxLength(200)]
    public string AttendeeFirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string AttendeeLastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string AttendeeEmail { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? AttendeePhone { get; set; }

    public int NumberOfAttendees { get; set; } = 1;

    [Range(0, 99999.99)]
    public decimal? AmountPaid { get; set; }

    [MaxLength(100)]
    public string? PaymentTransactionId { get; set; }

    public PaymentStatus? PaymentStatus { get; set; }

    [MaxLength(1000)]
    public string? SpecialRequests { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public bool IsAttended { get; set; } = false;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    public Event Event { get; set; } = null!;
    public User User { get; set; } = null!;
}

public enum RegistrationStatus
{
    Registered = 1,
    Confirmed = 2,
    Waitlisted = 3,
    Cancelled = 4,
    CheckedIn = 5,
    NoShow = 6
}
