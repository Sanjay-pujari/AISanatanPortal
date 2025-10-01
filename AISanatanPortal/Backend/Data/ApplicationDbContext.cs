using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User Management
    public DbSet<User> Users { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }

    // Content Tables
    public DbSet<Veda> Vedas { get; set; }
    public DbSet<VedaChapter> VedaChapters { get; set; }
    public DbSet<VedaVerse> VedaVerses { get; set; }
    
    public DbSet<Purana> Puranas { get; set; }
    public DbSet<PuranaChapter> PuranaChapters { get; set; }
    public DbSet<PuranaStory> PuranaStories { get; set; }
    
    public DbSet<Kavya> Kavyas { get; set; }
    public DbSet<KavyaChapter> KavyaChapters { get; set; }

    // Places and Temples
    public DbSet<Temple> Temples { get; set; }
    public DbSet<MythologicalPlace> MythologicalPlaces { get; set; }
    public DbSet<PlaceImage> PlaceImages { get; set; }

    // Panchang and Calendar
    public DbSet<PanchangData> PanchangData { get; set; }
    public DbSet<Festival> Festivals { get; set; }
    public DbSet<Vrata> Vratas { get; set; }
    public DbSet<Nakshatra> Nakshatras { get; set; }
    public DbSet<Tithi> Tithis { get; set; }

    // Bookstore
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<BookReview> BookReviews { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    // Gift Store
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    // Events
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }

    // Chat and AI
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    // Evaluation and Assessment
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<UserAssessmentResult> UserAssessmentResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity relationships and constraints
        ConfigureUserEntities(modelBuilder);
        ConfigureContentEntities(modelBuilder);
        ConfigurePlaceEntities(modelBuilder);
        ConfigurePanchangEntities(modelBuilder);
        ConfigureBookstoreEntities(modelBuilder);
        ConfigureGiftStoreEntities(modelBuilder);
        ConfigureEventEntities(modelBuilder);
        ConfigureChatEntities(modelBuilder);
        ConfigureAssessmentEntities(modelBuilder);

        // Configure indexes for performance
        ConfigureIndexes(modelBuilder);
    }

    private void ConfigureUserEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            
            entity.HasOne(e => e.Preference)
                  .WithOne(p => p.User)
                  .HasForeignKey<UserPreference>(p => p.UserId);
        });
    }

    private void ConfigureContentEntities(ModelBuilder modelBuilder)
    {
        // Vedas configuration
        modelBuilder.Entity<Veda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SanskritName).HasMaxLength(200);
        });

        modelBuilder.Entity<VedaChapter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Veda)
                  .WithMany(v => v.Chapters)
                  .HasForeignKey(e => e.VedaId);
        });

        modelBuilder.Entity<VedaVerse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Chapter)
                  .WithMany(c => c.Verses)
                  .HasForeignKey(e => e.ChapterId);
        });

        // Puranas configuration
        modelBuilder.Entity<Purana>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // Kavyas configuration
        modelBuilder.Entity<Kavya>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }

    private void ConfigurePlaceEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Temple>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);
        });

        modelBuilder.Entity<MythologicalPlace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Latitude).HasPrecision(10, 7);
            entity.Property(e => e.Longitude).HasPrecision(10, 7);
        });
    }

    private void ConfigurePanchangEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PanchangData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.HasIndex(e => e.Date).IsUnique();
        });

        modelBuilder.Entity<Festival>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });
    }

    private void ConfigureBookstoreEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            
            entity.HasOne(e => e.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(e => e.AuthorId);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(12, 2);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(e => e.UserId);
        });
    }

    private void ConfigureGiftStoreEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            
            entity.HasOne(e => e.Vendor)
                  .WithMany(v => v.Products)
                  .HasForeignKey(e => e.VendorId);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessName).IsRequired().HasMaxLength(200);
        });
    }

    private void ConfigureEventEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RegistrationFee).HasPrecision(10, 2);
        });
    }

    private void ConfigureChatEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.ChatSessions)
                  .HasForeignKey(e => e.UserId)
                  .IsRequired(false);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Session)
                  .WithMany(s => s.Messages)
                  .HasForeignKey(e => e.SessionId);
        });
    }

    private void ConfigureAssessmentEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Assessment)
                  .WithMany(a => a.Questions)
                  .HasForeignKey(e => e.AssessmentId);
        });
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Performance indexes
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Title);
        
        modelBuilder.Entity<Temple>()
            .HasIndex(t => new { t.Latitude, t.Longitude });
        
        modelBuilder.Entity<Event>()
            .HasIndex(e => e.StartDate);
        
        modelBuilder.Entity<PanchangData>()
            .HasIndex(p => p.Date);
    }
}