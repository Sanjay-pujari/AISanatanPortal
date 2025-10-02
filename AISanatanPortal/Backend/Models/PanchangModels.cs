using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

// Panchang-related Models
public class PanchangData : BaseEntity
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public Guid TithiId { get; set; }

    [Required]
    public Guid NakshatraId { get; set; }

    public Guid? YogaId { get; set; }

    public Guid? KaranaId { get; set; }

    [MaxLength(100)]
    public string? Vara { get; set; } // Day of the week

    [MaxLength(100)]
    public string? Masa { get; set; } // Month

    [MaxLength(100)]
    public string? Paksha { get; set; } // Fortnight

    [MaxLength(100)]
    public string? Ritu { get; set; } // Season

    [MaxLength(100)]
    public string? Samvatsara { get; set; } // Year

    [MaxLength(100)]
    public string? SunriseTime { get; set; }

    [MaxLength(100)]
    public string? SunsetTime { get; set; }

    [MaxLength(100)]
    public string? MoonriseTime { get; set; }

    [MaxLength(100)]
    public string? MoonsetTime { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    [MaxLength(1000)]
    public string? SpecialNotes { get; set; }

    // Navigation properties
    public Tithi Tithi { get; set; } = null!;
    public Nakshatra Nakshatra { get; set; } = null!;
    public ICollection<Festival> Festivals { get; set; } = new List<Festival>();
    public ICollection<Vrata> Vratas { get; set; } = new List<Vrata>();
}

public class Tithi : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SanskritName { get; set; }

    [Range(1, 30)]
    public int Number { get; set; }

    public TithiType Type { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? Significance { get; set; }

    [MaxLength(500)]
    public string? AuspiciousActivities { get; set; }

    [MaxLength(500)]
    public string? InauspiciousActivities { get; set; }

    public string? Deity { get; set; }

    // Navigation properties
    public ICollection<PanchangData> PanchangData { get; set; } = new List<PanchangData>();
}

public enum TithiType
{
    Shukla = 1, // Bright fortnight
    Krishna = 2 // Dark fortnight
}

public class Nakshatra : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SanskritName { get; set; }

    [Range(1, 27)]
    public int Number { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? Significance { get; set; }

    [MaxLength(100)]
    public string? RulingDeity { get; set; }

    [MaxLength(100)]
    public string? Symbol { get; set; }

    [MaxLength(100)]
    public string? Animal { get; set; }

    [MaxLength(100)]
    public string? Tree { get; set; }

    [MaxLength(100)]
    public string? Color { get; set; }

    [MaxLength(500)]
    public string? Characteristics { get; set; }

    [MaxLength(500)]
    public string? AuspiciousActivities { get; set; }

    [MaxLength(500)]
    public string? InauspiciousActivities { get; set; }

    // Navigation properties
    public ICollection<PanchangData> PanchangData { get; set; } = new List<PanchangData>();
}

public class Festival : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritName { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public FestivalType Type { get; set; }

    public FestivalCategory Category { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsNationalHoliday { get; set; } = false;

    public bool IsRegionalFestival { get; set; } = false;

    [MaxLength(500)]
    public string? Regions { get; set; } // Comma-separated regions

    [MaxLength(200)]
    public string? MainDeity { get; set; }

    [MaxLength(1000)]
    public string? Rituals { get; set; }

    [MaxLength(1000)]
    public string? Significance { get; set; }

    [MaxLength(1000)]
    public string? Traditions { get; set; }

    [MaxLength(1000)]
    public string? Foods { get; set; }

    public string? BannerImageUrl { get; set; }

    public string? ImageUrls { get; set; } // JSON array of image URLs

    [MaxLength(1000)]
    public string? Tags { get; set; } // Comma-separated tags

    public bool IsFeatured { get; set; } = false;

    // Navigation properties
    public ICollection<PanchangData> PanchangData { get; set; } = new List<PanchangData>();
}

public enum FestivalType
{
    Religious = 1,
    Cultural = 2,
    Seasonal = 3,
    Regional = 4,
    National = 5,
    Devotional = 6,
    Harvest = 7,
    Lunar = 8,
    Solar = 9,
    Traditional = 10
}

public enum FestivalCategory
{
    Major = 1,
    Minor = 2,
    Regional = 3,
    Sectarian = 4,
    Seasonal = 5,
    Devotional = 6,
    Cultural = 7,
    Historical = 8,
    Mythological = 9,
    Agricultural = 10
}

public class Vrata : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritName { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public VrataType Type { get; set; }

    public VrataDuration Duration { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public DateTime? EndDate { get; set; }

    [MaxLength(200)]
    public string? Deity { get; set; }

    [MaxLength(1000)]
    public string? Purpose { get; set; }

    [MaxLength(1000)]
    public string? Rules { get; set; }

    [MaxLength(1000)]
    public string? Rituals { get; set; }

    [MaxLength(1000)]
    public string? Benefits { get; set; }

    [MaxLength(1000)]
    public string? Precautions { get; set; }

    [MaxLength(500)]
    public string? FoodRestrictions { get; set; }

    [MaxLength(500)]
    public string? AllowedFoods { get; set; }

    public bool IsForMen { get; set; } = true;

    public bool IsForWomen { get; set; } = true;

    public bool IsForChildren { get; set; } = false;

    public string? ImageUrl { get; set; }

    [MaxLength(1000)]
    public string? Tags { get; set; } // Comma-separated tags

    // Navigation properties
    public ICollection<PanchangData> PanchangData { get; set; } = new List<PanchangData>();
}

public enum VrataType
{
    Fasting = 1,
    Partial_Fasting = 2,
    Devotional = 3,
    Ritual = 4,
    Meditation = 5,
    Charity = 6,
    Pilgrimage = 7,
    Prayer = 8,
    Study = 9,
    Service = 10
}

public enum VrataDuration
{
    One_Day = 1,
    Multiple_Days = 2,
    Weekly = 3,
    Monthly = 4,
    Seasonal = 5,
    Annual = 6,
    Lifetime = 7
}
