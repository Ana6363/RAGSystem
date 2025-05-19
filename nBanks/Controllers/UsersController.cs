using Infrastructure.Mongo;
using Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using nBanks.Application.Users;
using Application.Users;


namespace nBanks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserService _userService;



        public UsersController(UserService context)
        {
            _userService = context;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(UserDTO userDTO)
        {
            try
            {
                var res = await _userService.AddUserAsync(userDTO);

                if (res == null)
                {
                    return BadRequest(new { message = "Could not create user." });
                }

                return Ok(res);
            }
            catch (InvalidOperationException ex)
            {
                // Handle known business logic error: duplicate VAT
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Let other errors bubble as 500
                return StatusCode(500, new { message = "Unexpected error.", detail = ex.Message });
            }
        }


        [HttpGet("vat")]
        public async Task<IActionResult> GetByVatNumberAsync(string vat)
        {
            var res = await _userService.GetUserByVATNumberAsync(vat);

            if (res == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(res);
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(string vatNumber)
        {
            var res = await _userService.DeleteUserAsync(vatNumber);
            if (res == null)
            {
                return BadRequest();
            }
            return Ok();
        }

    }
}
