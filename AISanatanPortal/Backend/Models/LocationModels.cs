using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

public class Temple : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public decimal Latitude { get; set; }

    [Required]
    public decimal Longitude { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    public TempleType Type { get; set; }

    [MaxLength(200)]
    public string? MainDeity { get; set; }

    public string? Significance { get; set; }

    public string? History { get; set; }

    public string? VisitingHours { get; set; }

    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    [MaxLength(255)]
    public string? ContactEmail { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    public decimal Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public bool IsVerified { get; set; } = false;

    // Navigation properties
    public ICollection<PlaceImage> Images { get; set; } = new List<PlaceImage>();
}

public enum TempleType
{
    Ancient = 1,
    Medieval = 2,
    Modern = 3,
    Cave = 4,
    Rock_Cut = 5,
    Structural = 6
}

public class MythologicalPlace : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public decimal Latitude { get; set; }

    [Required]
    public decimal Longitude { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    public MythologicalPlaceType Type { get; set; }

    public string? MythologicalSignificance { get; set; }

    public string? RelatedTexts { get; set; }

    public string? AssociatedDeities { get; set; }

    public string? HistoricalEvidence { get; set; }

    public bool IsVerified { get; set; } = false;

    // Navigation properties
    public ICollection<PlaceImage> Images { get; set; } = new List<PlaceImage>();
}

public enum MythologicalPlaceType
{
    Birthplace = 1,
    Battlefield = 2,
    Kingdom = 3,
    Ashram = 4,
    Sacred_Grove = 5,
    Mountain = 6,
    River = 7,
    Lake = 8,
    Forest = 9,
    City = 10
}

public class PlaceImage : BaseEntity
{
    public Guid? TempleId { get; set; }

    public Guid? MythologicalPlaceId { get; set; }

    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Caption { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsPrimary { get; set; } = false;

    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public Temple? Temple { get; set; }
    public MythologicalPlace? MythologicalPlace { get; set; }
}