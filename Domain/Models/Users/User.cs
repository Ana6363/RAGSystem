using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Users
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public VATNumber VATNumber { get; set; }

        public User() {}

        public User(VATNumber vatNumber)
        {
            Id = ObjectId.GenerateNewId().ToString();
            VATNumber = vatNumber;
        }
    }
}
