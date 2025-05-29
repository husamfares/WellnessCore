public class WorkoutPlan
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Goal { get; set; } // e.g., "lose_weight", etc.
    public string? FitnessLevel { get; set; } // e.g., "beginner", etc.
    public string? Type { get; set; } // "standard", "home", etc.
    public string? Description { get; set; }

    public ICollection<WorkoutPlanExercise>? WorkoutPlanExercises { get; set; }
}
