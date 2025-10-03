using Microsoft.EntityFrameworkCore;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Create database if it doesn't exist
        await context.Database.EnsureCreatedAsync();

        // Check if already seeded
        if (context.Users.Any())
        {
            return; // Database has been seeded
        }

        // Seed Users
        await SeedUsers(context);
        
        // Seed Content
        await SeedVedas(context);
        await SeedPuranas(context);
        await SeedKavyas(context);
        
        // Seed Places
        await SeedTemples(context);
        await SeedMythologicalPlaces(context);
        
        // Seed Calendar Data
        await SeedPanchangData(context);
        await SeedFestivals(context);
        
        // Seed Books and Authors
        await SeedBooks(context);
        
        // Seed Events
        await SeedEvents(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@aisanatanportal.com",
            FirstName = "System",
            LastName = "Administrator",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"),
            Role = UserRole.SuperAdmin,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
            Role = UserRole.User,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, testUser);

        // Add user preferences
        var adminPreferences = new UserPreference
        {
            UserId = adminUser.Id,
            Language = "en",
            Theme = "light",
            NotificationsEnabled = true,
            EmailUpdatesEnabled = true,
            TimeZone = "UTC"
        };

        var testUserPreferences = new UserPreference
        {
            UserId = testUser.Id,
            Language = "en",
            Theme = "light",
            NotificationsEnabled = true,
            EmailUpdatesEnabled = false,
            TimeZone = "Asia/Kolkata"
        };

        context.UserPreferences.AddRange(adminPreferences, testUserPreferences);
    }

    private static async Task SeedVedas(ApplicationDbContext context)
    {
        var vedas = new List<Veda>
        {
            new Veda
            {
                Name = "Rigveda",
                SanskritName = "ऋग्वेद",
                Description = "The Rigveda is one of the four sacred texts of Hinduism, known as the Vedas. It is the oldest of the four Vedas.",
                ChapterCount = 10,
                VerseCount = 1028
            },
            new Veda
            {
                Name = "Yajurveda",
                SanskritName = "यजुर्वेद",
                Description = "The Yajurveda is the Veda of sacrificial prayers and mantras. It is used by priests during sacrificial ceremonies.",
                ChapterCount = 40,
                VerseCount = 1975
            },
            new Veda
            {
                Name = "Samaveda",
                SanskritName = "सामवेद",
                Description = "The Samaveda is the Veda of melodies and chants. It consists of hymns taken from the Rigveda set to music.",
                ChapterCount = 21,
                VerseCount = 1549
            },
            new Veda
            {
                Name = "Atharvaveda",
                SanskritName = "अथर्ववेद",
                Description = "The Atharvaveda is the Veda of everyday life and contains spells, charms, and incantations.",
                ChapterCount = 20,
                VerseCount = 5977
            }
        };

        context.Vedas.AddRange(vedas);
    }

    private static async Task SeedPuranas(ApplicationDbContext context)
    {
        var puranas = new List<Purana>
        {
            new Purana
            {
                Name = "Vishnu Purana",
                SanskritName = "विष्णु पुराण",
                Description = "The Vishnu Purana is one of the eighteen Mahapuranas dedicated to Lord Vishnu.",
                Type = PuranaType.Mahapurana,
                ChapterCount = 6,
                StoryCount = 126
            },
            new Purana
            {
                Name = "Shiva Purana",
                SanskritName = "शिव पुराण",
                Description = "The Shiva Purana is one of the eighteen Mahapuranas dedicated to Lord Shiva.",
                Type = PuranaType.Mahapurana,
                ChapterCount = 7,
                StoryCount = 100
            },
            new Purana
            {
                Name = "Bhagavata Purana",
                SanskritName = "भागवत पुराण",
                Description = "The Bhagavata Purana is one of the most important Puranas, focusing on the devotion to Krishna.",
                Type = PuranaType.Mahapurana,
                ChapterCount = 12,
                StoryCount = 335
            }
        };

        context.Puranas.AddRange(puranas);
    }

    private static async Task SeedKavyas(ApplicationDbContext context)
    {
        var kavyas = new List<Kavya>
        {
            new Kavya
            {
                Name = "Ramayana",
                SanskritName = "रामायण",
                Author = "Maharshi Valmiki",
                Description = "The epic tale of Lord Rama's journey and his victory over the demon king Ravana.",
                Type = KavyaType.Epic,
                ChapterCount = 7
            },
            new Kavya
            {
                Name = "Mahabharata",
                SanskritName = "महाभारत",
                Author = "Maharshi Vyasa",
                Description = "The great epic of the Bharata dynasty and the Kurukshetra War.",
                Type = KavyaType.Epic,
                ChapterCount = 18
            }
        };

        context.Kavyas.AddRange(kavyas);
    }

    private static async Task SeedTemples(ApplicationDbContext context)
    {
        var temples = new List<Temple>
        {
            new Temple
            {
                Name = "Vaishno Devi Temple",
                Description = "A famous Hindu temple dedicated to Mata Vaishno Devi",
                Latitude = 33.0204m,
                Longitude = 74.9519m,
                City = "Katra",
                State = "Jammu and Kashmir",
                Country = "India",
                Type = TempleType.Ancient,
                MainDeity = "Mata Vaishno Devi",
                Significance = "One of the most visited pilgrimage sites in India",
                IsVerified = true,
                Rating = 4.8m,
                ReviewCount = 15000
            },
            new Temple
            {
                Name = "Tirupati Balaji Temple",
                Description = "Famous temple of Lord Venkateswara in Andhra Pradesh",
                Latitude = 13.6833m,
                Longitude = 79.3500m,
                City = "Tirupati",
                State = "Andhra Pradesh",
                Country = "India",
                Type = TempleType.Ancient,
                MainDeity = "Lord Venkateswara",
                Significance = "Richest temple in the world",
                IsVerified = true,
                Rating = 4.9m,
                ReviewCount = 25000
            }
        };

        context.Temples.AddRange(temples);
    }

    private static async Task SeedMythologicalPlaces(ApplicationDbContext context)
    {
        var places = new List<MythologicalPlace>
        {
            new MythologicalPlace
            {
                Name = "Ayodhya",
                Description = "The birthplace of Lord Rama",
                Latitude = 26.7922m,
                Longitude = 82.1998m,
                City = "Ayodhya",
                State = "Uttar Pradesh",
                Country = "India",
                Type = MythologicalPlaceType.Birthplace,
                MythologicalSignificance = "Birthplace of Lord Rama, capital of Kosala Kingdom",
                RelatedTexts = "Ramayana, Puranas",
                AssociatedDeities = "Lord Rama, Sita, Lakshmana",
                IsVerified = true
            },
            new MythologicalPlace
            {
                Name = "Kurukshetra",
                Description = "The battlefield where the Mahabharata war was fought",
                Latitude = 29.9667m,
                Longitude = 76.8833m,
                City = "Kurukshetra",
                State = "Haryana",
                Country = "India",
                Type = MythologicalPlaceType.Battlefield,
                MythologicalSignificance = "Site of the great Kurukshetra War",
                RelatedTexts = "Mahabharata, Bhagavad Gita",
                AssociatedDeities = "Lord Krishna, Arjuna",
                IsVerified = true
            }
        };

        context.MythologicalPlaces.AddRange(places);
    }

    private static async Task SeedPanchangData(ApplicationDbContext context)
    {
        // Add sample Panchang data for current date
        var today = DateTime.Today;
        // Note: PanchangData requires TithiId and NakshatraId references to existing Tithi and Nakshatra records
        // For now, we'll skip seeding PanchangData as it requires proper Tithi and Nakshatra setup
        // var panchangData = new AISanatanPortal.API.Models.PanchangData
        // {
        //     Id = Guid.NewGuid(),
        //     Date = today,
        //     TithiId = Guid.NewGuid(), // Would need to reference actual Tithi record
        //     NakshatraId = Guid.NewGuid() // Would need to reference actual Nakshatra record
        // };

        // Note: In a real implementation, you would add proper Panchang data
        // context.PanchangData.Add(panchangData);
    }

    private static async Task SeedFestivals(ApplicationDbContext context)
    {
        var festivals = new List<AISanatanPortal.API.Models.Festival>
        {
            new AISanatanPortal.API.Models.Festival
            {
                Name = "Diwali",
                Date = new DateTime(2024, 11, 1),
                Description = "Festival of lights celebrating the victory of light over darkness",
                Content = "Diwali is one of the most important festivals in Hinduism...",
                Type = AISanatanPortal.API.Models.FestivalType.Religious,
                Category = AISanatanPortal.API.Models.FestivalCategory.Major
            },
            new AISanatanPortal.API.Models.Festival
            {
                Name = "Holi",
                Date = new DateTime(2024, 3, 25),
                Description = "Festival of colors celebrating the arrival of spring",
                Content = "Holi is a vibrant festival celebrating the arrival of spring...",
                Type = AISanatanPortal.API.Models.FestivalType.Cultural,
                Category = AISanatanPortal.API.Models.FestivalCategory.Major
            }
        };

        // context.Festivals.AddRange(festivals);
    }

    private static async Task SeedBooks(ApplicationDbContext context)
    {
        var authors = new List<AISanatanPortal.API.Models.Author>
        {
            new AISanatanPortal.API.Models.Author
            {
                Name = "Swami Vivekananda",
                Biography = "Renowned spiritual leader and philosopher"
            },
            new AISanatanPortal.API.Models.Author
            {
                Name = "Paramahansa Yogananda",
                Biography = "Indian monk and guru who introduced millions to meditation and Kriya Yoga"
            }
        };

        // context.Authors.AddRange(authors);
    }

    private static async Task SeedEvents(ApplicationDbContext context)
    {
        var events = new List<AISanatanPortal.API.Models.Event>
        {
            new AISanatanPortal.API.Models.Event
            {
                Title = "Spiritual Discourse on Bhagavad Gita",
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(7).AddHours(2),
                Description = "Weekly discourse on the teachings of Bhagavad Gita",
                Content = "Join us for an enlightening discourse on the timeless wisdom of Bhagavad Gita...",
                VenueName = "Community Hall",
                Address = "Community Hall, Delhi",
                City = "Delhi",
                Type = AISanatanPortal.API.Models.EventType.Lecture,
                Category = AISanatanPortal.API.Models.EventCategory.Religious
            },
            new AISanatanPortal.API.Models.Event
            {
                Title = "Vedic Mathematics Workshop",
                StartDate = DateTime.Now.AddDays(14),
                EndDate = DateTime.Now.AddDays(14).AddHours(4),
                Description = "Learn the ancient techniques of Vedic Mathematics",
                Content = "Discover the fascinating world of Vedic Mathematics and its practical applications...",
                VenueName = "Educational Center",
                Address = "Educational Center, Mumbai",
                City = "Mumbai",
                Type = AISanatanPortal.API.Models.EventType.Workshop,
                Category = AISanatanPortal.API.Models.EventCategory.Educational
            }
        };

        // context.Events.AddRange(events);
    }
}