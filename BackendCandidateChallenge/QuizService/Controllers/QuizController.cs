using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using QuizService.Queries;

namespace QuizService.Controllers;

[Route("api/quizzes")]
public class QuizController : Controller
{
    private readonly IDbConnection _connection;
    private readonly IMediator _mediator;


    // TODO
    // All 'computation'/'logic' would have been in a separate DLL,
    // a Domain DLL, where it would have been reference free (pure C#) in order to test the core of the project
    // without impediments. Some Domain driven approach is what I prefer.

    // TODO
    // The changes I've made with MediatR and Queries and QueryHandlers reflect my CQRS preference,
    // and only one layer (the handlers) between the API and the Domain layer. 
    // Put, Post and Update would have been placed in Commands and Command handlers.

    // TODO
    // A remark: some DB factory would have been made in order to instantiate SqlConnections, but 
    // I did not make one, as you said, this is only a short challenge meant to be done in a couple of hours.
    // Also, I haven't dealt with Sqllite much, so I decided that using your Singleton was the easiest approach.


    // TODO 
    // Modeling is something I see as the most important part of development, so in a more real-world-like app,
    // I'd focus on that the most. Just a heads up, if we get to talk about this in the future.

    // TODO
    // API, Service/Infrastructure and Domain levels are the levels I would have separated QuizService into, at the very least.

    public QuizController(IDbConnection connection, IMediator mediator)
    {
        _connection = connection;
        _mediator = mediator;
    }

    // GET api/quizzes
    [HttpGet]
    public async Task<IEnumerable<QuizResponseModel>> Get()
    {
        var quizzes = await _mediator.Send(new GetAllQuizzesQuery());

        return quizzes.Select(quiz =>
            new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title
            });
    }

    // GET api/quizzes/5
    [HttpGet("{id}")]
    public async Task<object> Get(int id)
    {
        var quiz = await _mediator.Send(new GetQuizByIdQuery(id));

        if (quiz == null)
            return NotFound();

        var questions = await _mediator.Send(new GetQuestionsByIdQuery(id));

        var answers = await _mediator.Send(new GetAnswersByQuizIdQuery(id));

        return new QuizResponseModel
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Questions = questions.Select(question => new QuizResponseModel.QuestionItem
            {
                Id = question.Id,
                Text = question.Text,
                Answers = answers.ContainsKey(question.Id)
                    ? answers[question.Id].Select(answer => new QuizResponseModel.AnswerItem
                    {
                        Id = answer.Id,
                        Text = answer.Text
                    })
                    : new QuizResponseModel.AnswerItem[0],
                CorrectAnswerId = question.CorrectAnswerId
            }),
            Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
        };
    }

    // POST api/quizzes
    [HttpPost]
    public IActionResult Post([FromBody]QuizCreateModel value)
    {
        var sql = $"INSERT INTO Quiz (Title) VALUES('{value.Title}'); SELECT LAST_INSERT_ROWID();";
        var id = _connection.ExecuteScalar(sql);
        return Created($"/api/quizzes/{id}", null);
    }

    // PUT api/quizzes/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody]QuizUpdateModel value)
    {
        const string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";
        int rowsUpdated = _connection.Execute(sql, new {Id = id, Title = value.Title});
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        const string sql = "DELETE FROM Quiz WHERE Id = @Id";
        int rowsDeleted = _connection.Execute(sql, new {Id = id});
        if (rowsDeleted == 0)
            return NotFound();
        return NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id}/questions")]
    public IActionResult PostQuestion(int id, [FromBody]QuestionCreateModel value)
    {
        const string sql = "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();";
        var questionId = _connection.ExecuteScalar(sql, new {Text = value.Text, QuizId = id});
        return Created($"/api/quizzes/{id}/questions/{questionId}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id}/questions/{qid}")]
    public IActionResult PutQuestion(int id, int qid, [FromBody]QuestionUpdateModel value)
    {
        const string sql = "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @QuestionId";
        int rowsUpdated = _connection.Execute(sql, new {QuestionId = qid, Text = value.Text, CorrectAnswerId = value.CorrectAnswerId});
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public IActionResult DeleteQuestion(int id, int qid)
    {
        const string sql = "DELETE FROM Question WHERE Id = @QuestionId";
        _connection.ExecuteScalar(sql, new {QuestionId = qid});
        return NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public IActionResult PostAnswer(int id, int qid, [FromBody]AnswerCreateModel value)
    {
        const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
        var answerId = _connection.ExecuteScalar(sql, new {Text = value.Text, QuestionId = qid});
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult PutAnswer(int id, int qid, int aid, [FromBody]AnswerUpdateModel value)
    {
        const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
        int rowsUpdated = _connection.Execute(sql, new {AnswerId = qid, Text = value.Text});
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult DeleteAnswer(int id, int qid, int aid)
    {
        const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
        _connection.ExecuteScalar(sql, new {AnswerId = aid});
        return NoContent();
    }

    [HttpPost]
    [Route("{id}/questions/{qid}/answers/{aid}/check")]
    public async Task<IActionResult> TakeQuiz(int id, int qid, int aid)
    {
        var answers = await _mediator.Send(new GetAnswersByQuizIdQuery(id));

        if (answers[id].Any(x => x.QuestionId == qid && x.Id == aid))
            return Ok();

        return NotFound();
    }
}