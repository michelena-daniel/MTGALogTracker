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

        [HttpGet("{mtgArenaId}")]
        public async Task<IActionResult> GetRanksByPlayer([FromRoute] string mtgArenaId)
        {
            if (string.IsNullOrEmpty(mtgArenaId))
                return BadRequest("Id cannot be empty");
            var ranks = await _playerRankRepository.GetRanksByMtgArenaId(mtgArenaId);
            if (!ranks.Any())
                return NotFound($"Ranks not found for player id {mtgArenaId}");
            return Ok(ranks);
        }
    }
}
