using System;

namespace API.Dtos;

public class TherapistSessionPriceDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public decimal Price { get; set; }
}
