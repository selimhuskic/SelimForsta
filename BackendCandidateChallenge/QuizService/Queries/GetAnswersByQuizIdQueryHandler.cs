using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuizService.Model.Domain;
using QuizService.Repositories;

namespace QuizService.Queries
{
    public class GetAnswersByQuizIdQueryHandler
        : IRequestHandler<GetAnswersByQuizIdQuery, Dictionary<int, IList<Answer>>>
    {
        private readonly IAnswersRepository _answersRepository;

        public GetAnswersByQuizIdQueryHandler(IDbConnection dbConnection)
        {
            _answersRepository = new AnswersRepository(dbConnection, nameof(Answer));
        }

        public async Task<Dictionary<int, IList<Answer>>> Handle(GetAnswersByQuizIdQuery request, CancellationToken cancellationToken)
        {
            return await _answersRepository.GetAnswerDictionaryByQuizId(request.Id);
        }
    }
}
