using Domain.Models.Users;

namespace Application.Users
{
    public class UserDTO
    {
        public string VATNumber { get; set; }

        public UserDTO(string vatNumber)
        {
            if (string.IsNullOrWhiteSpace(vatNumber))
            {
                throw new ArgumentException("VAT number cannot be null or empty.", nameof(vatNumber));
            }
            VATNumber = vatNumber;
        }
    }
}
