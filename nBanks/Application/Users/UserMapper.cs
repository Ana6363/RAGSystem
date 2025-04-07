using Application.Users;
using Domain.Models.Users;
using MongoDB.Bson;


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
            return new UserDTO(user.VATNumber.ToString(), user.Id);

        }

        public static User ToDomain(UserDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO), "UserDTO cannot be null.");
            }
            var user = new User(new VATNumber(userDTO.VATNumber));
            user.Id = userDTO.Id ?? ObjectId.GenerateNewId().ToString();
            return user;
        }




    }
}
