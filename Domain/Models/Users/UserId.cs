using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Users
{
    public class UserId
{
    public string Value { get; set; }

    public UserId()
    {
        Value = ObjectId.GenerateNewId().ToString();
    }
}

}
