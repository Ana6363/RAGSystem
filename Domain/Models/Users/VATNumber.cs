using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Users
{
    public class VATNumber
    {
        public string VATValue { get; set; }

        public VATNumber(string vatValue)
        {
            if (string.IsNullOrWhiteSpace(vatValue))
            {
                throw new ArgumentException("VAT number cannot be null or empty.", nameof(vatValue));
            }

            VATValue = vatValue;
        }

        public override string ToString() => VATValue;

    }
}
