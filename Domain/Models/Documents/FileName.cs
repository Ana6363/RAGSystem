using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Documents
{
    [BsonNoId]
    public class FileName
    {

        [BsonElement("FileNameValue")]
        public string FileNameValue { get; set; }
        public FileName() { }
        public FileName(string fileNameValue)
        {
            if (string.IsNullOrWhiteSpace(fileNameValue))
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileNameValue));
            FileNameValue = fileNameValue;
        }
        public override string ToString() => FileNameValue;
       
    }
}
