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
                SanskritName = "ऋग्वेदः",
                Description = "The oldest of the four Vedas, containing hymns and mantras dedicated to various deities. It is the foundation of Hindu philosophy and contains 10,552 mantras.",
                ChapterCount = 10,
                VerseCount = 10552,
                ImageUrl = "/images/vedas/rigveda.jpg"
            },
            new Veda
            {
                Name = "Samaveda",
                SanskritName = "सामवेदः",
                Description = "The Veda of melodies and chants. It contains 1,875 mantras, most of which are from the Rigveda, arranged for musical recitation.",
                ChapterCount = 2,
                VerseCount = 1875,
                ImageUrl = "/images/vedas/samaveda.jpg"
            },
            new Veda
            {
                Name = "Yajurveda",
                SanskritName = "यजुर्वेदः",
                Description = "The Veda of sacrificial formulas. It contains prose mantras for performing yajnas and rituals, divided into Shukla and Krishna Yajurveda.",
                ChapterCount = 40,
                VerseCount = 1975,
                ImageUrl = "/images/vedas/yajurveda.jpg"
            },
            new Veda
            {
                Name = "Atharvaveda",
                SanskritName = "अथर्ववेदः",
                Description = "The Veda of spells and charms. It contains 5,977 mantras dealing with medicine, magic, and daily life practices.",
                ChapterCount = 20,
                VerseCount = 5977,
                ImageUrl = "/images/vedas/atharvaveda.jpg"
            }
        };

        context.Vedas.AddRange(vedas);
        await context.SaveChangesAsync();

        // Add Veda Chapters
        await SeedVedaChapters(context, vedas);
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

    // Helper methods for comprehensive seed data
    private static async Task SeedVedaChapters(ApplicationDbContext context, List<Veda> vedas)
    {
        var chapters = new List<VedaChapter>();
        
        // Rigveda Chapters (10 Mandalas)
        var rigveda = vedas.First(v => v.Name == "Rigveda");
        for (int i = 1; i <= 10; i++)
        {
            chapters.Add(new VedaChapter
            {
                VedaId = rigveda.Id,
                Title = $"Mandala {i}",
                SanskritTitle = $"मण्डल {i}",
                ChapterNumber = i,
                Summary = $"The {i}th Mandala of Rigveda containing hymns dedicated to various deities and natural forces.",
                VerseCount = i == 1 ? 191 : i == 2 ? 43 : i == 3 ? 62 : i == 4 ? 58 : i == 5 ? 87 : i == 6 ? 75 : i == 7 ? 104 : i == 8 ? 103 : i == 9 ? 114 : 191
            });
        }

        // Samaveda Chapters (2 parts)
        var samaveda = vedas.First(v => v.Name == "Samaveda");
        chapters.Add(new VedaChapter
        {
            VedaId = samaveda.Id,
            Title = "Purvarchika",
            SanskritTitle = "पूर्वार्चिक",
            ChapterNumber = 1,
            Summary = "The first part of Samaveda containing 585 mantras arranged for musical recitation.",
            VerseCount = 585
        });
        chapters.Add(new VedaChapter
        {
            VedaId = samaveda.Id,
            Title = "Uttararchika",
            SanskritTitle = "उत्तरार्चिक",
            ChapterNumber = 2,
            Summary = "The second part of Samaveda containing 1,290 mantras for various rituals.",
            VerseCount = 1290
        });

        // Yajurveda Chapters (40 chapters for Shukla Yajurveda)
        var yajurveda = vedas.First(v => v.Name == "Yajurveda");
        for (int i = 1; i <= 40; i++)
        {
            chapters.Add(new VedaChapter
            {
                VedaId = yajurveda.Id,
                Title = $"Adhyaya {i}",
                SanskritTitle = $"अध्याय {i}",
                ChapterNumber = i,
                Summary = $"The {i}th chapter of Yajurveda containing sacrificial formulas and ritual procedures.",
                VerseCount = 50 + (i * 2) // Approximate verse count
            });
        }

        // Atharvaveda Chapters (20 Kandas)
        var atharvaveda = vedas.First(v => v.Name == "Atharvaveda");
        for (int i = 1; i <= 20; i++)
        {
            chapters.Add(new VedaChapter
            {
                VedaId = atharvaveda.Id,
                Title = $"Kanda {i}",
                SanskritTitle = $"काण्ड {i}",
                ChapterNumber = i,
                Summary = $"The {i}th Kanda of Atharvaveda containing spells, charms, and medicinal formulas.",
                VerseCount = 300 + (i * 5) // Approximate verse count
            });
        }

        context.VedaChapters.AddRange(chapters);
        await context.SaveChangesAsync();

        // Add sample verses for each chapter
        await SeedVedaVerses(context, chapters);
    }

    private static async Task SeedVedaVerses(ApplicationDbContext context, List<VedaChapter> chapters)
    {
        var verses = new List<VedaVerse>();
        
        foreach (var chapter in chapters)
        {
            // Add 3-5 sample verses per chapter
            int verseCount = Math.Min(5, chapter.VerseCount);
            for (int i = 1; i <= verseCount; i++)
            {
                verses.Add(new VedaVerse
                {
                    ChapterId = chapter.Id,
                    VerseNumber = i,
                    SanskritText = $"Sample Sanskrit text for verse {i} of {chapter.Title}",
                    EnglishTranslation = $"English translation of verse {i} from {chapter.Title}",
                    HindiTranslation = $"हिंदी अनुवाद of verse {i} from {chapter.Title}",
                    Commentary = $"Detailed commentary explaining the meaning and significance of verse {i}",
                    AudioUrl = $"/audio/vedas/{chapter.VedaId}/{chapter.Id}/verse_{i}.mp3"
                });
            }
        }

        context.VedaVerses.AddRange(verses);
        await context.SaveChangesAsync();
    }
}