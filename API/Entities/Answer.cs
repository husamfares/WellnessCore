using System;

namespace API.Entities;

public class Answer
{
    public int Id { get; set; }
    public string? AnswerText { get; set; }
    public string? AnsweredBy { get; set; }  // Username
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}
