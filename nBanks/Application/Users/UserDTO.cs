using Domain.Models.Users;

namespace Application.Users
{
    public class UserDTO
    {
        public string VATNumber { get; set; }
        public string? Id { get; set; }

        public UserDTO() { }
        public UserDTO(string vatNumber, string? id = null)
        {
            if (string.IsNullOrWhiteSpace(vatNumber))
            {
                throw new ArgumentException("VAT number cannot be null or empty.", nameof(vatNumber));
            }
            VATNumber = vatNumber;
            Id = id;
        }
        
    }
}
