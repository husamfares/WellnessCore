public class WorkoutPlanExercise
{
    public int Id { get; set; }

    public int WorkoutPlanId { get; set; }
    public WorkoutPlan? WorkoutPlan { get; set; }

    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }

    public int Day { get; set; }
    public int Order { get; set; }
}
