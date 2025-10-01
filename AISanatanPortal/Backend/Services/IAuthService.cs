using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Models;

namespace AISanatanPortal.API.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string userId);
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}

public interface IAzureOpenAIService
{
    Task<string> GetChatCompletionAsync(string userMessage, string conversationHistory = "");
    Task<string> GetSpiritualGuidanceAsync(string question, string context = "");
    Task<string> ExplainConceptAsync(string concept, string difficulty = "beginner");
}

public interface IVedasService
{
    Task<IEnumerable<Veda>> GetAllVedasAsync();
    Task<Veda?> GetVedaByIdAsync(Guid id);
    Task<IEnumerable<VedaChapter>> GetVedaChaptersAsync(Guid vedaId);
    Task<VedaVerse?> GetVerseAsync(Guid chapterId, int verseNumber);
    Task<IEnumerable<VedaVerse>> SearchVersesAsync(string searchTerm);
}

public interface IPuranasService
{
    Task<IEnumerable<Purana>> GetAllPuranasAsync();
    Task<Purana?> GetPuranaByIdAsync(Guid id);
    Task<IEnumerable<PuranaStory>> GetPuranaStoriesAsync(Guid puranaId);
    Task<PuranaStory?> GetStoryByIdAsync(Guid storyId);
}

public interface ITemplesService
{
    Task<IEnumerable<Temple>> GetNearbyTemplesAsync(decimal latitude, decimal longitude, int radiusKm = 50);
    Task<Temple?> GetTempleByIdAsync(Guid id);
    Task<IEnumerable<Temple>> SearchTemplesAsync(string searchTerm, string? city = null, string? state = null);
    Task<IEnumerable<MythologicalPlace>> GetMythologicalPlacesAsync();
}

public interface IPanchangService
{
    Task<PanchangData?> GetPanchangForDateAsync(DateTime date);
    Task<IEnumerable<Festival>> GetUpcomingFestivalsAsync(int days = 30);
    Task<IEnumerable<Vrata>> GetVratasForMonthAsync(int month, int year);
    Task<Tithi> GetCurrentTithiAsync();
    Task<Nakshatra> GetCurrentNakshatraAsync();
}

public interface IBookstoreService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid id);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
}

public interface IEventsService
{
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<Event?> GetEventByIdAsync(Guid id);
    Task<EventRegistration> RegisterForEventAsync(Guid eventId, Guid userId);
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm, DateTime? fromDate = null);
}

public interface IGiftStoreService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<Vendor>> GetVendorsAsync();
}