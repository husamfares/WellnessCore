using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseImportController : ControllerBase
    {
        private readonly ExerciseImportService _importService;
        private readonly ILogger<ExerciseImportController> _logger;

        public ExerciseImportController(ExerciseImportService importService, ILogger<ExerciseImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromQuery] int targetCount = 480, [FromQuery] List<string>? bodyParts = null)
        {
            try
            {
                if (bodyParts == null || bodyParts.Count == 0)
                {
                    bodyParts = new List<string> { "back", "chest", "lower arms", "lower legs", "shoulders", "upper arms", "upper legs", "waist", "cardio" };
                }

                await _importService.ImportExercisesFromApiAsync(targetCount, bodyParts, HttpContext.RequestAborted);

                return Ok($"Exercises imported successfully. Target: {targetCount} exercises for {string.Join(", ", bodyParts)}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing exercises.");
                return StatusCode(500, $"Error importing exercises: {ex.Message}");
            }
        }
    }
}
