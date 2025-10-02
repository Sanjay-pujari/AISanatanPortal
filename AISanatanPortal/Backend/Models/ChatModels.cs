using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

// Chat and AI-related Models
public class ChatSession : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public ChatSessionType Type { get; set; } = ChatSessionType.General;

    public ChatSessionStatus Status { get; set; } = ChatSessionStatus.Active;

    [MaxLength(1000)]
    public string? Context { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; } // Comma-separated tags

    public int MessageCount { get; set; } = 0;

    public DateTime? LastMessageAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public enum ChatSessionType
{
    General = 1,
    Spiritual_Guidance = 2,
    Scripture_Study = 3,
    Ritual_Help = 4,
    Philosophy = 5,
    Meditation = 6,
    Prayer = 7,
    Festival_Info = 8,
    Temple_Info = 9,
    Astrology = 10
}

public enum ChatSessionStatus
{
    Active = 1,
    Archived = 2,
    Deleted = 3
}

public class ChatMessage : BaseEntity
{
    [Required]
    public Guid SessionId { get; set; }

    [Required]
    public MessageRole Role { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public MessageType Type { get; set; } = MessageType.Text;

    [MaxLength(500)]
    public string? Metadata { get; set; } // JSON metadata

    public bool IsEdited { get; set; } = false;

    public DateTime? EditedAt { get; set; }

    public int? ParentMessageId { get; set; }

    public bool IsDeleted { get; set; } = false;

    // AI-specific fields
    [MaxLength(100)]
    public string? ModelUsed { get; set; }

    public int? TokensUsed { get; set; }

    public decimal? ConfidenceScore { get; set; }

    [MaxLength(1000)]
    public string? Sources { get; set; } // JSON array of source references

    // Navigation properties
    public ChatSession Session { get; set; } = null!;
}

public enum MessageRole
{
    User = 1,
    Assistant = 2,
    System = 3
}

public enum MessageType
{
    Text = 1,
    Image = 2,
    Audio = 3,
    File = 4,
    Link = 5,
    Quote = 6,
    Scripture = 7
}

// Assessment and Learning Models
public class Assessment : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public AssessmentType Type { get; set; }

    public AssessmentCategory Category { get; set; }

    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;

    public int TimeLimit { get; set; } = 0; // in minutes, 0 = no limit

    public int QuestionCount { get; set; } = 0;

    public int PassingScore { get; set; } = 70; // percentage

    public new bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = true;

    [MaxLength(1000)]
    public string? Instructions { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; } // Comma-separated tags

    public int AttemptCount { get; set; } = 0;

    public decimal AverageScore { get; set; } = 0;

    // Navigation properties
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<UserAssessmentResult> Results { get; set; } = new List<UserAssessmentResult>();
}

public enum AssessmentType
{
    Quiz = 1,
    Test = 2,
    Survey = 3,
    Evaluation = 4,
    Practice = 5,
    Certification = 6
}

public enum AssessmentCategory
{
    Vedas = 1,
    Puranas = 2,
    Upanishads = 3,
    Bhagavad_Gita = 4,
    Ramayana = 5,
    Mahabharata = 6,
    Dharma = 7,
    Philosophy = 8,
    Rituals = 9,
    Festivals = 10,
    Temples = 11,
    Mythology = 12,
    General = 13
}

public enum DifficultyLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}

public class Question : BaseEntity
{
    [Required]
    public Guid AssessmentId { get; set; }

    [Required]
    public string QuestionText { get; set; } = string.Empty;

    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

    [Required]
    public string Options { get; set; } = string.Empty; // JSON array of options

    [Required]
    public string CorrectAnswer { get; set; } = string.Empty; // JSON for correct answer(s)

    public int Points { get; set; } = 1;

    public int DisplayOrder { get; set; } = 0;

    [MaxLength(1000)]
    public string? Explanation { get; set; }

    [MaxLength(500)]
    public string? Hint { get; set; }

    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;

    [MaxLength(500)]
    public string? Tags { get; set; } // Comma-separated tags

    public string? ImageUrl { get; set; }

    public new bool IsActive { get; set; } = true;

    // Navigation properties
    public Assessment Assessment { get; set; } = null!;
}

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
    Essay = 4,
    FillInTheBlank = 5,
    Matching = 6,
    Ordering = 7,
    MultipleSelect = 8
}

public class UserAssessmentResult : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid AssessmentId { get; set; }

    public int Score { get; set; } = 0;

    public int MaxScore { get; set; } = 0;

    public decimal Percentage { get; set; } = 0;

    public bool IsPassed { get; set; } = false;

    public int TimeSpent { get; set; } = 0; // in seconds

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public AssessmentResultStatus Status { get; set; } = AssessmentResultStatus.InProgress;

    [Required]
    public string Answers { get; set; } = string.Empty; // JSON of user answers

    [MaxLength(2000)]
    public string? Feedback { get; set; }

    public int AttemptNumber { get; set; } = 1;

    // Navigation properties
    public User User { get; set; } = null!;
    public Assessment Assessment { get; set; } = null!;
}

public enum AssessmentResultStatus
{
    InProgress = 1,
    Completed = 2,
    Abandoned = 3,
    TimedOut = 4
}
