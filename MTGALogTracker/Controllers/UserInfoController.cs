using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MTGALogTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfoController : ControllerBase
    {
        private IUserInfoRepository _userInfoRepository;
        public UserInfoController(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userInfoRepository.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("usernames")]
        public async Task<IActionResult> GetUsersByUsernames([FromQuery] List<string> userNames)
        {
            if(userNames == null || !userNames.Any())
            {
                return BadRequest("Usernames cannot be empty.");
            }
            var users = await _userInfoRepository.GetUsersByUserNames(userNames);
            return Ok(users);
        }
    }
}
