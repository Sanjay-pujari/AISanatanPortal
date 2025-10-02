using AISanatanPortal.API.DTOs;
using AISanatanPortal.API.Models;
using Models = AISanatanPortal.API.Models;

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
    Task<IEnumerable<Models.Veda>> GetAllVedasAsync();
    Task<Models.Veda?> GetVedaByIdAsync(Guid id);
    Task<IEnumerable<Models.VedaChapter>> GetVedaChaptersAsync(Guid vedaId);
    Task<Models.VedaVerse?> GetVerseAsync(Guid chapterId, int verseNumber);
    Task<IEnumerable<Models.VedaVerse>> SearchVersesAsync(string searchTerm);
}

public interface IPuranasService
{
    Task<IEnumerable<Models.Purana>> GetAllPuranasAsync();
    Task<Models.Purana?> GetPuranaByIdAsync(Guid id);
    Task<IEnumerable<Models.PuranaStory>> GetPuranaStoriesAsync(Guid puranaId);
    Task<Models.PuranaStory?> GetStoryByIdAsync(Guid storyId);
}

public interface ITemplesService
{
    Task<IEnumerable<Models.Temple>> GetNearbyTemplesAsync(decimal latitude, decimal longitude, int radiusKm = 50);
    Task<Models.Temple?> GetTempleByIdAsync(Guid id);
    Task<IEnumerable<Models.Temple>> SearchTemplesAsync(string searchTerm, string? city = null, string? state = null);
    Task<IEnumerable<Models.MythologicalPlace>> GetMythologicalPlacesAsync();
}

public interface IPanchangService
{
    Task<Models.PanchangData?> GetPanchangForDateAsync(DateTime date);
    Task<IEnumerable<Models.Festival>> GetUpcomingFestivalsAsync(int days = 30);
    Task<IEnumerable<Models.Vrata>> GetVratasForMonthAsync(int month, int year);
    Task<Models.Tithi> GetCurrentTithiAsync();
    Task<Models.Nakshatra> GetCurrentNakshatraAsync();
}

public interface IBookstoreService
{
    Task<IEnumerable<Models.Book>> GetAllBooksAsync();
    Task<Models.Book?> GetBookByIdAsync(Guid id);
    Task<IEnumerable<Models.Book>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<Models.Book>> GetBooksByCategoryAsync(string category);
    Task<Models.Order> CreateOrderAsync(Models.CreateOrderRequest request);
}

public interface IEventsService
{
    Task<IEnumerable<Models.Event>> GetUpcomingEventsAsync();
    Task<Models.Event?> GetEventByIdAsync(Guid id);
    Task<Models.EventRegistration> RegisterForEventAsync(Guid eventId, Guid userId);
    Task<IEnumerable<Models.Event>> SearchEventsAsync(string searchTerm, DateTime? fromDate = null);
}

public interface IGiftStoreService
{
    Task<IEnumerable<Models.Product>> GetAllProductsAsync();
    Task<Models.Product?> GetProductByIdAsync(Guid id);
    Task<IEnumerable<Models.Product>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<Models.Vendor>> GetVendorsAsync();
}