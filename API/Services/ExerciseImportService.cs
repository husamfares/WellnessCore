using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.Dtos;
using API.Data;
using Microsoft.EntityFrameworkCore;

public class ExerciseImportService
{
   
    private readonly HttpClient _httpClient;
    private readonly DataContext _context;

    // The base URL of the Exercise API
    private readonly string BaseUrl = "https://exercisedb.p.rapidapi.com/exercises";

    // API Key for RapidAPI
    private readonly string RapidApiKey = "f13ec23faamsh77fa19b43fd4ac8p172375jsn16d31dbcf104";

    public ExerciseImportService(HttpClient httpClient, DataContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }


            public async Task ImportExercisesFromApiAsync(int targetCount = 480, List<string>? bodyParts = null, CancellationToken none = default)
{
    var defaultBodyParts = new List<string> { "back", "chest", "lower arms", "lower legs", "shoulders", "upper arms", "upper legs", "waist", "cardio" };
    var selectedBodyParts = bodyParts ?? defaultBodyParts;

    var seen = new HashSet<string>();
    var exercisesToAdd = new List<Exercise>();

    int baseTargetPerBodyPart = targetCount / selectedBodyParts.Count;
    int remainder = targetCount % selectedBodyParts.Count;

    // Distribute the remainder among the first few body parts
    var bodyPartTargets = selectedBodyParts
        .Select((bp, i) => new { BodyPart = bp, Target = baseTargetPerBodyPart + (i < remainder ? 1 : 0) })
        .ToList();

    foreach (var entry in bodyPartTargets)
    {
        string bodyPart = entry.BodyPart;
        int perBodyPartTarget = entry.Target;

        int offset = 0;
        const int limit = 100;
        int collectedForThisPart = 0;

        while (collectedForThisPart < perBodyPartTarget)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/bodyPart/{bodyPart}?limit={limit}&offset={offset}");
            request.Headers.Add("X-RapidAPI-Key", RapidApiKey);
            request.Headers.Add("X-RapidAPI-Host", "exercisedb.p.rapidapi.com");

            var response = await _httpClient.SendAsync(request, none);
            if (!response.IsSuccessStatusCode) break;

            var exercises = await response.Content.ReadFromJsonAsync<List<ExerciseDto>>();
            if (exercises == null || exercises.Count == 0) break;

            bool newExercisesFound = false;

            foreach (var exercise in exercises)
            {
                var key = $"{exercise.Name}-{exercise.BodyPart}";
                if (seen.Contains(key)) continue;

                var existingExercise = await _context.Exercises
                    .FirstOrDefaultAsync(e => e.Name == exercise.Name && e.BodyPart == exercise.BodyPart);

                if (existingExercise == null)
                {
                    var dbExercise = new Exercise
                    {
                        Name = exercise.Name,
                        BodyPart = exercise.BodyPart,
                        Equipment = exercise.Equipment,
                        GifUrl = exercise.GifUrl,
                        Target = exercise.Target,
                        SecondaryMuscles = exercise.SecondaryMuscles ?? new List<string>(),
                        Instructions = exercise.Instructions ?? new List<string>(),
                    };

                    exercisesToAdd.Add(dbExercise);
                    seen.Add(key);
                    newExercisesFound = true;
                    collectedForThisPart++;
                }

                if (collectedForThisPart >= perBodyPartTarget)
                    break;
            }

            if (!newExercisesFound || exercises.Count < limit)
                break;

            offset += limit;
        }
    }

    if (exercisesToAdd.Count > 0)
    {
        _context.Exercises.AddRange(exercisesToAdd);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Successfully imported {exercisesToAdd.Count} exercises.");
    }
    else
    {
        Console.WriteLine("⚠️ No exercises to import.");
    }
}

}