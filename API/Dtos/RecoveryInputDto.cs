namespace API.Dtos;

public class RecoveryInputDto
{
    public double SleepHours { get; set; }
    public required string WorkoutIntensity { get; set; }
    public required string FatigueLevel { get; set; } 
}
