using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MTGALogTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        public MatchController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [HttpGet("{mtgArenaId}")]
        public async Task<IActionResult> GetMatchesByMtgArenaId([FromRoute] string mtgArenaId)
        {
            if (string.IsNullOrEmpty(mtgArenaId))
                return BadRequest("Id cannot be empty");
            var matches = await _matchRepository.GetMatchesByMtgArenaId(mtgArenaId);
            if (!matches.Any())
                return NotFound($"Matches not found for player id {mtgArenaId}");
            return Ok(matches);
        }
    }
}
