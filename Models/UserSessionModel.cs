using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class UserSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [BsonElement("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("scopes")]
    public List<string> Scopes { get; set; } = [];

    [BsonElement("provider")]
    public string Provider { get; set; } = string.Empty;
}
