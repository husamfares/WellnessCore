namespace API.Dtos;

public class BodyPartExercisesDto
{
    public string? BodyPart { get; set; }

    public List<ExerciseDto>? Exercises { get; set; }
}