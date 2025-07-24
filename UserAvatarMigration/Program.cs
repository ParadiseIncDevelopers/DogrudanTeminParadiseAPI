using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
}

public class AdminUser
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
}

public class UserAvatar
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserOrAdminId { get; set; }

    public int AvatarCode { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var conn = config["MongoAPI"];
        var dbName = config["MongoDBName"];

        var client = new MongoClient(conn);
        var db = client.GetDatabase(dbName);

        var userCol = db.GetCollection<User>("Users");
        var adminCol = db.GetCollection<AdminUser>("AdminUsers");
        var avatarCol = db.GetCollection<UserAvatar>("UserAvatars");

        var users = await userCol.Find(_ => true).ToListAsync();
        foreach (var u in users)
        {
            await UpsertAvatar(avatarCol, u.Id);
        }

        var admins = await adminCol.Find(_ => true).ToListAsync();
        foreach (var a in admins)
        {
            await UpsertAvatar(avatarCol, a.Id);
        }

        Console.WriteLine("Avatar migration completed.");
    }

    static async Task UpsertAvatar(IMongoCollection<UserAvatar> avatarCol, Guid id)
    {
        var filter = Builders<UserAvatar>.Filter.Eq(x => x.UserOrAdminId, id);
        var update = Builders<UserAvatar>.Update
            .Set(x => x.AvatarCode, 10)
            .SetOnInsert(x => x.Id, Guid.NewGuid())
            .SetOnInsert(x => x.UserOrAdminId, id);
        await avatarCol.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}
