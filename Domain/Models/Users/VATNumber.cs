using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Users
{
    [BsonNoId]
    public class VATNumber
    {
        [BsonElement("VATValue")]
        public string VATValue { get; set; }

        public VATNumber() {}

        public VATNumber(string vatValue)
        {
            if (string.IsNullOrWhiteSpace(vatValue))
                throw new ArgumentException("VAT number cannot be null or empty.", nameof(vatValue));

            VATValue = vatValue;
        }

        public override string ToString() => VATValue;
    }
}
