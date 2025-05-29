using API.Data;
using Microsoft.EntityFrameworkCore;

public class WorkoutPlanSeeder
{
    private readonly DataContext _context;

    public WorkoutPlanSeeder(DataContext context)
    {
        _context = context;
    }

    public async Task SeedPredefinedPlansAsync()
    {

            if (!_context.Exercises.Any())
            {
                throw new Exception("Exercises are not yet imported. Cannot seed workout plans.");
            }

    
        if (_context.WorkoutPlans.Any(wp => wp.Type == "predefined"))
            return; // Already seeded

        var goals = new[] { "Build Muscle", "Lose Weight", "Improve Overall Health", "Improve Endurance" };
        var levels = new[] { "Beginner", "Moderate", "Advanced" };

        var exercises = await _context.Exercises.ToListAsync();

        var exercisesByGroup = exercises
            .GroupBy(e => MapBodyPartToClassification(e.BodyPart ?? "other"))
            .ToDictionary(g => g.Key, g => g.ToList());

        var customDistributions = GetCustomDistributions();


    foreach (var goal in goals)
    {
        foreach (var level in levels)
        {
            if (!customDistributions.TryGetValue((goal, level), out var distribution))
                continue;

            var selectedExercisesByGroup  = PickExercisesByGroup(exercisesByGroup, distribution);

                var plan = new WorkoutPlan
                {
                    Title = $"{level} - {goal} Plan",
                    FitnessLevel = level,
                    Goal = goal,
                    Type = "predefined",
                    Description = $"Custom plan for {goal} ({level})",
                    WorkoutPlanExercises = new List<WorkoutPlanExercise>()
                };

                int day = 1;
                int order = 1;

                foreach (var group in selectedExercisesByGroup)
                {
                    foreach (var exercise in group.Value)
                    {
                        var workoutExercise = new WorkoutPlanExercise
                        {
                            ExerciseId = exercise.Id,
                            Day = day,
                            Order = order
                        };

                        plan.WorkoutPlanExercises.Add(workoutExercise);

                        // Increment day and order for next exercise
                        order++;
                        if (order > 6)
                        {
                            order = 1;
                            day = day % 5 + 1; // Cycle through 1–5
                        }
                    }
                }

                _context.WorkoutPlans.Add(plan);

            }
        }

        await _context.SaveChangesAsync();
    }

private Dictionary<string, List<Exercise>> PickExercisesByGroup(Dictionary<string, List<Exercise>> exercisesByGroup, Dictionary<string, int> distribution)
{
    var rand = new Random();
    var selected = new Dictionary<string, List<Exercise>>();

    foreach (var kvp in distribution)
    {
        var category = kvp.Key.ToLower();
        var count = kvp.Value;

        if (exercisesByGroup.TryGetValue(category, out var exercises) && exercises.Count >= count)
        {
            selected[category] = exercises.OrderBy(x => rand.Next()).Take(count).ToList();
        }
        else
        {
            throw new Exception($"Not enough exercises in category '{category}' to meet the required count: {count}");
        }
    }

    return selected;
}


    private string MapBodyPartToClassification(string bodyPart)
    {
        if (string.IsNullOrWhiteSpace(bodyPart))
            return "other";

        bodyPart = bodyPart.ToLower();

        return bodyPart switch
            {
                "chest" => "chest",
                "back" => "back",
                "waist" => "core",
                "lower arms" or "upper arms" => "arms",
                "lower legs" or "upper legs" => "legs",
                "shoulders" => "shoulders",
                "cardio" => "cardio",
                _ => "other"
            };

    }

        private Dictionary<(string Goal, string Level), Dictionary<string, int>> GetCustomDistributions()
            {
                return new Dictionary<(string Goal, string Level), Dictionary<string, int>>
                {
                    // Build Muscle
                    { ("Build Muscle", "Beginner"), new Dictionary<string, int>
                        {
                            { "chest", 6 }, { "back", 6 }, { "legs", 6 }, { "arms", 4 }, { "shoulders", 4 }, { "core", 4 }, { "cardio", 2 }
                        }
                    },
                    { ("Build Muscle", "Moderate"), new Dictionary<string, int>
                        {
                            { "chest", 7 }, { "back", 6 }, { "legs", 7 }, { "arms", 5 }, { "shoulders", 5 }, { "core", 4 }, { "cardio", 2 }
                        }
                    },
                    { ("Build Muscle", "Advanced"), new Dictionary<string, int>
                        {
                            { "chest", 8 }, { "back", 7 }, { "legs", 8 }, { "arms", 6 }, { "shoulders", 6 }, { "core", 5 } , { "cardio", 0 }
                        }
                    },

                    // Lose Weight
                    { ("Lose Weight", "Beginner"), new Dictionary<string, int>
                        {
                            { "legs", 6 }, { "core", 6 }, { "arms", 5 }, { "shoulders", 5 }, { "back", 4 }, { "chest", 4 }, { "cardio", 5 }
                        }
                    },
                    { ("Lose Weight", "Moderate"), new Dictionary<string, int>
                        {
                            { "legs", 7 }, { "core", 6 }, { "arms", 5 }, { "shoulders", 4 }, { "back", 4 }, { "chest", 4 }, { "cardio", 6 }
                        }
                    },
                    { ("Lose Weight", "Advanced"), new Dictionary<string, int>
                        {
                            { "legs", 7 }, { "core", 5 }, { "arms", 5 }, { "shoulders", 4 }, { "back", 4 }, { "chest", 4 }, { "cardio", 10 }
                        }
                    },

                    // Improve Health
                    { ("Improve Overall Health", "Beginner"), new Dictionary<string, int>
                        {
                            { "legs", 6 }, { "core", 6 }, { "arms", 4 }, { "shoulders", 4 }, { "back", 4 }, { "chest", 4 }, { "cardio", 6 }
                        }
                    },
                    { ("Improve Overall Health", "Moderate"), new Dictionary<string, int>
                        {
                            { "legs", 6 }, { "core", 5 }, { "arms", 4 }, { "shoulders", 4 }, { "back", 4 }, { "chest", 4 }, { "cardio", 8 }
                        }
                    },
                    { ("Improve Overall Health", "Advanced"), new Dictionary<string, int>
                        {
                            { "legs", 6 }, { "core", 5 }, { "arms", 4 }, { "shoulders", 4 }, { "back", 4 }, { "chest", 4 }, { "cardio", 10 }
                        }
                    },

                    // Improve Endurance
                    { ("Improve Endurance", "Beginner"), new Dictionary<string, int>
                        {
                            { "legs", 5 }, { "core", 5 }, { "arms", 4 }, { "shoulders", 3 }, { "back", 3 }, { "chest", 3 }, { "cardio", 10 }
                        }
                    },
                    { ("Improve Endurance", "Moderate"), new Dictionary<string, int>
                        {
                            { "legs", 4 }, { "core", 4 }, { "arms", 4 }, { "shoulders", 3 }, { "back", 3 }, { "chest", 3 }, { "cardio", 15 }
                        }
                    },
                    { ("Improve Endurance", "Advanced"), new Dictionary<string, int>
                        {
                            { "legs", 3 }, { "core", 3 }, { "arms", 3 }, { "shoulders", 3 }, { "back", 3 }, { "chest", 3 }, { "cardio", 20 }
                        }
                    }
                };
            }


    
}
