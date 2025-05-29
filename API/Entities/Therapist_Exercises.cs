using API.Migrations;

namespace API.Entities;

public class TherapistExercise
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public string? TargetArea { get; set; }
    public string? SuitableFor { get; set; }

    public string? Instructions { get; set; }

    public string? YoutubeUrl { get; set; }
    
    // public List<TherapistExercises>? ExerciseInfo { get; set; }
}