using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Users
{
    public class User
    {
        public UserId UserId { get; set; }
        public VATNumber VATNumber { get; set; }

        public User()
        {
        }

        public User(VATNumber vatNumber)
        {
            UserId = new UserId();
            VATNumber = vatNumber ?? throw new ArgumentNullException(nameof(vatNumber));
        }
       

    }
}
