using System;

namespace API.Dtos;

public class QuestionDto
{
    public int Id { get; set; }
    public required string Caption { get; set; }
    public required string AskedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<AnswerDto>? Answers { get; set; }
}
