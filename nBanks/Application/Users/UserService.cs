using Domain.Models.Users;
using System;
using Application.Users;

namespace nBanks.Application.Users
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
    
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user == null ? null: UserMapper.ToDTO(user);
        }

        public async Task<UserDTO?> GetUserByVATNumberAsync(string vatNumber)
        {
            var user = await _userRepository.GetUserByVATNumberAsync(vatNumber);
            return user == null ? null : UserMapper.ToDTO(user);
        }

        public async Task<UserDTO> AddUserAsync(UserDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO));
            }
            var dto = new UserDTO(userDTO.VATNumber);
            var user = UserMapper.ToDomain(userDTO);

            

            if (await _userRepository.GetUserByVATNumberAsync(user.VATNumber.ToString()) != null)
            {
                throw new InvalidOperationException("User with this VAT number already exists.");
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            try
            {
                await _userRepository.AddUserAsync(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding user to the database.", ex);
            }
            return UserMapper.ToDTO(user);
        }

        public async Task<UserDTO?> DeleteUserAsync(string vatNumber)
        {
            var user = await _userRepository.GetUserByVATNumberAsync(vatNumber);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            await _userRepository.DeleteUserAsync(user.Id);
            return UserMapper.ToDTO(user);
        }

    }
}
