using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    [BsonNoId]
    public class Content
    {
        [BsonElement("ContentValue")]
        public string ContentValue { get; set; }
        public Content() { }

        public Content(string contentValue)
        {
            if (string.IsNullOrWhiteSpace(contentValue))
                throw new ArgumentException("Content cannot be null or empty.", nameof(contentValue));
            ContentValue = contentValue;
        }

        public override string ToString() => ContentValue;

        public string ToJson()
        {
            return ContentValue;
        }
    }
}
