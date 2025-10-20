namespace Prompt2Ads.Repositories.OAuth2;

public interface IUserSessionRepository
{
    Task<List<UserSession>> GetAllAsync();
    Task<UserSession?> GetBySessionIdAsync(string sessionId);
    Task CreateAsync(UserSession userSession);
}