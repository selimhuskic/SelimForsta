using System.Collections.Generic;
using MediatR;
using QuizService.Model.Domain;

namespace QuizService.Queries
{
    public class GetQuestionsByIdQuery : IRequest<IEnumerable<Question>>
    {
        public int Id { get; }

        public GetQuestionsByIdQuery(int id)
        {
            Id = id;
        }
    }
}