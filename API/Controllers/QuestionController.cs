using API.Controllers;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Trainer,Therapist")]
    [HttpPost("{id}/answers")]
    public async Task<ActionResult<AnswerDto>> PostAnswer(int id, [FromBody] CreateAnswerDto answerDto)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized("User not authenticated");

        var questions = await context.Questions.ToListAsync();
        if (questions == null) return Ok(questions);

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

    [Authorize(Roles = "Trainer,Therapist")]
    [HttpPut("{questionId}/answers/{answerId}")]
    public async Task<ActionResult> UpdateAnswer(int questionId, int answerId, [FromBody] CreateAnswerDto updatedDto)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized("User not authenticated");

        var answer = await context.Answers.FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);
        if (answer == null) return NotFound("Answer not found");

        if (answer.AnsweredBy != username)
            return Forbid("You can only edit your own answers");

        answer.AnswerText = updatedDto.AnswerText;
        await context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet]
    public async Task<ActionResult<List<QuestionDto>>> GetQuestions()
    {
        var questions = await context.Questions
            .Include(q => q.Answers)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
        if (questions == null || questions.Count == 0) return Ok(questions);

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

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateQuestion(int id, [FromBody] CreateQuestionDto updatedDto)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Unauthorized("User not authenticated");

        var question = await context.Questions.FindAsync(id);
        if (question == null) return Ok("Question not found");

        if (question.AskedBy != username)
            return Forbid("You can only edit your own questions");

        question.Caption = updatedDto.Caption;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var username = User.Identity?.Name;
        var question = await context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
        if (question == null) return NotFound("Question not found");

        if (question.AskedBy != username) return Unauthorized("You can only delete your own questions");

        context.Questions.Remove(question);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Trainer,Therapist,Admin")]
    [HttpDelete("{questionId}/answers/{answerId}")]
    public async Task<IActionResult> DeleteAnswer(int questionId, int answerId)
    {
        var username = User.Identity?.Name;
        var answer = await context.Answers.FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);

        if (answer == null) return NotFound("Answer not found");

        if (answer.AnsweredBy != username) return Unauthorized("You can only delete your own answers");

        context.Answers.Remove(answer);
        await context.SaveChangesAsync();

        return NoContent();
    }

}
