using MongoDB.Driver;

namespace Prompt2Ads.Repositories.OAuth2;

public class UserSessionRepository: IUserSessionRepository
{
    private readonly IMongoCollection<UserSession> _userSessions;

    public UserSessionRepository(IMongoDatabase database)
    {
        _userSessions = database.GetCollection<UserSession>("userSessions");
    }

    public async Task<List<UserSession>> GetAllAsync() => await _userSessions.Find(_ => true).ToListAsync();

    public async Task<UserSession?> GetBySessionIdAsync(string sessionId) =>
        await _userSessions.Find(u => u.SessionId == sessionId).FirstOrDefaultAsync();
    
    public async Task CreateAsync(UserSession userSession) => await _userSessions.InsertOneAsync(userSession);
}
