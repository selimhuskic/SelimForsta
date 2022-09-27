using System.Collections.Generic;
using MediatR;
using QuizService.Model.Domain;

namespace QuizService.Queries
{
    public class GetAllQuizzesQuery : IRequest<IEnumerable<Quiz>>
    {
    }
}
