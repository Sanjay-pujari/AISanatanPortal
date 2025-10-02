using System.ComponentModel.DataAnnotations;

namespace AISanatanPortal.API.Models;

// E-commerce Models
public class Product : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? SanskritName { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    public Guid VendorId { get; set; }

    [MaxLength(100)]
    public string? SKU { get; set; }

    [Range(0, 99999.99)]
    public decimal Price { get; set; }

    [Range(0, 99999.99)]
    public decimal? DiscountPrice { get; set; }

    public int StockQuantity { get; set; } = 0;

    public bool IsInStock { get; set; } = true;

    public ProductStatus Status { get; set; } = ProductStatus.Active;

    public string? PrimaryImageUrl { get; set; }

    public string? ImageUrls { get; set; } // JSON array of image URLs

    public decimal Weight { get; set; } = 0;

    [MaxLength(50)]
    public string? Dimensions { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    [MaxLength(100)]
    public string? Color { get; set; }

    public bool IsFeatured { get; set; } = false;

    public decimal Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public int SalesCount { get; set; } = 0;

    // Navigation properties
    public ProductCategory Category { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public enum ProductStatus
{
    Active = 1,
    Inactive = 2,
    OutOfStock = 3,
    Discontinued = 4
}

public class ProductCategory : BaseEntity
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
    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Vendor : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

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

    [MaxLength(2000)]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    public VendorStatus Status { get; set; } = VendorStatus.Pending;

    public decimal Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public DateTime? VerifiedAt { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public enum VendorStatus
{
    Pending = 1,
    Approved = 2,
    Suspended = 3,
    Rejected = 4
}

public class ProductReview : BaseEntity
{
    [Required]
    public Guid ProductId { get; set; }

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

    public string? ImageUrls { get; set; } // JSON array of review image URLs

    // Navigation properties
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class Order : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Range(0, 99999.99)]
    public decimal SubTotal { get; set; }

    [Range(0, 9999.99)]
    public decimal TaxAmount { get; set; }

    [Range(0, 9999.99)]
    public decimal ShippingAmount { get; set; }

    [Range(0, 9999.99)]
    public decimal DiscountAmount { get; set; }

    [Range(0, 99999.99)]
    public decimal TotalAmount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [MaxLength(100)]
    public string? PaymentTransactionId { get; set; }

    // Shipping Address
    [Required]
    [MaxLength(200)]
    public string ShippingName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ShippingCity { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ShippingState { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ShippingPostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ShippingCountry { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? ShippingPhone { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    [MaxLength(100)]
    public string? TrackingNumber { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Returned = 7,
    Refunded = 8
}

public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    NetBanking = 3,
    UPI = 4,
    Wallet = 5,
    COD = 6
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6
}

public class OrderItem : BaseEntity
{
    [Required]
    public Guid OrderId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? BookId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SKU { get; set; }

    [Range(0, 99999.99)]
    public decimal UnitPrice { get; set; }

    [Range(1, 9999)]
    public int Quantity { get; set; }

    [Range(0, 99999.99)]
    public decimal TotalPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product? Product { get; set; }
    public Book? Book { get; set; }
}

// DTOs for Order Creation
public class CreateOrderRequest
{
    public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    public ShippingAddressRequest ShippingAddress { get; set; } = new ShippingAddressRequest();
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

public class OrderItemRequest
{
    public Guid? ProductId { get; set; }
    public Guid? BookId { get; set; }
    public int Quantity { get; set; }
}

public class ShippingAddressRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
