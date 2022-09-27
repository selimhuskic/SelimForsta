using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    private const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using var testHost = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
        
        var client = testHost.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
            content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using var testHost = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());

        var client = testHost.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);
        var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
        Assert.Equal(quizId, quiz?.Id);
        Assert.Equal("My first quiz", quiz?.Title);
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using var testHost = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());

        var client = testHost.CreateClient();
        const long quizId = 999;
        var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
        
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        using var testHost = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());


        var client = testHost.CreateClient();
        const long quizId = 999;
        var question = new QuestionCreateModel("The answer to everything is what?");
        var content = new StringContent(JsonConvert.SerializeObject(question));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"),content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // TODO
    // The test would have been different as the modeling would have been different as well.
    // This test reflects what I have been able to read from the code in approximately two hours.
    // TDD is an approach I like using, so there would have been a lot of refactoring.

    [Fact]
    public async Task GivenThatAQuizWithTwoQuestionsIsCreated_WhenTestTakenAndAnsweredCorrectly_WeGetAScoreOfTwo()
    {
        // Given
        var quiz = new QuizCreateModel("Challenge quiz");

        using var testHost = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());

        var client = testHost.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
            content);

        var quizQuestionOne = new QuestionCreateModel("Did I pass this test?");
        var quizQuestionTwo = new QuestionCreateModel("What did I think of this test?");

        var addQuestionLocation = response.Headers.Location + "/questions";

        var contentQuestionOne = new StringContent(JsonConvert.SerializeObject(quizQuestionOne));
        contentQuestionOne.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var contentQuestionTwo = new StringContent(JsonConvert.SerializeObject(quizQuestionTwo));
        contentQuestionTwo.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var createResponse =  await client.PostAsync(new Uri(testHost.BaseAddress, addQuestionLocation), contentQuestionOne);
        await client.PostAsync(new Uri(testHost.BaseAddress, addQuestionLocation), contentQuestionTwo);

        var quizAnswerOne = new AnswerCreateModel("Yes");
        var quizAnswerTwo = new AnswerCreateModel("Not bad");

        var addAnswerOne = createResponse.Headers.Location + "/answers";
        var addAnswerTwo = createResponse.Headers.Location + "/answers";

        var contentAnswerOne = new StringContent(JsonConvert.SerializeObject(quizAnswerOne));
        contentAnswerOne.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var contentAnswerTwo = new StringContent(JsonConvert.SerializeObject(quizAnswerTwo));
        contentAnswerTwo.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var answerOneCreateResponse = await client.PostAsync(new Uri(testHost.BaseAddress, addAnswerOne), contentAnswerOne);
        var answerTwoCreateResponse = await client.PostAsync(new Uri(testHost.BaseAddress, addAnswerTwo), contentAnswerTwo);

        // When
        var checkAnswerOne = answerOneCreateResponse.Headers.Location + "/check";
        var checkAnswerTwo = answerTwoCreateResponse.Headers.Location + " /check";

        var contentCheckAnswerOne = new StringContent(JsonConvert.SerializeObject(checkAnswerOne));
        contentCheckAnswerOne.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var contentCheckAnswerTwo = new StringContent(JsonConvert.SerializeObject(checkAnswerTwo));
        contentCheckAnswerTwo.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var answerOneResponseMessage = await client.PostAsync(new Uri(testHost.BaseAddress, checkAnswerOne), contentCheckAnswerOne);
        var answerTwoResponseMessage = await client.PostAsync(new Uri(testHost.BaseAddress, checkAnswerTwo), contentCheckAnswerTwo);


        // Then
        var correctAnswerCount = 0;

        if (answerOneResponseMessage.IsSuccessStatusCode)
            correctAnswerCount++;

        if (answerTwoResponseMessage.IsSuccessStatusCode)
            correctAnswerCount++;

        Assert.True(correctAnswerCount == 2);
    }
}