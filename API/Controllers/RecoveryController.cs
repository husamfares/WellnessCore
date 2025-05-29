using API.Entities;
using API.Interfaces;
using API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class RecoveryController : BaseApiController
{
   

    [HttpPost("calculate")]
public ActionResult<double> CalculateRecovery([FromBody] RecoveryInputDto input)
{
    double sleepEffect = input.SleepHours >= 7 
        ? 1.0 
        : Math.Pow(input.SleepHours / 7.0, 1.5);

    double intensityEffect = input.WorkoutIntensity switch
    {
        "light" => 1.0,
        "moderate" => 0.85,
        "intense" => 0.65,
        _ => 0.75
    };

    double fatigueEffect = input.FatigueLevel switch
    {
        "low" => 1.0,
        "medium" => 0.7,
        "high" => 0.5,
        _ => 0.7
    };

    double recovery = 100 * sleepEffect * intensityEffect * fatigueEffect;
    recovery = Math.Clamp(recovery, 0, 100);

    return Ok(Math.Round(recovery, 2));
}

}
