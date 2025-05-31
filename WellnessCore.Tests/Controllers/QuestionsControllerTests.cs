using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class QuestionsControllerTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public QuestionsControllerTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }

    [Fact]
    public async Task PostQuestion_ReturnsOkResult_WithCreatedQuestion()
    {
        // Arrange
        var context = new DataContext(_dbContextOptions);
        var controller = new QuestionsController(context);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "testuser")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var newQuestion = new CreateQuestionDto
        {
            Caption = "What is unit testing?"
        };

        // Act
        var result = await controller.PostQuestion(newQuestion);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var questionDto = Assert.IsType<QuestionDto>(okResult.Value);
        Assert.Equal("testuser", questionDto.AskedBy);
        Assert.Equal("What is unit testing?", questionDto.Caption);
    }

    [Fact]
    public async Task PostAnswer_ReturnsOkResult_WithCreatedAnswer()
    {
        // Arrange
        var context = new DataContext(_dbContextOptions);
        var question = new Question
        {
            Caption = "Sample?",
            AskedBy = "someone",
            CreatedAt = DateTime.UtcNow
        };
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var controller = new QuestionsController(context);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.Name, "testuser")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var answerDto = new CreateAnswerDto
        {
            AnswerText = "This is an answer."
        };

        // Act
        var result = await controller.PostAnswer(question.Id, answerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedAnswer = Assert.IsType<AnswerDto>(okResult.Value);
        Assert.Equal("testuser", returnedAnswer.AnsweredBy);
        Assert.Equal("This is an answer.", returnedAnswer.AnswerText);
    }

    [Fact]
public async Task GetQuestions_ReturnsListOfQuestions()
{
    var options = new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Isolated DB
        .Options;

    var context = new DataContext(options);
    context.Questions.Add(new Question
    {
        Caption = "Test Question?",
        AskedBy = "user1",
        CreatedAt = DateTime.UtcNow,
        Answers = new List<Answer>
        {
            new Answer
            {
                AnswerText = "Answer 1",
                AnsweredBy = "user2",
                CreatedAt = DateTime.UtcNow
            }
        }
    });
    await context.SaveChangesAsync();

    var controller = new QuestionsController(context);

    // Act
    var result = await controller.GetQuestions();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var questions = Assert.IsType<List<QuestionDto>>(okResult.Value);
    Assert.Single(questions); // ✅ Now this should pass
}



    [Fact]
    public async Task UpdateAnswer_UpdatesAnswer_WhenAuthorized()
    {
        // Arrange
        var context = new DataContext(_dbContextOptions);
        var question = new Question
        {
            Caption = "Q",
            AskedBy = "user",
            CreatedAt = DateTime.UtcNow
        };
        var answer = new Answer
        {
            AnswerText = "Old Text",
            AnsweredBy = "testuser",
            CreatedAt = DateTime.UtcNow,
            Question = question
        };
        context.Questions.Add(question);
        context.Answers.Add(answer);
        await context.SaveChangesAsync();

        var controller = new QuestionsController(context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "testuser")
                }, "mock"))
            }
        };

        var updatedDto = new CreateAnswerDto { AnswerText = "Updated Text" };

        // Act
        var result = await controller.UpdateAnswer(question.Id, answer.Id, updatedDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var updatedAnswer = await context.Answers.FindAsync(answer.Id);
        Assert.Equal("Updated Text", updatedAnswer!.AnswerText);
    }


[Fact]
public async Task DeleteAnswer_ReturnsUnauthorized_IfNotOwner()
{
    // Arrange
    var context = new DataContext(_dbContextOptions);
    var question = new Question { Caption = "Q", AskedBy = "someone", CreatedAt = DateTime.UtcNow };
    var answer = new Answer
    {
        AnswerText = "Test",
        AnsweredBy = "owner",
        Question = question,
        CreatedAt = DateTime.UtcNow
    };
    context.Questions.Add(question);
    context.Answers.Add(answer);
    await context.SaveChangesAsync();

    var controller = new QuestionsController(context);
    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "otheruser")
            }, "mock"))
        }
    };

    // Act
    var result = await controller.DeleteAnswer(question.Id, answer.Id);

    // Assert
    var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal("You can only delete your own answers", unauthorized.Value);
}


}
