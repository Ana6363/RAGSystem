using Application.Users;
using Domain.Models.Users;

namespace nBanks.Application.Users
{
    public class UserMapper
    {
        public static UserDTO ToDTO(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            return new UserDTO(user.VATNumber.ToString());
        }

        public static User ToDomain(UserDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO), "UserDTO cannot be null.");
            }
            return new User(new VATNumber(userDTO.VATNumber));
        }

        public static UserDTO ToDTO(string vatNumber)
        {
            if (string.IsNullOrWhiteSpace(vatNumber))
            {
                throw new ArgumentException("VAT number cannot be null or empty.", nameof(vatNumber));
            }
            return new UserDTO(vatNumber);
        }

        public static User ToDomain(string vatNumber)
        {
            if (string.IsNullOrWhiteSpace(vatNumber))
            {
                throw new ArgumentException("VAT number cannot be null or empty.", nameof(vatNumber));
            }
            return new User(new VATNumber(vatNumber));
        }




    }
}
