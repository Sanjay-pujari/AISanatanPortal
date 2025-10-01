using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

// Vedas Models
public class Veda : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SanskritName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int ChapterCount { get; set; }

    public int VerseCount { get; set; }

    public string? AudioUrl { get; set; }

    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<VedaChapter> Chapters { get; set; } = new List<VedaChapter>();
}

public class VedaChapter : BaseEntity
{
    [Required]
    public Guid VedaId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritTitle { get; set; }

    public int ChapterNumber { get; set; }

    [MaxLength(2000)]
    public string? Summary { get; set; }

    public int VerseCount { get; set; }

    // Navigation properties
    public Veda Veda { get; set; } = null!;
    public ICollection<VedaVerse> Verses { get; set; } = new List<VedaVerse>();
}

public class VedaVerse : BaseEntity
{
    [Required]
    public Guid ChapterId { get; set; }

    public int VerseNumber { get; set; }

    [Required]
    public string SanskritText { get; set; } = string.Empty;

    [Required]
    public string EnglishTranslation { get; set; } = string.Empty;

    public string? HindiTranslation { get; set; }

    public string? Commentary { get; set; }

    public string? AudioUrl { get; set; }

    // Navigation properties
    public VedaChapter Chapter { get; set; } = null!;
}

// Puranas Models
public class Purana : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SanskritName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public PuranaType Type { get; set; }

    public int ChapterCount { get; set; }

    public int StoryCount { get; set; }

    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<PuranaChapter> Chapters { get; set; } = new List<PuranaChapter>();
    public ICollection<PuranaStory> Stories { get; set; } = new List<PuranaStory>();
}

public enum PuranaType
{
    Mahapurana = 1,
    Upapurana = 2
}

public class PuranaChapter : BaseEntity
{
    [Required]
    public Guid PuranaId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int ChapterNumber { get; set; }

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    // Navigation properties
    public Purana Purana { get; set; } = null!;
}

public class PuranaStory : BaseEntity
{
    [Required]
    public Guid PuranaId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? MoralLesson { get; set; }

    public string? Characters { get; set; }

    public string? ImageUrl { get; set; }

    // Navigation properties
    public Purana Purana { get; set; } = null!;
}

// Kavyas Models
public class Kavya : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SanskritName { get; set; }

    [Required]
    [MaxLength(200)]
    public string Author { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public KavyaType Type { get; set; }

    public int ChapterCount { get; set; }

    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<KavyaChapter> Chapters { get; set; } = new List<KavyaChapter>();
}

public enum KavyaType
{
    Epic = 1,
    Drama = 2,
    Poetry = 3,
    Other = 4
}

public class KavyaChapter : BaseEntity
{
    [Required]
    public Guid KavyaId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int ChapterNumber { get; set; }

    [MaxLength(2000)]
    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    // Navigation properties
    public Kavya Kavya { get; set; } = null!;
}