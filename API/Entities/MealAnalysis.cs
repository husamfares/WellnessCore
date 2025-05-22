using System;
using Newtonsoft.Json;

namespace API.Entities;

public class MealAnalysis
{
    public int Id { get; set; }
    public string Food { get; set; } = string.Empty;
    public int Calories { get; set; }

    [JsonProperty("protein_g")]
    public double Protein { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
