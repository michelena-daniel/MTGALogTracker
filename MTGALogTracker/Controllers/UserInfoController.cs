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
        public async Task<IActionResult> GetUsersByMtgArenaIds([FromQuery] List<string> mtgArenaIds)
        {
            if(mtgArenaIds == null || !mtgArenaIds.Any())
            {
                return BadRequest("Ids cannot be empty.");
            }
            var users = await _userInfoRepository.GetUsersByMtgArenaIds(mtgArenaIds);
            return Ok(users);
        }
    }
}
