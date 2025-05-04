using System;

namespace API.Entities;

public class TrainerSubscription
{
     public int Id { get; set; }
    public string? Type { get; set; } // "Online" or "InPerson"
    public string? Title { get; set; } // Example: "1 Month"
    public decimal Price { get; set; }

    public int TrainerId { get; set; }
    public AppUser? Trainer { get; set; }
}
