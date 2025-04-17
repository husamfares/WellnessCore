using System;

namespace API.Dtos;

public class AnswerDto
{
    public int Id { get; set; }
    public required string AnswerText { get; set; }
    public required string AnsweredBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
