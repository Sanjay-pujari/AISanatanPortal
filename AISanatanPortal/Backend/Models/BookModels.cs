using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

// Book-related Models
public class Book : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritTitle { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [MaxLength(50)]
    public string? ISBN { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public int PageCount { get; set; }

    public BookLanguage Language { get; set; } = BookLanguage.English;

    public BookFormat Format { get; set; } = BookFormat.Digital;

    [Range(0, 9999.99)]
    public decimal Price { get; set; }

    public bool IsFree { get; set; } = true;

    public string? CoverImageUrl { get; set; }

    public string? AudioUrl { get; set; }

    public DateTime? PublishedDate { get; set; }

    public decimal Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public int DownloadCount { get; set; } = 0;

    public bool IsFeatured { get; set; } = false;

    // Navigation properties
    public Author Author { get; set; } = null!;
    public BookCategory Category { get; set; } = null!;
    public ICollection<BookReview> Reviews { get; set; } = new List<BookReview>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum BookLanguage
{
    English = 1,
    Hindi = 2,
    Sanskrit = 3,
    Tamil = 4,
    Telugu = 5,
    Bengali = 6,
    Gujarati = 7,
    Marathi = 8,
    Kannada = 9,
    Malayalam = 10,
    Punjabi = 11,
    Urdu = 12
}

public enum BookFormat
{
    Digital = 1,
    Physical = 2,
    Audio = 3,
    Both = 4
}

public class Author : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritName { get; set; }

    [MaxLength(2000)]
    public string? Biography { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime? DeathDate { get; set; }

    [MaxLength(200)]
    public string? BirthPlace { get; set; }

    public string? ProfileImageUrl { get; set; }

    public AuthorType Type { get; set; } = AuthorType.Contemporary;

    public bool IsVerified { get; set; } = false;

    // Navigation properties
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public enum AuthorType
{
    Ancient = 1,
    Medieval = 2,
    Contemporary = 3,
    Modern = 4
}

public class BookCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SanskritName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public string? IconUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public BookCategory? ParentCategory { get; set; }
    public ICollection<BookCategory> SubCategories { get; set; } = new List<BookCategory>();
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

public class BookReview : BaseEntity
{
    [Required]
    public Guid BookId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }

    public bool IsVerified { get; set; } = false;

    public int HelpfulCount { get; set; } = 0;

    // Navigation properties
    public Book Book { get; set; } = null!;
    public User User { get; set; } = null!;
}
