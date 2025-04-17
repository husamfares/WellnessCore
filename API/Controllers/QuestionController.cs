using API.Controllers;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class QuestionsController(DataContext context) : BaseApiController
{
   
    [HttpPost]
    public async Task<ActionResult<QuestionDto>> PostQuestion([FromBody] CreateQuestionDto questionDto)
    {

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized("User not authenticated");

        var question = new Question
    {
        Caption = questionDto.Caption,
        AskedBy = username,
        CreatedAt = DateTime.UtcNow
    };

    context.Questions.Add(question);
    await context.SaveChangesAsync();

    return Ok(new QuestionDto
    {
        Id = question.Id,
        Caption = question.Caption,
        AskedBy = question.AskedBy,
        CreatedAt = question.CreatedAt,
        Answers = new List<AnswerDto>()
    });
}

    [HttpPost("{id}/answers")]
    public async Task<ActionResult<AnswerDto>> PostAnswer(int id, [FromBody] CreateAnswerDto answerDto)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized("User not authenticated");

        

        var question = await context.Questions.FindAsync(id);
        if (question == null) return NotFound("Question not found");

        var answer = new Answer
    {
        AnswerText = answerDto.AnswerText,
        AnsweredBy = username,
        QuestionId = id,
        CreatedAt = DateTime.UtcNow
    };

    context.Answers.Add(answer);
    await context.SaveChangesAsync();

    var answerResult = new AnswerDto
    {
        Id = answer.Id,
        AnswerText = answer.AnswerText,
        AnsweredBy = answer.AnsweredBy,
        CreatedAt = answer.CreatedAt
    };

    return Ok(answerResult);
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionDto>>> GetQuestions()
    {
        var questions = await context.Questions
            .Include(q => q.Answers)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
        if (questions == null || questions.Count == 0) return NotFound("No questions found");

        var result = questions.Select(q => new QuestionDto
    {
        Id = q.Id,
        Caption = q.Caption!,
        AskedBy = q.AskedBy!,
        CreatedAt = q.CreatedAt,
        Answers = q.Answers.Select(a => new AnswerDto
        {
            Id = a.Id,
            AnswerText = a.AnswerText!,
            AnsweredBy = a.AnsweredBy!,
            CreatedAt = a.CreatedAt
        }).ToList()!
    }).ToList();
        
        
       return Ok(result);
    }
}
