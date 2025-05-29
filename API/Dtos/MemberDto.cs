using System;

namespace API.Dtos;

public class MemberDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    
    public int Age { get; set; }
    
    public  int Weight { get; set; }

    public  int Height { get; set; }

    public  string? Gender { get; set; } 

    public string? FitnessLevel { get; set; }

    public string? TraineeGoal { get; set; }

}
