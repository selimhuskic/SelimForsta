using MediatR;
using QuizService.Model.Domain;

namespace QuizService.Queries
{
    public class GetQuizByIdQuery : IRequest<Quiz>
    {
        public int Id { get; }

        public GetQuizByIdQuery(int id)
        {
            Id = id;
        }
    }
}
