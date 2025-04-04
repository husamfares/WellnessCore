namespace API.Dtos;

public class WellnessInfoDto
{
   public int UserId { get; set; } 
   public int Age { get; set; }

    public int Weight { get; set; }
    public int Height { get; set; }

    public required string Gender { get; set; }
}