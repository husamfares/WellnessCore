using System;

namespace API.Entities;

public class TherapistSessionPrice
{
     public int Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }

    public int TherapistId { get; set; }
    public AppUser Therapist { get; set; } = null!;
}
