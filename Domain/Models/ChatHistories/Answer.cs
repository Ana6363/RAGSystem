using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.ChatHistories
{
    [BsonNoId]
    public class Answer
    {
        [BsonElement("AnswerValue")]
        public string AnswerValue { get; set; }
        public Answer() { }
        public Answer(string answerValue)
        {
            if (string.IsNullOrWhiteSpace(answerValue))
                throw new ArgumentException("Answer cannot be null or empty.", nameof(answerValue));
            AnswerValue = answerValue;
        }
        public override string ToString() => AnswerValue;
        public string ToJson()
        {
            return AnswerValue;
        }
    }
}
