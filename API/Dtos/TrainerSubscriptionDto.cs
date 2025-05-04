using System;

namespace API.Dtos;

public class TrainerSubscriptionDto
{
    public int Id { get; set; } 
    public string? Type { get; set; }
    public   string? Title { get; set; }
    public decimal Price { get; set; }
}
