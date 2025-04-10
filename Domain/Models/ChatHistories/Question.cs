using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ChatHistories
{
    [BsonNoId]
    public class Question
    {
        [BsonElement("QuestionValue")]
        public string QuestionValue { get; set; }

        public Question() { }

        public Question(string questionValue)
        {
            if (string.IsNullOrWhiteSpace(questionValue))
                throw new ArgumentException("Question cannot be null or empty.", nameof(questionValue));
            QuestionValue = questionValue;
        }

        public override string ToString() => QuestionValue;
    }
}
