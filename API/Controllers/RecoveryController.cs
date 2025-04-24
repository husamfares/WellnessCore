using API.Entities;
using API.Interfaces;
using API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class RecoveryController(IRecoveryRepository repo) : BaseApiController

{
    [HttpPost("calculate")]
public ActionResult<double> CalculateRecovery([FromBody] RecoveryInputDto input)
{
    double sleepMultiplier = input.SleepHours >= 7 ? 1.0 : 0.8;

    double intensityMultiplier = input.WorkoutIntensity switch
    {
        "light" => 1.0,
        "moderate" => 0.9,
        "intense" => 0.75,
        _ => 0.85
    };

    double fatigueMultiplier = input.FatigueLevel switch
    {
        "low" => 1.0,
        "medium" => 0.85,
        "high" => 0.7,
        _ => 0.85
    };

    var recovery = 100 * sleepMultiplier * intensityMultiplier * fatigueMultiplier;

    return Ok(Math.Round(recovery, 2));
}


}