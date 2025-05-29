using System;
namespace API.Dtos;

public class MealAnalysisDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public string Food { get; set; } = string.Empty;
    public int Calories { get; set; }
    public double Protein { get; set; }
    public DateTime CreatedAt { get; set; }
}
