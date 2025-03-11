using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MTGALogTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerRankController : ControllerBase
    {
        private readonly IPlayerRankRepository _playerRankRepository;
        public PlayerRankController(IPlayerRankRepository playerRankRepository)
        {
            _playerRankRepository = playerRankRepository;
        }

        [HttpGet("{playerNameWithCode}")]
        public async Task<IActionResult> GetRanksByPlayer([FromRoute] string playerNameWithCode)
        {
            if (string.IsNullOrEmpty(playerNameWithCode))
                return BadRequest("Player name cannot be empty");
            var ranks = await _playerRankRepository.GetPlayerRanksByPlayerName(playerNameWithCode);
            if (!ranks.Any())
                return NotFound($"Ranks not found for player {playerNameWithCode}");
            return Ok(ranks);
        }
    }
}
