using System.Collections.Generic;
using MediatR;
using QuizService.Model.Domain;

namespace QuizService.Queries
{
    public class GetAnswersByQuizIdQuery : IRequest<Dictionary<int, IList<Answer>>>
    {
        public int Id { get; }

        public GetAnswersByQuizIdQuery(int id)
        {
            Id = id;
        }
    }
}
